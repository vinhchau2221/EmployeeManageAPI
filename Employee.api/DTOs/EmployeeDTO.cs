using System.ComponentModel.DataAnnotations;

namespace Employee.api.DTOs
{
    public class EmployeeRequest
    {
        [Required, MaxLength(50)]
        public string name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string contactNo { get; set; } = string.Empty;

        [Required, MaxLength(50), EmailAddress]
        public string email { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string city { get; set; } = string.Empty;

        [Required, MaxLength(6)]
        public string pincode { get; set; } = string.Empty;

        [MaxLength(50)]
        public string altContactNo { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string address { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int designationId { get; set; }
    }

    public class EmployeeQuery
    {
        public const int PageSize = 10;               // fixed: 10 records per page

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
        public string SortBy { get; set; } = "name";  // employeeId | name | email | city | designationId | createdDate
        public string SortDir { get; set; } = "asc";  // asc | desc
        public string? Search { get; set; }           // matches name, email, city, contactNo
    }
}
