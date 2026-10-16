using Employee.api.DTOs;
using Employee.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeMasterController : ControllerBase
    {
        private readonly EmployeeDbContext _db;
        public EmployeeMasterController(EmployeeDbContext db) => _db = db;

        // GET api/employees?page=1&sortBy=name&sortDir=asc&search=john
        [HttpGet]
        public async Task<ActionResult<PagedResult<EmployeeModel>>> GetAll(
            [FromQuery] EmployeeQuery q, CancellationToken ct)
        {
            IQueryable<EmployeeModel> query = _db.Employees.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.Trim();
                query = query.Where(e => e.name.Contains(s) || e.email.Contains(s)
                                      || e.city.Contains(s) || e.contactNo.Contains(s));
            }

            bool desc = string.Equals(q.SortDir, "desc", StringComparison.OrdinalIgnoreCase);

            // Whitelisted sort columns; employeeId as tie-breaker keeps paging stable
            query = (q.SortBy?.ToLowerInvariant()) switch
            {
                "employeeid" => desc ? query.OrderByDescending(e => e.employeeId)
                                        : query.OrderBy(e => e.employeeId),
                "email" => desc ? query.OrderByDescending(e => e.email).ThenBy(e => e.employeeId)
                                        : query.OrderBy(e => e.email).ThenBy(e => e.employeeId),
                "city" => desc ? query.OrderByDescending(e => e.city).ThenBy(e => e.employeeId)
                                        : query.OrderBy(e => e.city).ThenBy(e => e.employeeId),
                "designationid" => desc ? query.OrderByDescending(e => e.designationId).ThenBy(e => e.employeeId)
                                        : query.OrderBy(e => e.designationId).ThenBy(e => e.employeeId),
                "createddate" => desc ? query.OrderByDescending(e => e.createdDate).ThenBy(e => e.employeeId)
                                        : query.OrderBy(e => e.createdDate).ThenBy(e => e.employeeId),
                _ => desc ? query.OrderByDescending(e => e.name).ThenBy(e => e.employeeId)
                                        : query.OrderBy(e => e.name).ThenBy(e => e.employeeId),
            };

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((q.Page - 1) * EmployeeQuery.PageSize)
                .Take(EmployeeQuery.PageSize)
                .ToListAsync(ct);

            return new PagedResult<EmployeeModel>
            {
                Items = items,
                Page = q.Page,
                PageSize = EmployeeQuery.PageSize,
                TotalCount = total
            };
        }

        // GET api/employees/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeModel>> GetById(int id, CancellationToken ct)
        {
            var entity = await _db.Employees.AsNoTracking()
                .FirstOrDefaultAsync(e => e.employeeId == id, ct);
            return entity is null ? NotFound() : entity;
        }

        // POST api/employees
        [HttpPost]
        public async Task<ActionResult<EmployeeModel>> Create(EmployeeRequest req, CancellationToken ct)
        {
            var email = req.email.Trim();

            if (!await _db.Designations.AnyAsync(d => d.designationId == req.designationId, ct))
                return BadRequest(DesignationNotFound(req.designationId));

            if (await _db.Employees.AnyAsync(e => e.email == email, ct))
                return Conflict(EmailExists(email));

            var entity = new EmployeeModel { createdDate = DateTime.UtcNow };
            Map(req, entity);
            _db.Employees.Add(entity);

            try { await _db.SaveChangesAsync(ct); }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Conflict(EmailExists(email)); // race condition: caught by the unique index
            }

            return CreatedAtAction(nameof(GetById), new { id = entity.employeeId }, entity);
        }

        // PUT api/employees/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, EmployeeRequest req, CancellationToken ct)
        {
            var entity = await _db.Employees.FindAsync([id], ct);
            if (entity is null) return NotFound();

            var email = req.email.Trim();

            if (!await _db.Designations.AnyAsync(d => d.designationId == req.designationId, ct))
                return BadRequest(DesignationNotFound(req.designationId));

            if (await _db.Employees.AnyAsync(e => e.email == email && e.employeeId != id, ct))
                return Conflict(EmailExists(email));

            Map(req, entity);
            entity.modifiedDate = DateTime.UtcNow;

            try { await _db.SaveChangesAsync(ct); }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Conflict(EmailExists(email));
            }

            return NoContent();
        }

        // DELETE api/employees/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _db.Employees
                .Where(e => e.employeeId == id)
                .ExecuteDeleteAsync(ct);
            return deleted == 0 ? NotFound() : NoContent();
        }

        private static void Map(EmployeeRequest req, EmployeeModel e)
        {
            e.name = req.name.Trim();
            e.contactNo = req.contactNo.Trim();
            e.email = req.email.Trim();
            e.city = req.city.Trim();
            e.pincode = req.pincode.Trim();
            e.altContactNo = req.altContactNo?.Trim() ?? string.Empty;
            e.address = req.address.Trim();
            e.designationId = req.designationId;
        }

        private static ProblemDetails EmailExists(string email) => new()
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Duplicate email",
            Detail = $"An employee with email '{email}' already exists."
        };

        private static ProblemDetails DesignationNotFound(int id) => new()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid designation",
            Detail = $"Designation {id} does not exist."
        };

        // 2601 = duplicate unique index, 2627 = duplicate unique constraint (SQL Server)
        private static bool IsUniqueViolation(DbUpdateException ex) =>
            ex.InnerException is SqlException { Number: 2601 or 2627 };
    }
}
