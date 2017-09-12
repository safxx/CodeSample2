using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HighLoad.Application;
using HighLoad.Application.Data;
using HighLoad.Application.Data.ReadModel.VisitView;
using ServiceStack.OrmLite;

namespace HighLoad.Framework.Data.ReadModel
{
    public class VisitViewProvider : IVisitViewProvider
    {
        private readonly IVisitViewDbConnectionFactory _visitViewDbConnectionFactory;
        private readonly IUsersRepository _usersRepository;

        public VisitViewProvider(IVisitViewDbConnectionFactory visitViewDbConnectionFactory, IUsersRepository usersRepository)
        {
            _visitViewDbConnectionFactory = visitViewDbConnectionFactory;
            _usersRepository = usersRepository;
        }

        public async Task<FilteredResourceResult<VisitView>> FindFilteredAsync(int userId, Expression<Func<VisitView, bool>> filterQuery)
        {
            if (!_usersRepository.Exists(userId)) 
                return FilteredResourceResult<VisitView>.ResourceNotFound;

            IEnumerable<VisitView> visitViews;
            using (var db = _visitViewDbConnectionFactory.OpenDbConnection())
            {
                var query = db.From<VisitView>().Where(filterQuery);
                visitViews = await db.SelectAsync(query);
            }

            if (!visitViews.Any()) return FilteredResourceResult<VisitView>.Empty;

            return FilteredResourceResult<VisitView>.Success(visitViews.ToArray());
        }
    }
}