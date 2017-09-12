using System;

namespace HighLoad.Application.Entities
{
    public class Visit
    {
        public byte? Mark { get; set; }
        public DateTime? VisitedAt { get; set; }
        public int? UserId { get; set; }
        public int Id { get; set; }
        public int? LocationId { get; set; }
    }
}
