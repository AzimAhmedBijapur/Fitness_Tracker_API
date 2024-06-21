namespace Api.Helpers{
    public class QueryObject{
        public DateTime? From {get; set;} 
        public DateTime? To {get; set;} 
        public int? PageSize {get; set;} = 10;
        public int? PageCount {get; set;} = 1;

        public bool OrderDescending {get; set;} = false;

    }
}