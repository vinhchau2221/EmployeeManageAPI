using Employee.api.DTOs;
using Employee.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationMasterController : ControllerBase
    {
        private readonly EmployeeDbContext _db;
        public DesignationMasterController(EmployeeDbContext db) => _db = db;

        // GET api/designations?page=1&sortBy=designationName&sortDir=asc&search=dev
        [HttpGet]
        public async Task<ActionResult<PagedResult<Designation>>> GetAll(
            [FromQuery] DesignationQuery q, CancellationToken ct)
        {
            IQueryable<Designation> query = _db.Designations.AsNoTracking();

            if (q.DepartmentID.HasValue && q.DepartmentID.Value > 0) { 
                query= query.Where(w => w.departmentId == q.DepartmentID.Value);
            }

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.Trim();
                query = query.Where(d => d.designationName.Contains(s));
            }

            bool desc = string.Equals(q.SortDir, "desc", StringComparison.OrdinalIgnoreCase);

            // Whitelisted sort columns; designationId as tie-breaker keeps paging stable
            query = (q.SortBy?.ToLowerInvariant()) switch
            {
                "designationid" => desc ? query.OrderByDescending(d => d.designationId)
                                        : query.OrderBy(d => d.designationId),
                "departmentid" => desc ? query.OrderByDescending(d => d.departmentId).ThenBy(d => d.designationId)
                                        : query.OrderBy(d => d.departmentId).ThenBy(d => d.designationId),
                _ => desc ? query.OrderByDescending(d => d.designationName).ThenBy(d => d.designationId)
                                        : query.OrderBy(d => d.designationName).ThenBy(d => d.designationId),
            };

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((q.Page - 1) * DesignationQuery.PageSize)
                .Take(DesignationQuery.PageSize)
                .ToListAsync(ct);

            return new PagedResult<Designation>
            {
                Items = items,
                Page = q.Page,
                PageSize = DesignationQuery.PageSize,
                TotalCount = total
            };
        }

        // GET api/designations/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Designation>> GetById(int id, CancellationToken ct)
        {
            var entity = await _db.Designations.AsNoTracking()
                .FirstOrDefaultAsync(d => d.designationId == id, ct);
            return entity is null ? NotFound() : entity;
        }

        // POST api/designations
        [HttpPost]
        public async Task<ActionResult<Designation>> Create(DesignationRequest req, CancellationToken ct)
        {
            var name = req.designationName.Trim();

            if (await _db.Designations.AnyAsync(d => d.designationName == name, ct))
                return Conflict(NameExists(name));

            var entity = new Designation { departmentId = req.departmentId, designationName = name };
            _db.Designations.Add(entity);

            try { await _db.SaveChangesAsync(ct); }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Conflict(NameExists(name)); // race condition: caught by the unique index
            }

            return CreatedAtAction(nameof(GetById), new { id = entity.designationId }, entity);
        }

        // PUT api/designations/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, DesignationRequest req, CancellationToken ct)
        {
            var entity = await _db.Designations.FindAsync(new object[] { id }, ct);
            if (entity is null) return NotFound();

            var name = req.designationName.Trim();

            if (await _db.Designations.AnyAsync(d => d.designationName == name && d.designationId != id, ct))
                return Conflict(NameExists(name));

            entity.departmentId = req.departmentId;
            entity.designationName = name;

            try { await _db.SaveChangesAsync(ct); }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Conflict(NameExists(name));
            }

            return NoContent();
        }

        // DELETE api/designations/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _db.Designations
                .Where(d => d.designationId == id)
                .ExecuteDeleteAsync(ct);
            return deleted == 0 ? NotFound() : NoContent();
        }

        private static ProblemDetails NameExists(string name) => new()
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Duplicate designation name",
            Detail = $"Designation '{name}' already exists."
        };

        // 2601 = duplicate unique index, 2627 = duplicate unique constraint (SQL Server)
        private static bool IsUniqueViolation(DbUpdateException ex) =>
            ex.InnerException is SqlException { Number: 2601 or 2627 };
    }
}
