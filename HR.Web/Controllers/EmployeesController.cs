using System.Linq;
using System.Threading.Tasks;
using HR.Infrastructure.Data;
using HR.Domain.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HR.Web.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public EmployeesController(ApplicationDbContext db) => _db = db;

        private void LoadLookups()
        {
            ViewBag.Departments = new SelectList(_db.Departments.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.Designations = new SelectList(_db.Designations.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.Categories = new SelectList(_db.EmployeeCategories.OrderBy(x => x.Name), "Id", "Name");
        }

        // GET: /Employees
        public async Task<IActionResult> Index(string? q, int? departmentId, int? categoryId)
        {
            var query = _db.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.EmployeeCategory)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(e =>
                      e.FullName.Contains(q)
                   || (e.NIC ?? "").Contains(q)
                   || (e.Email ?? "").Contains(q)
                );

            if (departmentId.HasValue)
                query = query.Where(e => e.DepartmentId == departmentId.Value);

            if (categoryId.HasValue)
                query = query.Where(e => e.EmployeeCategoryId == categoryId.Value);

            ViewBag.Query = q;
            ViewBag.Departments = new SelectList(_db.Departments.OrderBy(x => x.Name), "Id", "Name", departmentId);
            ViewBag.Categories = new SelectList(_db.EmployeeCategories.OrderBy(x => x.Name), "Id", "Name", categoryId);

            var data = await query.OrderBy(e => e.FullName).Take(200).ToListAsync();
            return View(data);
        }

        // GET: /Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var emp = await _db.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.EmployeeCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        // GET: /Employees/Create
        public IActionResult Create()
        {
            LoadLookups();
            return View(new Employee());
        }

        // POST: /Employees/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee model)
        {
            if (!ModelState.IsValid) { LoadLookups(); return View(model); }

            // optional uniqueness (uncomment as policy)
            // if (!string.IsNullOrWhiteSpace(model.NIC) &&
            //     await _db.Employees.AnyAsync(x => x.NIC == model.NIC))
            // {
            //     ModelState.AddModelError(nameof(model.NIC), "NIC already used.");
            //     LoadLookups(); return View(model);
            // }

            _db.Employees.Add(model);
            await _db.SaveChangesAsync();

            // simple history
            _db.EmployeeHistories.Add(new EmployeeHistory
            {
                EmployeeId = model.Id,
                Action = "Created",
                ByUser = User?.Identity?.Name
            });
            await _db.SaveChangesAsync();

            TempData["OK"] = "Employee created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null) return NotFound();
            LoadLookups();
            return View(emp);
        }

        // POST: /Employees/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) { LoadLookups(); return View(model); }

            _db.Update(model);
            await _db.SaveChangesAsync();

            _db.EmployeeHistories.Add(new EmployeeHistory
            {
                EmployeeId = model.Id,
                Action = "Updated",
                ByUser = User?.Identity?.Name
            });
            await _db.SaveChangesAsync();

            TempData["OK"] = "Employee updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var emp = await _db.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.EmployeeCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        // POST: /Employees/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emp = await _db.Employees.FindAsync(id);
            if (emp == null) return NotFound();

            _db.Employees.Remove(emp);
            await _db.SaveChangesAsync();

            _db.EmployeeHistories.Add(new EmployeeHistory
            {
                EmployeeId = id,
                Action = "Deleted",
                ByUser = User?.Identity?.Name
            });
            await _db.SaveChangesAsync();

            TempData["OK"] = "Employee deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
