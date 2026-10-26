using Employee.api.DTOs;
using Employee.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentMasterController : ControllerBase
    {
        private readonly EmployeeDbContext _db;

        public DepartmentMasterController(EmployeeDbContext db)
        {
            _db = db;
        }

        // GET api/departments
        [HttpGet]
        public async Task<ActionResult<List<Department>>> GetAll(CancellationToken ct) =>
            await _db.Departments.AsNoTracking().ToListAsync(ct);

        // GET api/departments/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Department>> GetById(int id, CancellationToken ct)
        {
            var dept = await _db.Departments.AsNoTracking()
                .FirstOrDefaultAsync(d => d.departmentId == id, ct);
            return dept is null ? NotFound() : dept;
        }

        // POST api/departments
        [HttpPost]
        public async Task<ActionResult<Department>> Create(DepartmentRequest req, CancellationToken ct)
        {
            if (await _db.Departments.AnyAsync(e => EF.Functions.Collate(e.departmentName, "SQL_Latin1_General_CP1_CI_AS") == req.departmentName, ct))
                return Conflict(DepartmentExists(req.departmentName.Trim()));

            var dept = new Department
            {
                departmentName = req.departmentName.Trim(),
                isActive = req.isActive
            };

            _db.Departments.Add(dept);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetById), new { id = dept.departmentId }, dept);
        }

        // PUT api/departments/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, DepartmentRequest req, CancellationToken ct)
        {
            var dept = await _db.Departments.FindAsync([id], ct);
            if (dept is null) return NotFound();

            if (await _db.Departments.AnyAsync(e => EF.Functions.Collate(e.departmentName, "SQL_Latin1_General_CP1_CI_AS") == req.departmentName && e.departmentId != id, ct))
                return Conflict(DepartmentExists(req.departmentName.Trim()));

            dept.departmentName = req.departmentName.Trim();
            dept.isActive = req.isActive;

            await _db.SaveChangesAsync(ct);
            return NoContent();
        }

        // DELETE api/departments/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _db.Departments
                .Where(d => d.departmentId == id)
                .ExecuteDeleteAsync(ct);

            return deleted == 0 ? NotFound() : NoContent();
        }

        private static ProblemDetails DepartmentExists(string name) => new()
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Duplicate department name",
            Detail = $"A department with name '{name}' already exists."
        };
    }
}
