using System;
using ServiceStack.DataAnnotations;

namespace HighLoad.Application.Data.ReadModel.VisitView
{
    public class VisitView
    {
        [PrimaryKey]
        public int VisitId { get; set; }
        public int UserId { get; set; }
        public DateTime VisitedAt { get; set; }
        public byte Mark { get; set; }

        public int LocationId { get; set; }
        public string Place { get; set; }
        public string Country { get; set; }
        public int Distance { get; set; }
    }
}
