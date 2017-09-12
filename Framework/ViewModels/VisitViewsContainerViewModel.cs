using System;

namespace HighLoad.Framework.ViewModels
{
    public class VisitViewsContainerViewModel
    {
        public VisitViewModel[] Visits { get; set; }

        public static VisitViewsContainerViewModel Empty { get; }

        static VisitViewsContainerViewModel()
        {
            Empty = new VisitViewsContainerViewModel{Visits = Array.Empty<VisitViewModel>()};
        }
    }
}
