using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HighLoad.Application.Data.ReadModel.VisitView
{
    public interface IVisitViewProvider
    {
        Task<FilteredResourceResult<VisitView>> FindFilteredAsync(int userId, Expression<Func<VisitView, bool>> filterQuery);
    }
}