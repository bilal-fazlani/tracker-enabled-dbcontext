using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext.Common.Testing.Code
{
    public static class RandomUtility
    {
        public static string RandomText => Guid.NewGuid().ToString();

        public static int RandomNumber => new Random().Next(100, 200);

        public static DateTime RandomDate => DateTime.Now.AddDays(-RandomNumber);

        public static char RandomChar
        {
            get
            {
                int num = new System.Random().Next(0, 26); // Zero to 25
                char let = (char)('a' + num);
                return let;
            }
        }

        public static bool RandomBoolean => new Random().Next(2) == 0;
    }
}
