namespace SurveyBasket.API.Abstraction
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; }

        public int PageNumber { get; set; } 

        public int TotalPage { get; set; }

        public bool HasPrevious => PageNumber > 1;

        public bool HasNext => PageNumber < TotalPage;

        public PaginatedList()
        {
            
        }

        public PaginatedList(List<T> items,int pageNumber,int count,int pageSize)
        {
            Items= items;

            PageNumber = pageNumber;

            TotalPage=(int) Math.Ceiling(count/(double) pageSize);
            
        }

    }
}
