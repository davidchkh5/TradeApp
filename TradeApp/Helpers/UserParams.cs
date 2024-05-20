namespace TradeApp.Helpers
{
    public class UserParams // Parameters that user sends
    {
        private const int MaxPageSize = 50; //Maximum of page size
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
