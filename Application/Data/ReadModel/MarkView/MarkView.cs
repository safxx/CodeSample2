using System;
using ServiceStack.DataAnnotations;

namespace HighLoad.Application.Data.ReadModel.MarkView
{
    public class MarkView
    {
        public int LocationId { get; set; }
        public DateTime VisitedAt { get; set; }
        public byte Age { get; set; }
        public bool Gender { get; set; }
        public byte Mark { get; set; }
        public int UserId { get; set; }
        [PrimaryKey]
        public int VisitId { get; set; }
    }
}