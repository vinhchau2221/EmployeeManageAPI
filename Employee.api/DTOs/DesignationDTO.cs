using System.ComponentModel.DataAnnotations;

namespace Employee.api.DTOs
{
    public class DesignationRequest
    {
        [Range(1, int.MaxValue)]
        public int departmentId { get; set; }

        [Required, StringLength(200)]
        public string designationName { get; set; } = string.Empty;
    }

    public class DesignationQuery
    {
        public const int PageSize = 10;               // fixed: 10 records per page

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
        public string SortBy { get; set; } = "designationName"; // designationId | departmentId | designationName
        public string SortDir { get; set; } = "asc";            // asc | desc
        public string? Search { get; set; }                     // optional filter on name
    }

    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
