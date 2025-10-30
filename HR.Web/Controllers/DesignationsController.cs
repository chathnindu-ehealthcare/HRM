using System.Linq;
using System.Threading.Tasks;
using HR.Infrastructure.Data;
using HR.Domain.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR.Web.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class DesignationsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DesignationsController(ApplicationDbContext db) => _db = db;

        // GET: /Designations
        public async Task<IActionResult> Index(string? q)
        {
            var list = _db.Designations.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                list = list.Where(d => d.Name.Contains(q));
            var data = await list.OrderBy(d => d.Name).ToListAsync();
            ViewBag.Query = q;
            return View(data);
        }

        // GET: /Designations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var desig = await _db.Designations.FirstOrDefaultAsync(m => m.Id == id);
            if (desig == null) return NotFound();
            return View(desig);
        }

        // GET: /Designations/Create
        public IActionResult Create() => View(new Designation());

        // POST: /Designations/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Designation model)
        {
            if (!ModelState.IsValid) return View(model);

            // unique check
            var exists = await _db.Designations.AnyAsync(x => x.Name == model.Name);
            if (exists)
            {
                ModelState.AddModelError(nameof(model.Name), "Designation name already exists.");
                return View(model);
            }

            _db.Designations.Add(model);
            await _db.SaveChangesAsync();
            TempData["OK"] = "Designation created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Designations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var desig = await _db.Designations.FindAsync(id);
            if (desig == null) return NotFound();
            return View(desig);
        }

        // POST: /Designations/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Designation model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            // unique check (excluding self)
            var exists = await _db.Designations.AnyAsync(x => x.Name == model.Name && x.Id != model.Id);
            if (exists)
            {
                ModelState.AddModelError(nameof(model.Name), "Designation name already exists.");
                return View(model);
            }

            try
            {
                _db.Update(model);
                await _db.SaveChangesAsync();
                TempData["OK"] = "Designation updated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.Designations.AnyAsync(e => e.Id == model.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Designations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var desig = await _db.Designations.FirstOrDefaultAsync(m => m.Id == id);
            if (desig == null) return NotFound();
            return View(desig);
        }

        // POST: /Designations/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var desig = await _db.Designations.FindAsync(id);
            if (desig == null) return NotFound();

            // Prevent deleting if any employees use this designation
            var inUse = await _db.Employees.AnyAsync(e => e.DesignationId == id);
            if (inUse)
            {
                TempData["ERR"] = "Cannot delete this designation because it is assigned to one or more employees.";
                return RedirectToAction(nameof(Index));
            }

            // If you prefer soft-delete, do:
            // desig.IsActive = false; _db.Update(desig);
            // else hard delete:
            _db.Designations.Remove(desig);

            await _db.SaveChangesAsync();
            TempData["OK"] = "Designation deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
