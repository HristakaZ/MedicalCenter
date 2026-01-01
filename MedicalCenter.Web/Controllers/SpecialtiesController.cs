using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalCenter.Web.Models;
using MedicalCenter.Web.Attributes;
using MedicalCenter.Web.ViewModels.Specialty;
using MedicalCenter.Web.Dtos.Specialty;

namespace MedicalCenter.Web.Controllers
{
    public class SpecialtiesController : Controller
    {
        private readonly MedicalCenterDbContext _context;

        public SpecialtiesController(MedicalCenterDbContext context)
        {
            _context = context;
        }

        [AuthenticateAuthorize("Administrator")]
        public async Task<IActionResult> Index(string description,
                                               int pageNumber = 1,
                                               int pageSize = 10)
        {
            var query = _context.Specialties.AsQueryable();

            // Filter by description (partial match)
            if (!string.IsNullOrWhiteSpace(description))
            {
                string d = description.Trim().ToLower();
                query = query.Where(s => s.Description.ToLower().Contains(d));
            }

            // Pagination
            int totalItems = await query.CountAsync();

            var specialties = await query
                .OrderBy(s => s.Description)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map to DTO
            var dtos = specialties.Select(s => new GetSpecialtyViewModel
            {
                ID = s.ID,
                Description = s.Description
            }).ToList();

            // Pass filter values back to view
            ViewBag.Description = description;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(dtos);
        }


        // GET: Specialties/Details/5
        [AuthenticateAuthorize("Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(m => m.ID == id);
            if (specialty == null)
            {
                return NotFound();
            }

            GetSpecialtyViewModel getSpecialtyViewModel = new GetSpecialtyViewModel() { ID = specialty.ID, Description = specialty.Description };

            return View(getSpecialtyViewModel);
        }

        // GET: Specialties/Create
        [AuthenticateAuthorize("Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Administrator")]
        public async Task<IActionResult> Create(AddSpecialtyDto addSpecialtyDto)
        {
            if (ModelState.IsValid)
            {
                Specialty specialty = new Specialty() { Description = addSpecialtyDto.Description };
                _context.Add(specialty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            GetSpecialtyViewModel getSpecialtyViewModel = new GetSpecialtyViewModel() { Description = addSpecialtyDto.Description };
            return View(getSpecialtyViewModel);
        }

        // GET: Specialties/Edit/5
        [AuthenticateAuthorize("Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }

            GetSpecialtyViewModel getSpecialtyViewModel = new GetSpecialtyViewModel() { ID = specialty.ID, Description = specialty.Description };
            return View(getSpecialtyViewModel);
        }

        // POST: Specialties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Administrator")]
        public async Task<IActionResult> Edit(int id, EditSpecialtyDto editSpecialtyDto)
        {
            if (id != editSpecialtyDto.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Specialty specialty = await _context.Specialties.FindAsync(editSpecialtyDto.ID);
                    specialty.Description = editSpecialtyDto.Description;
                    _context.Update(specialty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialtyExists(editSpecialtyDto.ID))
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

            GetSpecialtyViewModel getSpecialtyViewModel = new GetSpecialtyViewModel() { ID = editSpecialtyDto.ID, Description = editSpecialtyDto.Description };
            return View(getSpecialtyViewModel);
        }

        // GET: Specialties/Delete/5
        [AuthenticateAuthorize("Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(m => m.ID == id);
            if (specialty == null)
            {
                return NotFound();
            }

            GetSpecialtyViewModel getSpecialtyViewModel = new GetSpecialtyViewModel() { ID = specialty.ID, Description = specialty.Description };
            return View(getSpecialtyViewModel);
        }

        // POST: Specialties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty != null)
            {
                bool isSpecialtyUsed = await _context.Doctors.AnyAsync(d => d.SpecialtyID == id);
                if (isSpecialtyUsed)
                {
                    ModelState.AddModelError("CannotDeleteSpecialty", "Докторската специалност се използва и не може да бъде изтрита.");
                    GetSpecialtyViewModel getSpecialtyViewModel = new GetSpecialtyViewModel() { ID = specialty.ID, Description = specialty.Description };
                    return View(getSpecialtyViewModel);
                }

                _context.Specialties.Remove(specialty);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialtyExists(int id)
        {
            return _context.Specialties.Any(e => e.ID == id);
        }
    }
}
