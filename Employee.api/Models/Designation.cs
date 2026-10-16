using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.api.Models
{
    [Table("designationTbl")]
    public class Designation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int designationId { get; set; }
        public int departmentId { get; set; }
        public string designationName { get; set; } = string.Empty;
    }
}
