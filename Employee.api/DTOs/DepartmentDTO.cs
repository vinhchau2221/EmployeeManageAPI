using System.ComponentModel.DataAnnotations;

namespace Employee.api.DTOs
{
    public class DepartmentRequest
    {
        [Required, StringLength(200)]
        public string departmentName { get; set; } = string.Empty;

        public bool isActive { get; set; }
    }
}
