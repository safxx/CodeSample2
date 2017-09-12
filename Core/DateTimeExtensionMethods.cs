using System;

namespace HighLoad.Core
{
    public static class DateTimeExtensionMethods
    {
        public static int CalculateAge(this DateTime birthDate)
        {
            var currentDate = DateTime.UtcNow;
            var age = currentDate.Year - birthDate.Year;
            if (birthDate > currentDate.AddYears(-age)) age--;
            return age;
        }
    }
}
