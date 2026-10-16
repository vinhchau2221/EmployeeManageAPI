using Employee.api.DTOs;
using Employee.api.Models;
using Employee.api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EmployeeDbContext _db;
        private readonly TokenService _tokens;

        public AuthController(EmployeeDbContext db, TokenService tokens)
        {
            _db = db;
            _tokens = tokens;
        }

        // POST api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest req, CancellationToken ct)
        {
            var email = req.email.Trim();
            var contactNo = req.contactNo.Trim();

            var employee = await _db.Employees.AsNoTracking()
                .FirstOrDefaultAsync(e => e.email == email && e.contactNo == contactNo, ct);

            // Same message for "email not found" and "wrong contact number" to avoid revealing which emails exist
            if (employee is null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Login failed",
                    Detail = "Invalid email or contact number."
                });
            }

            var (token, expiresAtUtc) = _tokens.CreateToken(employee);

            return new LoginResponse
            {
                token = token,
                expiresAtUtc = expiresAtUtc,
                employeeId = employee.employeeId,
                name = employee.name,
                email = employee.email,
                designationId = employee.designationId,
                role = employee.role,
            };
        }

        // GET api/auth/me  (example of a protected endpoint)
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<EmployeeModel>> Me(CancellationToken ct)
        {
            var id = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var employee = await _db.Employees.AsNoTracking()
                .FirstOrDefaultAsync(e => e.employeeId == id, ct);
            return employee is null ? NotFound() : employee;
        }
    }
}
