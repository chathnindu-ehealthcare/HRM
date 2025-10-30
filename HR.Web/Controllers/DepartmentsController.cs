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
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DepartmentsController(ApplicationDbContext db) => _db = db;

        // GET: /Departments
        public async Task<IActionResult> Index(string? q)
        {
            var list = _db.Departments.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                list = list.Where(d => d.Name.Contains(q));
            var data = await list.OrderBy(d => d.Name).ToListAsync();
            ViewBag.Query = q;
            return View(data);
        }

        // GET: /Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var dept = await _db.Departments.FirstOrDefaultAsync(m => m.Id == id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        // GET: /Departments/Create
        public IActionResult Create() => View(new Department());

        // POST: /Departments/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department model)
        {
            if (!ModelState.IsValid) return View(model);

            // unique check
            var exists = await _db.Departments.AnyAsync(x => x.Name == model.Name);
            if (exists)
            {
                ModelState.AddModelError(nameof(model.Name), "Department name already exists.");
                return View(model);
            }

            _db.Add(model);
            await _db.SaveChangesAsync();
            TempData["OK"] = "Department created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var dept = await _db.Departments.FindAsync(id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        // POST: /Departments/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            // unique check (excluding self)
            var exists = await _db.Departments.AnyAsync(x => x.Name == model.Name && x.Id != model.Id);
            if (exists)
            {
                ModelState.AddModelError(nameof(model.Name), "Department name already exists.");
                return View(model);
            }

            try
            {
                _db.Update(model);
                await _db.SaveChangesAsync();
                TempData["OK"] = "Department updated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.Departments.AnyAsync(e => e.Id == model.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var dept = await _db.Departments.FirstOrDefaultAsync(m => m.Id == id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        // POST: /Departments/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dept = await _db.Departments.FindAsync(id);
            if (dept == null) return NotFound();

            // if you prefer soft-delete:
            // dept.IsActive = false; _db.Update(dept);
            _db.Departments.Remove(dept);

            await _db.SaveChangesAsync();
            TempData["OK"] = "Department deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
