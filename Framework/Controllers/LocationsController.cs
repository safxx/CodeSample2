using System;
using System.Linq;
using System.Threading.Tasks;
using HighLoad.Application;
using HighLoad.Application.Data;
using HighLoad.Application.Data.ReadModel.MarkView;
using HighLoad.Application.Entities;
using HighLoad.Framework.Filters;
using HighLoad.Framework.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HighLoad.Framework.Controllers
{
    [Route("[controller]")]
    public class LocationsController : Controller
    {
        private static readonly object _emptyResult = new object();
        private readonly ILocationsRepository _locationsRepository;
        private readonly IMarkViewProvider _markViewProvider;

        public LocationsController(ILocationsRepository locationsRepository, IMarkViewProvider markViewProvider)
        {
            _locationsRepository = locationsRepository;
            _markViewProvider = markViewProvider;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var location = _locationsRepository.Find(id);

            if (location == null) return NotFound();

            return Ok(location);
        }

        [HttpPost("{id}")]
        [RestrictInvalidValuesFilter]
        public async Task<IActionResult> Post(int id, [FromBody] Location location)
        {
            if (!ModelState.IsValid) return BadRequest();
            if (!_locationsRepository.Exists(id)) return NotFound();

            var result = await _locationsRepository.UpdateAsync(id, location);
            if (result.IsNotFound) NotFound();

            return Ok(_emptyResult);
        }

        [HttpPost("new")]
        [RestrictInvalidValuesFilter]
        public IActionResult Post([FromBody] Location location)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = _locationsRepository.Create(location);
            if (result.IsFailure) BadRequest();

            return Ok(_emptyResult);
        }

        [HttpGet("{locationId}/avg")]
        public async Task<IActionResult> Get(int locationId, DateTime? fromDate, DateTime? toDate, int? fromAge, int? toAge, char? gender)
        {
            if (!ModelState.IsValid) return BadRequest();

            var filterQuery = MarkViewQueryBuilder.GetBuilder()
                .WithLocationId(locationId)
                .WithFromDate(fromDate)
                .WithToDate(toDate)
                .WithFromAge(fromAge)
                .WithToAge(toAge)
                .WithGender(gender)
                .Build();

            var response = await _markViewProvider.FindFilteredAsync(locationId, filterQuery);

            IActionResult result = null;

            response
                .ResourceNotFoundResponse(() => result = NotFound())
                .EmptyResponse(() => result = Ok(AverageMarkViewModel.Empty))
                .NonemptyResponse(r => result = Ok(new AverageMarkViewModel {Avg = Math.Round(r.Value.Average(mv => mv.Mark), 5)}));

            return result;
        }
    }
}
