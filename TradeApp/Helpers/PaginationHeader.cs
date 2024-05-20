namespace TradeApp.Helpers
{
    public class PaginationHeader
    {
        public PaginationHeader(int totalItems, int totalPages, int currentPage, int pageSize)
        {
            TotalItems = totalItems;
            TotalPages = totalPages;
            CurrentPage = currentPage;
            PageSize = pageSize;
        }

        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
