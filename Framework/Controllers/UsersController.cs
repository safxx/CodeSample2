using System;
using System.Threading.Tasks;
using HighLoad.Application;
using HighLoad.Application.Data;
using HighLoad.Application.Data.ReadModel.VisitView;
using HighLoad.Application.Entities;
using HighLoad.Framework.Filters;
using HighLoad.Framework.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HighLoad.Framework.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private static readonly object _emptyResult = new object();
        private readonly IUsersRepository _usersRepository;
        private readonly IVisitViewProvider _visitViewProvider;
        private readonly IVisitViewsContainerViewModelFactory _visitViewsContainerViewModelFactory;

        public UsersController(IUsersRepository usersRepository, IVisitViewProvider visitViewProvider, IVisitViewsContainerViewModelFactory visitViewsContainerViewModelFactory)
        {
            _usersRepository = usersRepository;
            _visitViewProvider = visitViewProvider;
            _visitViewsContainerViewModelFactory = visitViewsContainerViewModelFactory;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _usersRepository.Find(id);

            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost("{id}")]
        [RestrictInvalidValuesFilter]
        public async Task<IActionResult> Post(int id, [FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _usersRepository.UpdateAsync(id, user);
            if (result.IsNotFound) return NotFound();

            return Ok(_emptyResult);
        }

        [HttpPost("new")]
        [RestrictInvalidValuesFilter]
        public IActionResult Post([FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = _usersRepository.Create(user);
            if (result.IsFailure) return BadRequest();

            return Ok(_emptyResult);
        }

        [HttpGet("{userId}/visits")]
        public async Task<IActionResult> GetVisits(int userId, DateTime? fromDate, DateTime? toDate, string country, int? toDistance)
        {
            if (!ModelState.IsValid) return BadRequest();

            var query = VisitViewQueryBuilder.GetBuilder()
                .WithUserId(userId)
                .WithFromDate(fromDate)
                .WithToDate(toDate)
                .WithCountry(country)
                .WithToDistance(toDistance)
                .Build();

            var response = await _visitViewProvider.FindFilteredAsync(userId, query);

            IActionResult result = null;

            response
                .ResourceNotFoundResponse(() => result = NotFound())
                .EmptyResponse(() => result = Ok(VisitViewsContainerViewModel.Empty))
                .NonemptyResponse(r => result = Ok(_visitViewsContainerViewModelFactory.Create(r.Value)));

            return result;
        }
    }
}