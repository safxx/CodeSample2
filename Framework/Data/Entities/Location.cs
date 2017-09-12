
namespace HighLoad.Framework.Data.Entities
{
    public class Location : IEntity
    {
        public int? Distance { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Place { get; set; }
        public int Id { get; set; }
    }
}
