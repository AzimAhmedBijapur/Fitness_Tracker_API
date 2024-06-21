using System.ComponentModel.DataAnnotations;
using Api.Models;

namespace Api.Dto{
    public class WorkoutReqDto{
        public DateTime DateAndTime { get; set;}
        public int Duration { get; set;}
        public List<Exercises>? Exercises { get; set;}

    }
}