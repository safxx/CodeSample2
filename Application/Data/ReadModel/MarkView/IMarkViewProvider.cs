using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HighLoad.Application.Data.ReadModel.MarkView
{
    public interface IMarkViewProvider
    {
        Task<FilteredResourceResult<MarkView>> FindFilteredAsync(int locationId, Expression<Func<MarkView, bool>> filterQuery);
    }
}