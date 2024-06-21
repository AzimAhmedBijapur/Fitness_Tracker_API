using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Api.Dto;
using Api.Helpers;
using Api.Repositories;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWorkoutRepository _repository;

        public WorkoutController(ApplicationDbContext context,IWorkoutRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        // GET: api/Workout
        [HttpGet("{userid}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Workouts>>> GetWorkouts(string userid, [FromQuery] QueryObject query)
        {
                var workouts = await _repository.GetWorkouts(userid, query);
                if(workouts== null){
                    return NotFound("Either the user or the workout does not exist!");
                }
                return workouts;
        }

        // GET: api/Workout/5
        [HttpGet("{userid}/{id:int}")]
        [Authorize]
        public async Task<ActionResult<Workouts>> GetWorkouts(string userid,int id)
        {
            var workout = await _repository.GetWorkouts(userid,id);

            if(workout==null){
                return NotFound("Workout not found!");
            }

            return workout;
        }

        // PUT: api/Workout/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{userid}/{id:int}")]
        public async Task<IActionResult> PutWorkouts(string userid, int id, [FromBody] WorkoutReqDto workoutDto)
        {
            if(!ModelState.IsValid){
                    
                    var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Message = "Data Validation failed!", Errors = errors });
            }

            var workout = await _repository.PutWorkouts(userid, id,workoutDto);

            if(workout==null){
                return NotFound("Could not update!");
            }

            return NoContent();
        }

        // POST: api/Workout
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{userid}")]
        [Authorize]
        public async Task<ActionResult<Workouts>> PostWorkouts([FromRoute]string userid,[FromBody] WorkoutReqDto workoutDto)
        {
            if(!ModelState.IsValid){
                    
                    var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Message = "Data Validation failed!", Errors = errors });
            }

            var workout = await  _repository.PostWorkouts(userid, workoutDto);

            if(workout==null){
                return BadRequest(new { Message = "User not found!"});
            }

            return CreatedAtAction(nameof(GetWorkouts), new { userid = workout.AppUserId }, workoutDto);
        }

        // DELETE: api/Workout/5
        [HttpDelete("{userid}/{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteWorkouts(string userid, int id)
        {
            if(!ModelState.IsValid){
                    
                    var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Message = "Could not delete workout", Errors = errors });
            }

            var workout = await  _repository.DeleteWorkouts(userid, id);

            if(workout==null){
                return NotFound(new { Message = "Either workout or user not found!"});
            }

            return NoContent();
        }

        private bool WorkoutsExists(int id)
        {
            return _repository.WorkoutsExists(id);
        }
    }
}
