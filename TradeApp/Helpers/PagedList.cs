using Microsoft.EntityFrameworkCore;

namespace TradeApp.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            AddRange(items);

        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public async static Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            
             var item = await source.Skip((pageNumber -1) * pageSize).Take(pageSize).ToListAsync(); // skip is like that because , for example if we are on first page we don't want to skip anything
           var returnThing = new PagedList<T>(item, count, pageNumber, pageSize);

            return returnThing;
        }
    }
}
