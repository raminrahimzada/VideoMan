using System;
using System.Linq;

namespace AllUnitTests
{
    public static class RandomHelpers
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Random _random;

        static RandomHelpers()
        {
            _random = new Random();
        }

        public static int RandomInteger()
        {
            return _random.Next();
        }
        public static int RandomInteger(int maxLength)
        {
            return _random.Next(maxLength);
        }
        public static string RandomString()
        {
            var length = RandomInteger();
            return RandomString(length);
        }
        public static string RandomString(int maxLength)
        {
            var length = RandomInteger(maxLength);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static DateTime RandomDate(DateTime max)
        {
            var start = new DateTime(1995, 1, 1);
            var range = (DateTime.Today - start).Days;
            return start.AddDays(_random.Next(range));
        }
        public static DateTime RandomDate()
        {
            return RandomDate(DateTime.MaxValue);
        }
    }
}