using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HighLoad.Application;
using HighLoad.Application.Data;
using HighLoad.Application.Data.ReadModel.MarkView;
using ServiceStack.OrmLite;

namespace HighLoad.Framework.Data.ReadModel
{
    public class MarkViewProvider : IMarkViewProvider
    {
        private readonly IMarkViewDbConnectionFactory _markViewDbConnectionFactory;
        private readonly ILocationsRepository _locationsRepository;

        public MarkViewProvider(IMarkViewDbConnectionFactory markViewDbConnectionFactory, ILocationsRepository locationsRepository)
        {
            _markViewDbConnectionFactory = markViewDbConnectionFactory;
            _locationsRepository = locationsRepository;
        }

        public async Task<FilteredResourceResult<MarkView>> FindFilteredAsync(int locationId, Expression<Func<MarkView, bool>> filterQuery)
        {
            if (!_locationsRepository.Exists(locationId))
                return FilteredResourceResult<MarkView>.ResourceNotFound;

            IEnumerable<MarkView> markViews;
            using (var db = _markViewDbConnectionFactory.OpenDbConnection())
            {
                var query = db.From<MarkView>().Where(filterQuery);
                markViews = await db.SelectAsync(query);
            }

            if (!markViews.Any()) return FilteredResourceResult<MarkView>.Empty;

            return FilteredResourceResult<MarkView>.Success(markViews.ToArray());
        }
    }
}