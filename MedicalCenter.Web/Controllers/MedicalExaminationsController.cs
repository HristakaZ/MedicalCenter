using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalCenter.Web.Models;
using MedicalCenter.Web.Dtos.MedicalExamination;
using MedicalCenter.Web.Attributes;

namespace MedicalCenter.Web.Controllers
{
    public class MedicalExaminationsController : Controller
    {
        private readonly MedicalCenterDbContext _context;
        private readonly bool isAdministrator;
        private readonly User loggedInUser;

        public MedicalExaminationsController(MedicalCenterDbContext context)
        {
            _context = context;
            isAdministrator = HttpContext.Session.GetString("UserRole") == "Administrator";
            loggedInUser = _context.Users.FirstOrDefault(u => u.ID == HttpContext.Session.GetInt32("UserID"));
        }

        // GET: MedicalExaminations
        [AuthenticateAuthorize("Administrator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.MedicalExaminations.ToListAsync());
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

            if (!IsAuthorizedToViewMedicalExamination(medicalExamination) && !isAdministrator)
            {
                return NotFound();
            }

            return View(medicalExamination);
        }

        // GET: MedicalExaminations/Create
        [AuthenticateAuthorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: MedicalExaminations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize]
        public async Task<IActionResult> Create(AddMedicalExaminationDto addMedicalExaminationDto)
        {
            if (ModelState.IsValid)
            {
                MedicalExamination medicalExamination = new MedicalExamination
                {
                    Diagnosis = addMedicalExaminationDto.Diagnosis,
                    Recommendation = addMedicalExaminationDto.Recommendation,
                    Patient = await _context.Patients.FindAsync(addMedicalExaminationDto.PatientId),
                    Doctor = await _context.Doctors.FindAsync(addMedicalExaminationDto.DoctorId)
                };
                _context.Add(medicalExamination);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(addMedicalExaminationDto);
        }

        // GET: MedicalExaminations/Edit/5
        [AuthenticateAuthorize("Doctor")]
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

            var loggedInDoctor = await _context.Doctors.FirstOrDefaultAsync(u => u.ID == HttpContext.Session.GetInt32("UserID"));
            bool isSameAsLoggedInDoctor = loggedInDoctor.ID != medicalExamination.Doctor.ID;

            if (!isSameAsLoggedInDoctor)
            {
                return NotFound();
            }

            return View(medicalExamination);
        }

        // POST: MedicalExaminations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Doctor")]
        public async Task<IActionResult> Edit(int id, EditMedicalExaminationDto editMedicalExaminationDto)
        {
            var medicalExamination = await _context.MedicalExaminations.FindAsync(id);
            if (medicalExamination == null)
            {
                return NotFound();
            }

            var loggedInDoctor = await _context.Doctors.FirstOrDefaultAsync(u => u.ID == HttpContext.Session.GetInt32("UserID"));
            bool isSameAsLoggedInDoctor = loggedInDoctor.ID != medicalExamination.Doctor.ID;
            if (!isSameAsLoggedInDoctor)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    medicalExamination.Diagnosis = editMedicalExaminationDto.Diagnosis;
                    medicalExamination.Recommendation = editMedicalExaminationDto.Recommendation;
                    medicalExamination.Patient = await _context.Patients.FindAsync(editMedicalExaminationDto.PatientId);
                    medicalExamination.Doctor = await _context.Doctors.FindAsync(editMedicalExaminationDto.DoctorId);
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
            string role = HttpContext.Session.GetString("UserRole");

            // making sure that only the assigned doctor for the medical examination of the patient can view its details
            if (role == "Doctor")
            {
                if (loggedInUser.ID != medicalExamination.Doctor.ID)
                {
                    return false;
                }
            }
            // making sure that only the correct patient can view the information for the medical examination
            else if (role == "Patient")
            {
                if (loggedInUser.ID != medicalExamination.Patient.ID)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
