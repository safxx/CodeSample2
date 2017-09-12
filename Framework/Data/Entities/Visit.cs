using System;

namespace HighLoad.Framework.Data.Entities
{
    public class Visit : IEntity
    {
        public byte? Mark { get; set; }
        public DateTime? VisitedAt { get; set; }
        public int? UserId { get; set; }
        public int Id { get; set; }
        public int? LocationId { get; set; }
    }
}
