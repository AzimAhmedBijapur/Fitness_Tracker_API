using Api.Data;
using Api.Dto;
using Api.Helpers;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories{
    public class WorkoutRepository : IWorkoutRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkoutRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Workouts?> DeleteWorkouts(string userid, int id)
        {
            var user = await _context.AppUser.FindAsync(userid);
            if (user == null){
                return null;
            }

            var workout = await _context.Workouts
            .Include(w => w.Exercises) // Ensure exercises are loaded
            .FirstOrDefaultAsync(w => w.Id == id && w.AppUserId == userid);
            
            if (workout == null || workout.AppUserId != userid)
            {
                return null;
            }

            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();

            return workout;
        }

        public async Task<List<Workouts>?> GetWorkouts(string userid, QueryObject query)
        {
            var workouts = _context.Workouts.Where(x => x.AppUserId == userid).Include(x => x.Exercises).AsQueryable();

                var user = await _context.AppUser.FindAsync(userid);

                if(user == null){
                    return null;
                }

                if (query.From.HasValue)
                {
                    workouts = workouts.Where(x => DateOnly.FromDateTime(x.DateAndTime) >= DateOnly.FromDateTime(query.From.Value));
                }

                if (query.To.HasValue)
                {
                    workouts = workouts.Where(x => DateOnly.FromDateTime(x.DateAndTime) <= DateOnly.FromDateTime(query.To.Value));
                }

                if (query.PageSize.HasValue && query.PageCount.HasValue)
                {
                    workouts = workouts
                        .Skip((query.PageCount.Value - 1) * query.PageSize.Value)
                        .Take(query.PageSize.Value);
                }

                if(query.OrderDescending){

                    workouts = workouts.OrderByDescending(x => x.DateAndTime);
                }
            return await workouts.ToListAsync();
        }

        public async Task<Workouts?> GetWorkouts(string userid, int id)
        {
            var workouts = _context.Workouts.Where(x => x.AppUserId == userid).AsQueryable();
            var workout = await workouts.Where(x => x.Id == id).Include(x => x.Exercises).FirstOrDefaultAsync();

            if (workout == null)
            {
                return null;
            }

            return workout;
        }

        public async Task<Workouts?> PostWorkouts(string userid, WorkoutReqDto workoutDto)
        {
            var user = await _context.AppUser.FindAsync(userid);
            if (user == null){
                return null;
            }

            var workout = new Workouts{
                AppUserId = userid,
                Duration = workoutDto.Duration,
                DateAndTime =  workoutDto.DateAndTime,
                Exercises = workoutDto.Exercises,
            };

            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            return workout;
        }

        public async Task<Workouts?> PutWorkouts(string userid, int id, WorkoutReqDto workoutDto)
        {
            var workouts = _context.Workouts.Where(x => x.AppUserId == userid).AsQueryable();
            var workout = await workouts.Where(x => x.Id == id).Include(x => x.Exercises).FirstOrDefaultAsync();

            if (workout == null)
            {
                return null;
            }

            if( workoutDto.Duration !=0){
                workout.Duration = workoutDto.Duration;
            }


            if(workoutDto.Exercises != null){

                if (workout.Exercises == null)
                {
                    workout.Exercises = new List<Exercises>();
                }
                        workout.Exercises.Clear();

                // Add updated exercises
                foreach (var exerciseDto in workoutDto.Exercises)
                {
                    workout.Exercises.Add(new Exercises
                    {
                        Name = exerciseDto.Name,
                        Reps = exerciseDto.Reps,
                        Sets = exerciseDto.Sets,
                    });
                }
            }

            _context.Entry(workout).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return workout;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkoutsExists(id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public bool WorkoutsExists(int id)
        {
            return _context.Workouts.Any(e => e.Id == id);
        }
    }
}