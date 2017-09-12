using System.Collections.Generic;
using System.Linq;
using HighLoad.Application.Data.ReadModel.VisitView;

namespace HighLoad.Framework.ViewModels
{
    public interface IVisitViewsContainerViewModelFactory
    {
        VisitViewsContainerViewModel Create(IReadOnlyCollection<VisitView> visitViews);
    }

    public class VisitViewsContainerViewModelFactory : IVisitViewsContainerViewModelFactory
    {
        public VisitViewsContainerViewModel Create(IReadOnlyCollection<VisitView> visitViews)
        {
            var viewModels = visitViews
                .Select(vv => new VisitViewModel {Mark = vv.Mark, VisitedAt = vv.VisitedAt, Place = vv.Place})
                .OrderBy(vv => vv.VisitedAt)
                .ToArray();

            return new VisitViewsContainerViewModel {Visits = viewModels};
        }
    }
}