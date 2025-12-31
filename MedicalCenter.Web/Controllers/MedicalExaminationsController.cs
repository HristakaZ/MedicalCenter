using MedicalCenter.Web.Attributes;
using MedicalCenter.Web.Constants;
using MedicalCenter.Web.Dtos.MedicalExamination;
using MedicalCenter.Web.Models;
using MedicalCenter.Web.ViewModels.MedicalExamination;
using Microsoft.AspNetCore.Mvc;
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

        [AuthenticateAuthorize("Administrator")]
        public async Task<IActionResult> Index(string doctorName,
                                               string patientName,
                                               DateTime? startDate,
                                               DateTime? endDate,
                                               int pageNumber = 1,
                                               int pageSize = 10)
        {
            var query = _context.MedicalExaminations
                .AsQueryable();

            // Doctor name filter
            if (!string.IsNullOrWhiteSpace(doctorName))
            {
                string dn = doctorName.Trim().ToLower();
                query = query.Where(m =>
                    (m.Doctor.Name + " " + m.Doctor.Surname).ToLower().Contains(dn));
            }

            // Patient name filter
            if (!string.IsNullOrWhiteSpace(patientName))
            {
                string pn = patientName.Trim().ToLower();
                query = query.Where(m =>
                    (m.Patient.Name + " " + m.Patient.Surname).ToLower().Contains(pn));
            }

            // BETWEEN date range filtering
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(m =>
                    m.StartTime >= startDate.Value &&
                    m.StartTime <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(m => m.StartTime >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                query = query.Where(m => m.StartTime <= endDate.Value);
            }

            // Pagination
            int totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(m => m.StartTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = items.Select(m => new GetMedicalExaminationViewModel
            {
                ID = m.ID,
                Diagnosis = m.Diagnosis,
                Recommendation = m.Recommendation,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                PatientName = $"{m.Patient.Name} {m.Patient.Surname}",
                DoctorName = $"{m.Doctor.Name} {m.Doctor.Surname}",
                DoctorSpecialty = m.Doctor.Specialty.Description
            }).ToList();

            // Pass filter values back to view
            ViewBag.DoctorName = doctorName;
            ViewBag.PatientName = patientName;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd HH:mm");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd HH:mm");

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(dtos);
        }

        [AuthenticateAuthorize("Doctor", "Patient")]
        public async Task<IActionResult> MyExaminations(DateTime? startDate,
                                                        DateTime? endDate,
                                                        int pageNumber = 1,
                                                        int pageSize = 10)
        {
            string role = _loggedInUser.Role.Description;
            int userId = _loggedInUser.ID;

            var query = _context.MedicalExaminations
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                    .ThenInclude(d => d.Specialty)
                .AsQueryable();

            // Filter by logged-in user
            if (role == "Doctor")
            {
                query = query.Where(me => me.Doctor.ID == userId);
            }
            else if (role == "Patient")
            {
                query = query.Where(me => me.Patient.ID == userId);
            }

            // Correct BETWEEN date range filtering
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(m =>
                    m.StartTime >= startDate.Value &&
                    m.StartTime <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(m => m.StartTime >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                query = query.Where(m => m.StartTime <= endDate.Value);
            }

            // Pagination
            int totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(m => m.StartTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map to DTO
            var dtos = items.Select(m => new GetMedicalExaminationViewModel
            {
                ID = m.ID,
                Diagnosis = m.Diagnosis,
                Recommendation = m.Recommendation,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                PatientName = $"{m.Patient.Name} {m.Patient.Surname}",
                DoctorName = $"{m.Doctor.Name} {m.Doctor.Surname}",
                DoctorSpecialty = m.Doctor.Specialty.Description
            }).ToList();

            // Pass filter values back to view
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd HH:mm");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd HH:mm");

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(dtos);
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

            GetMedicalExaminationViewModel getMedicalExaminationViewModel = new GetMedicalExaminationViewModel()
            {
                ID = medicalExamination.ID,
                Diagnosis = medicalExamination.Diagnosis,
                Recommendation = medicalExamination.Recommendation,
                StartTime = medicalExamination.StartTime,
                EndTime = medicalExamination.EndTime,
                PatientName = $"{medicalExamination.Patient.Name} {medicalExamination.Patient.Surname}",
                DoctorName = $"{medicalExamination.Doctor.Name} {medicalExamination.Doctor.Surname}",
                DoctorSpecialty = $"{medicalExamination.Doctor.Specialty.Description}"
            };

            return View(getMedicalExaminationViewModel);
        }

        // GET: MedicalExaminations/Create
        [AuthenticateAuthorize("Patient")]
        public async Task<IActionResult> Create()
        {
            Patient patient = await _context.Patients.FindAsync(_loggedInUser.ID);
            Doctor doctor = await _context.Doctors.FindAsync(patient.DoctorID);
            GetMedicalExaminationViewModel medicalExaminationDto = new GetMedicalExaminationViewModel()
            {
                DoctorName = $"{doctor.Name} {doctor.Surname}",
                PatientName = $"{patient.Name} {patient.Surname}",
                DoctorSpecialty = $"{doctor.Specialty.Description}",
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
            Patient patient = await _context.Patients.FindAsync(_loggedInUser.ID);
            Doctor doctor = await _context.Doctors.FindAsync(patient.DoctorID);
            GetMedicalExaminationViewModel getMedicalExaminationViewModel = new GetMedicalExaminationViewModel
            {
                DoctorName = $"{doctor.Name} {doctor.Surname}",
                PatientName = $"{patient.Name} {patient.Surname}",
                DoctorSpecialty = $"{doctor.Specialty.Description}",
                StartTime = addMedicalExaminationDto.StartTime,
                EndTime = addMedicalExaminationDto.EndTime
            };
            if (ModelState.IsValid)
            {
                if (patient == null || doctor == null)
                {
                    return NotFound();
                }

                if (addMedicalExaminationDto.StartTime >= addMedicalExaminationDto.EndTime)
                {
                    ModelState.AddModelError(nameof(addMedicalExaminationDto.StartTime), "End time must be later than start time.");
                    ModelState.AddModelError(nameof(addMedicalExaminationDto.EndTime), "End time must be later than start time.");
                    return View(getMedicalExaminationViewModel);
                }

                if (await IsMedicalExaminationConflictingForDoctorAsync(doctor.ID, addMedicalExaminationDto.StartTime, addMedicalExaminationDto.EndTime))
                {
                    ModelState.AddModelError(nameof(addMedicalExaminationDto.StartTime), "The doctor already has a medical examination scheduled during this time.");
                    ModelState.AddModelError(nameof(addMedicalExaminationDto.EndTime), "The doctor already has a medical examination scheduled during this time.");
                    return View(getMedicalExaminationViewModel);
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

            return View(getMedicalExaminationViewModel);
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

            GetMedicalExaminationViewModel getMedicalExaminationViewModel = new GetMedicalExaminationViewModel()
            {
                ID = medicalExamination.ID,
                DoctorName = $"{medicalExamination.Doctor.Name} {medicalExamination.Doctor.Surname}",
                PatientName = $"{medicalExamination.Patient.Name} {medicalExamination.Patient.Surname}",
                DoctorSpecialty = $"{medicalExamination.Doctor.Specialty.Description}",
                StartTime = medicalExamination.StartTime,
                EndTime = medicalExamination.EndTime,
                Recommendation = medicalExamination.Recommendation,
                Diagnosis = medicalExamination.Diagnosis,
            };

            return View(getMedicalExaminationViewModel);
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

            GetMedicalExaminationViewModel getMedicalExaminationViewModel = new GetMedicalExaminationViewModel()
            {
                ID = medicalExamination.ID,
                DoctorName = $"{medicalExamination.Doctor.Name} {medicalExamination.Doctor.Surname}",
                PatientName = $"{medicalExamination.Patient.Name} {medicalExamination.Patient.Surname}",
                DoctorSpecialty = $"{medicalExamination.Doctor.Specialty.Description}",
                StartTime = medicalExamination.StartTime,
                EndTime = medicalExamination.EndTime,
                Recommendation = medicalExamination.Recommendation,
                Diagnosis = medicalExamination.Diagnosis,
            };

            if (!IsAuthorizedToViewMedicalExamination(medicalExamination) && !_isAdministrator)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (editMedicalExaminationDto.StartTime != medicalExamination.StartTime || editMedicalExaminationDto.EndTime != medicalExamination.EndTime)
                    {
                        if (editMedicalExaminationDto.StartTime >= editMedicalExaminationDto.EndTime)
                        {
                            ModelState.AddModelError(nameof(editMedicalExaminationDto.StartTime), "End time must be later than start time.");
                            ModelState.AddModelError(nameof(editMedicalExaminationDto.EndTime), "End time must be later than start time.");
                            return View(getMedicalExaminationViewModel);
                        }

                        if (await IsMedicalExaminationConflictingForDoctorAsync(medicalExamination.DoctorID, editMedicalExaminationDto.StartTime, editMedicalExaminationDto.EndTime))
                        {
                            ModelState.AddModelError(nameof(editMedicalExaminationDto.StartTime), "The doctor already has a medical examination scheduled during this time.");
                            ModelState.AddModelError(nameof(editMedicalExaminationDto.EndTime), "The doctor already has a medical examination scheduled during this time.");
                            return View(getMedicalExaminationViewModel);
                        }
                    }

                    bool isLoggedInUserDoctor = _loggedInUser.RoleID == RoleConstants.DoctorRoleId;
                    if (isLoggedInUserDoctor)
                    {
                        medicalExamination.Diagnosis = editMedicalExaminationDto.Diagnosis;
                        medicalExamination.Recommendation = editMedicalExaminationDto.Recommendation;
                    }
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
                return RedirectToAction(nameof(MyExaminations));
            }

            return View(getMedicalExaminationViewModel);
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
            bool isSameAsLoggedInPatient = loggedInPatient.ID == medicalExamination.Patient.ID;
            if (!isSameAsLoggedInPatient)
            {
                return NotFound();
            }

            GetMedicalExaminationViewModel getMedicalExaminationViewModel = new GetMedicalExaminationViewModel()
            {
                ID = medicalExamination.ID,
                Diagnosis = medicalExamination.Diagnosis,
                Recommendation = medicalExamination.Recommendation,
                StartTime = medicalExamination.StartTime,
                EndTime = medicalExamination.EndTime,
                PatientName = $"{medicalExamination.Patient.Name} {medicalExamination.Patient.Surname}",
                DoctorName = $"{medicalExamination.Doctor.Name} {medicalExamination.Doctor.Surname}",
                DoctorSpecialty = $"{medicalExamination.Doctor.Specialty.Description}"
            };

            return View(getMedicalExaminationViewModel);
        }

        // POST: MedicalExaminations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Patient")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loggedInPatient = await _context.Patients.FirstOrDefaultAsync(u => u.ID == HttpContext.Session.GetInt32("UserID"));
            var medicalExamination = await _context.MedicalExaminations.FindAsync(id);
            bool isSameAsLoggedInPatient = loggedInPatient.ID == medicalExamination.Patient.ID;
            if (!isSameAsLoggedInPatient)
            {
                return NotFound();
            }

            if (medicalExamination != null)
            {
                _context.MedicalExaminations.Remove(medicalExamination);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyExaminations));
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
