using MedicalCenter.Web.Attributes;
using MedicalCenter.Web.Constants;
using MedicalCenter.Web.Dtos.MedicalExamination;
using MedicalCenter.Web.Dtos.User;
using MedicalCenter.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MedicalCenter.Web.Controllers
{
    public class MedicalExaminationsController : Controller
    {
        private readonly MedicalCenterDbContext _context;
        private readonly bool _isAdministrator;
        private readonly User _loggedInUser;

        public MedicalExaminationsController(MedicalCenterDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _loggedInUser = _context.Users.FirstOrDefault(u => u.ID == httpContextAccessor.HttpContext!.Session.GetInt32("UserID"))!;
            _isAdministrator = _loggedInUser.Role.Description == "Administrator";
        }

        // GET: MedicalExaminations
        [AuthenticateAuthorize("Administrator")]
        public async Task<IActionResult> Index()
        {
            var medicalExaminations = await _context.MedicalExaminations.ToListAsync();
            List<GetMedicalExaminationDto> medicalExaminationDtos = new List<GetMedicalExaminationDto>();
            foreach (var medicalExamination in medicalExaminations)
            {
                medicalExaminationDtos.Add(new GetMedicalExaminationDto()
                {
                    ID = medicalExamination.ID,
                    Diagnosis = medicalExamination.Diagnosis,
                    Recommendation = medicalExamination.Recommendation,
                    StartTime = medicalExamination.StartTime,
                    EndTime = medicalExamination.EndTime,
                    PatientName = $"{medicalExamination.Patient.Name} {medicalExamination.Patient.Surname}",
                    DoctorName = $"{medicalExamination.Doctor.Name} {medicalExamination.Doctor.Surname}"
                });
            }

            return View(medicalExaminationDtos);
        }

        [HttpGet]
        [AuthenticateAuthorize]
        public async Task<IActionResult> CheckDoctorAvailability(int doctorId, DateTime startTime, DateTime endTime)
        {
            bool hasConflict = await IsMedicalExaminationConflictingForDoctorAsync(doctorId, startTime, endTime);
            return Json(!hasConflict); // true = available, false = conflict
        }

        [AuthenticateAuthorize]
        public IActionResult MyExaminations()
        {
            string role = _loggedInUser.Role.Description;
            int userId = _loggedInUser.ID;
            List<MedicalExamination> medicalExaminations = new List<MedicalExamination>();
            if (role == "Doctor")
            {
                medicalExaminations = _context.MedicalExaminations
                    .Where(me => me.Doctor.ID == userId)
                    .ToList();
            }
            else if (role == "Patient")
            {
                medicalExaminations = _context.MedicalExaminations
                    .Where(me => me.Patient.ID == userId)
                    .ToList();
            }

            return View(medicalExaminations);
        }

        // GET: MedicalExaminations/Details/5
        [AuthenticateAuthorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalExamination = await _context.MedicalExaminations.FirstOrDefaultAsync(m => m.ID == id);
            if (medicalExamination == null)
            {
                return NotFound();
            }

            if (!IsAuthorizedToViewMedicalExamination(medicalExamination) && !_isAdministrator)
            {
                return NotFound();
            }

            return View(medicalExamination);
        }

        // GET: MedicalExaminations/Create
        [AuthenticateAuthorize]
        public async Task<IActionResult> Create()
        {
            List<Doctor> doctors = await _context.Doctors.ToListAsync();
            AddMedicalExaminationDto medicalExaminationDto = new AddMedicalExaminationDto()
            {
                SelectedDoctor = "",
                Doctors = doctors.Select(r => new SelectListItem
                {
                    Value = r.ID.ToString(),
                    Text = $"{r.Name} {r.Surname}"
                }).ToList(),
                StartTime = DateTime.UtcNow.Date,
                EndTime = DateTime.UtcNow.Date
            };

            return View(medicalExaminationDto);
        }

        // POST: MedicalExaminations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Patient")]
        public async Task<IActionResult> Create(AddMedicalExaminationDto addMedicalExaminationDto)
        {
            if (ModelState.IsValid)
            {
                if (!int.TryParse(addMedicalExaminationDto.SelectedDoctor, out int doctorId))
                {
                    return NotFound();
                }

                Patient patient = await _context.Patients.FindAsync(_loggedInUser.ID);
                Doctor doctor = await _context.Doctors.FindAsync(doctorId);
                if (patient == null || doctor == null)
                {
                    return NotFound();
                }
                var doctors = await _context.Doctors.ToListAsync();
                addMedicalExaminationDto.Doctors = doctors.Select(r => new SelectListItem
                {
                    Value = r.ID.ToString(),
                    Text = $"{r.Name} {r.Surname}"
                }).ToList();

                if (addMedicalExaminationDto.StartTime > addMedicalExaminationDto.EndTime)
                {
                    ModelState.AddModelError(nameof(addMedicalExaminationDto.StartTime), "End time must be later than start time.");
                    ModelState.AddModelError(nameof(addMedicalExaminationDto.EndTime), "End time must be later than start time.");
                    return View(addMedicalExaminationDto);
                }

                if (await IsMedicalExaminationConflictingForDoctorAsync(doctor.ID, addMedicalExaminationDto.StartTime, addMedicalExaminationDto.EndTime))
                {
                    ModelState.AddModelError(nameof(addMedicalExaminationDto.StartTime), "The doctor already has a medical examination scheduled during this time.");
                    ModelState.AddModelError(nameof(addMedicalExaminationDto.EndTime), "The doctor already has a medical examination scheduled during this time.");
                    return View(addMedicalExaminationDto);
                }

                MedicalExamination medicalExamination = new MedicalExamination
                {
                    Patient = patient,
                    Doctor = doctor,
                    PatientID = patient.ID,
                    DoctorID = doctor.ID,
                    StartTime = addMedicalExaminationDto.StartTime,
                    EndTime = addMedicalExaminationDto.EndTime
                };
                _context.Add(medicalExamination);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyExaminations));
            }

            return View(addMedicalExaminationDto);
        }

        // GET: MedicalExaminations/Edit/5
        [AuthenticateAuthorize("Patient", "Doctor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalExamination = await _context.MedicalExaminations.FindAsync(id);
            if (medicalExamination == null)
            {
                return NotFound();
            }

            if (!IsAuthorizedToViewMedicalExamination(medicalExamination) && !_isAdministrator)
            {
                return NotFound();
            }

            var doctors = await _context.Doctors.ToListAsync();

            EditMedicalExaminationDto editMedicalExaminationDto = new EditMedicalExaminationDto
            {
                ID = medicalExamination.ID,
                PatientId = medicalExamination.PatientID,
                DoctorId = medicalExamination.DoctorID,
                StartTime = medicalExamination.StartTime,
                EndTime = medicalExamination.EndTime,
                Diagnosis = medicalExamination.Diagnosis,
                Recommendation = medicalExamination.Recommendation,
                SelectedDoctor = medicalExamination.DoctorID.ToString(),
                Doctors = doctors.Select(r => new SelectListItem
                {
                    Value = r.ID.ToString(),
                    Text = $"{r.Name} {r.Surname}",
                    Selected = r.ID == medicalExamination.DoctorID
                }).ToList()
            };

            return View(editMedicalExaminationDto);
        }

        // POST: MedicalExaminations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Patient", "Doctor")]
        public async Task<IActionResult> Edit(int id, EditMedicalExaminationDto editMedicalExaminationDto)
        {
            var medicalExamination = await _context.MedicalExaminations.FindAsync(id);
            if (medicalExamination == null)
            {
                return NotFound();
            }

            if (!IsAuthorizedToViewMedicalExamination(medicalExamination) && !_isAdministrator)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Patient patient = await _context.Patients.FindAsync(editMedicalExaminationDto.PatientId);
                    Doctor doctor = await _context.Doctors.FindAsync(editMedicalExaminationDto.DoctorId);
                    if (patient == null || doctor == null)
                    {
                        return NotFound();
                    }

                    if (await IsMedicalExaminationConflictingForDoctorAsync(editMedicalExaminationDto.DoctorId, editMedicalExaminationDto.StartTime, editMedicalExaminationDto.EndTime))
                    {
                        ModelState.AddModelError(nameof(editMedicalExaminationDto.StartTime), "The doctor already has a medical examination scheduled during this time.");
                        ModelState.AddModelError(nameof(editMedicalExaminationDto.EndTime), "The doctor already has a medical examination scheduled during this time.");
                        return View(editMedicalExaminationDto);
                    }

                    bool isLoggedInUserDoctor = _loggedInUser.RoleID == RoleConstants.DoctorRoleId;
                    if (isLoggedInUserDoctor)
                    {
                        medicalExamination.Diagnosis = editMedicalExaminationDto.Diagnosis;
                        medicalExamination.Recommendation = editMedicalExaminationDto.Recommendation;
                    }
                    medicalExamination.Patient = patient;
                    medicalExamination.Doctor = doctor;
                    medicalExamination.StartTime = editMedicalExaminationDto.StartTime;
                    medicalExamination.EndTime = editMedicalExaminationDto.EndTime;
                    _context.Update(medicalExamination);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalExaminationExists(medicalExamination.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            /*var doctors = await _context.Doctors.ToListAsync();

            EditMedicalExaminationDto editMedicalExaminationDto = new EditMedicalExaminationDto
            {
                ID = medicalExamination.ID,
                PatientId = medicalExamination.PatientID,
                DoctorId = medicalExamination.DoctorID,
                StartTime = medicalExamination.StartTime,
                EndTime = medicalExamination.EndTime,
                Diagnosis = medicalExamination.Diagnosis,
                Recommendation = medicalExamination.Recommendation,
                SelectedDoctor = medicalExamination.DoctorID.ToString(),
                Doctors = doctors.Select(r => new SelectListItem
                {
                    Value = r.ID.ToString(),
                    Text = $"{r.Name} {r.Surname}",
                    Selected = r.ID == medicalExamination.DoctorID
                }).ToList()
            };*/

            return View(editMedicalExaminationDto);
        }

        // GET: MedicalExaminations/Delete/5
        [AuthenticateAuthorize("Patient")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalExamination = await _context.MedicalExaminations.FindAsync(id);
            if (medicalExamination == null)
            {
                return NotFound();
            }

            var loggedInPatient = await _context.Patients.FirstOrDefaultAsync(u => u.ID == HttpContext.Session.GetInt32("UserID"));
            bool isSameAsLoggedInPatient = loggedInPatient.ID != medicalExamination.Patient.ID;
            if (!isSameAsLoggedInPatient)
            {
                return NotFound();
            }

            return View(medicalExamination);
        }

        // POST: MedicalExaminations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Patient")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loggedInPatient = await _context.Patients.FirstOrDefaultAsync(u => u.ID == HttpContext.Session.GetInt32("UserID"));
            var medicalExamination = await _context.MedicalExaminations.FindAsync(id);
            bool isSameAsLoggedInPatient = loggedInPatient.ID != medicalExamination.Patient.ID;
            if (!isSameAsLoggedInPatient)
            {
                return NotFound();
            }

            if (medicalExamination != null)
            {
                _context.MedicalExaminations.Remove(medicalExamination);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalExaminationExists(int id)
        {
            return _context.MedicalExaminations.Any(e => e.ID == id);
        }

        private bool IsAuthorizedToViewMedicalExamination(MedicalExamination medicalExamination)
        {
            int roleID = _loggedInUser.Role.ID;

            // making sure that only the assigned doctor for the medical examination of the patient can view its details
            if (roleID == RoleConstants.DoctorRoleId)
            {
                if (_loggedInUser.ID != medicalExamination.Doctor.ID)
                {
                    return false;
                }
            }
            // making sure that only the correct patient can view the information for the medical examination
            else if (roleID == RoleConstants.PatientRoleId)
            {
                if (_loggedInUser.ID != medicalExamination.Patient.ID)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> IsMedicalExaminationConflictingForDoctorAsync(int doctorId, DateTime startTime, DateTime endTime)
        {
            return await _context.MedicalExaminations.AnyAsync(x =>
                    x.DoctorID == doctorId &&
                    startTime < x.EndTime &&
                    endTime > x.StartTime);
        }
    }
}
