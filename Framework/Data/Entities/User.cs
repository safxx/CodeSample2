using System;

namespace HighLoad.Framework.Data.Entities
{
    public class User : IEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public char? Gender { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Id { get; set; }
    }
}
