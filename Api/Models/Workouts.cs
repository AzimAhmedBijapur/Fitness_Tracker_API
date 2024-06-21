namespace Api.Models{
    public class Workouts{
        public int Id { get; set;}
        public string? AppUserId { get; set;}
        public DateTime DateAndTime { get; set;}
        public int Duration { get; set;}
        public List<Exercises>? Exercises { get; set;}

    }
}