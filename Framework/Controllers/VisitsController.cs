using System.Threading.Tasks;
using HighLoad.Application.Data;
using HighLoad.Application.Entities;
using HighLoad.Framework.Filters;
using Microsoft.AspNetCore.Mvc;

namespace HighLoad.Framework.Controllers
{
    [Route("[controller]")]
    public class VisitsController : Controller
    {
        private static readonly object _emptyResult = new object();
        private readonly IVisitsRepository _visitsRepository;

        public VisitsController(IVisitsRepository visitsRepository)
        {
            _visitsRepository = visitsRepository;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var visit = _visitsRepository.Find(id);

            if (visit == null)return NotFound();

            return Ok(visit);
        }

        [HttpPost("{id}")]
        [RestrictInvalidValuesFilter]
        public async Task<IActionResult> Post(int id, [FromBody] Visit visit)
        {
            if(!ModelState.IsValid) return BadRequest();

            var result = await _visitsRepository.UpdateAsync(id, visit);
            if (result.IsNotFound) return NotFound();

            return Ok(_emptyResult);
        }

        [HttpPost("new")]
        [RestrictInvalidValuesFilter]
        public async Task<IActionResult> Post([FromBody] Visit visit)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _visitsRepository.CreateAsync(visit);
            if (result.IsFailure) return BadRequest();
            return Ok(_emptyResult);
        }
    }
}
