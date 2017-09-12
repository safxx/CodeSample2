using System;

namespace HighLoad.Application.Entities
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public char? Gender { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Id { get; set; }
    }
}