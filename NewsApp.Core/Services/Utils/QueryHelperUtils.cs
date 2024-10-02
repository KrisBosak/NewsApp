namespace NewsApp.Core.Services.Utils
{
    public class QueryParameters
    {
        public string FilterBy { get; set; } = string.Empty;
        public string FilterValue {  get; set; } = string.Empty;
        public string OrderBy { get; set; } = string.Empty;
        public bool OrderByDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 2;
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
