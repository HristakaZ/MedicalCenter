using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalCenter.Web.Models;
using MedicalCenter.Web.Attributes;

namespace MedicalCenter.Web.Controllers
{
    public class SpecialtiesController : Controller
    {
        private readonly MedicalCenterDbContext _context;

        public SpecialtiesController(MedicalCenterDbContext context)
        {
            _context = context;
        }

        // GET: Specialties
        [AuthenticateAuthorize("Administrator, Doctor")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Specialties.ToListAsync());
        }

        // GET: Specialties/Details/5
        [AuthenticateAuthorize("Administrator, Doctor")]
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

            return View(specialty);
        }

        // GET: Specialties/Create
        [AuthenticateAuthorize("Administrator, Doctor")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Administrator, Doctor")]
        public async Task<IActionResult> Create([Bind("Description,ID")] Specialty specialty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specialty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specialty);
        }

        // GET: Specialties/Edit/5
        [AuthenticateAuthorize("Administrator, Doctor")]
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
            return View(specialty);
        }

        // POST: Specialties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Administrator, Doctor")]
        public async Task<IActionResult> Edit(int id, [Bind("Description,ID")] Specialty specialty)
        {
            if (id != specialty.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialtyExists(specialty.ID))
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
            return View(specialty);
        }

        // GET: Specialties/Delete/5
        [AuthenticateAuthorize("Administrator, Doctor")]
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

            return View(specialty);
        }

        // POST: Specialties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Administrator, Doctor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty != null)
            {
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
