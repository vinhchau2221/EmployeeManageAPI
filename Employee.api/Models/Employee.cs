using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.api.Models
{
    [Table("employeeTbl")]
    public class EmployeeModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int employeeId { get; set; }
        [Required, MaxLength(50)]
        public string name { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        [Phone]
        public string contactNo { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        [EmailAddress]
        public string email { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string city { get; set; } = string.Empty;
        [Required, MaxLength(6)]
        public string pincode { get; set; } = string.Empty;
        public string altContactNo { get; set; } = string.Empty;
        [Required, MaxLength(2000)]
        public string address { get; set; } = string.Empty;
        public int designationId { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        public string role { get; set; }
    }
}
