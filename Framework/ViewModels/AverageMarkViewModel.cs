namespace HighLoad.Framework.ViewModels
{
    public class AverageMarkViewModel
    {
        static AverageMarkViewModel()
        {
            Empty = new AverageMarkViewModel();
        }

        public double Avg { get; set; }
        public static AverageMarkViewModel Empty { get; }
    }
}