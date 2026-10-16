using System.ComponentModel.DataAnnotations;

namespace Employee.api.DTOs
{
    public class LoginRequest
    {
        [Required, MaxLength(50), EmailAddress]
        public string email { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string contactNo { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string token { get; set; } = string.Empty;
        public DateTime expiresAtUtc { get; set; }
        public int employeeId { get; set; }
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public int designationId { get; set; }
        public string role { get; set; } = string.Empty;
    }
}
