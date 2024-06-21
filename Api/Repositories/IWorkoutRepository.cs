using Api.Dto;
using Api.Helpers;
using Api.Models;

namespace Api.Repositories{
    public interface IWorkoutRepository{
        public Task<List<Workouts>?> GetWorkouts(string userid, QueryObject query);
        public Task<Workouts?> GetWorkouts(string userid,int id);
        public Task<Workouts?> PutWorkouts(string userid, int id, WorkoutReqDto workoutDto);
        public Task<Workouts?> PostWorkouts(string userid,WorkoutReqDto workoutDto);
        public Task<Workouts?> DeleteWorkouts(string userid, int id);
        public bool WorkoutsExists(int id);

    }
}