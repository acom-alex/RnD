using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace Dal
{
    public class MockedUserDao : IDao
    {
        private static string[] metaKeys = { "name", "dob", "dod", "surname", "pob", "pod", "address", "ssn", "employer", "citizenship" };
        private static string[] names = { "Rocio Radtke", "Lorretta Larock", "Annetta Arendt", "Arica Arendt", "Gayla Garris", "Betty Brathwaite", "Sharell Spadaro", "Marjorie Michel", "Corinna Carleton", "Easter Eggleston", "Carina Crick", "Isaiah Iwamoto", "Bella Bullington", "Bea Bento", "Romelia Rhett", "Michele Mulvey", "Kathlene Kinlaw", "Ida Imai", "Merrie Morin", "Shanta Saulsberry", "Thora Terhaar", "Lee Lemelle", "Antonio Augsburger", "Lilliana Looby", "Mafalda Mccorvey", "Booker Bejarano", "Bernita Buis", "Marleen Madia", "Eugene Exley", "Bari Brant" };
        private static IList<User> usersCache;

        private static Random rnd;

        static MockedUserDao()
        {
            rnd = new Random();
            usersCache = new List<User>();
            for (int i = 1; i <= 100; i++)
            {
                usersCache.Add(new User(i, names[rnd.Next(names.Length)], rnd.Next() % 2 == 0, GenerateMetaData()));
            }
        }

        public MockedUserDao()
        {
            if (usersCache == null || usersCache.Count == 0)
            {
                throw new InvalidProgramException("Invalid initialization.");
            }
        }

        public User GetUser(int id)
        {
            return usersCache.SingleOrDefault(u => u.Id == id);
        }

        public IEnumerable<User> GetUsers()
        {
            return usersCache;
        }

        public int AddUser(string name, IDictionary<string, string> meta)
        {
            int i = usersCache.Max<User>(u => u.Id);
            usersCache.Add(new User(i++, name, false, meta));
            return i;
        }

        public void ActivateUser(int id)
        {
            usersCache.Single(u => u.Id == id).Active = true;
        }

        public static IDictionary<string, string> GenerateMetaData()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            int max = metaKeys.Length;
            int count = rnd.Next(3, max);
            for (int i = 0; i < count; i++)
            {
                string key;
                do
                {
                    key = metaKeys[rnd.Next(0, max)];
                } while (retVal.ContainsKey(key));

                retVal.Add(key, Guid.NewGuid().ToString());
            }

            return retVal;
        }
    }
}
