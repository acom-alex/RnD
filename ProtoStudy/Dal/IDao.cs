using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace Dal
{
    public interface IDao
    {
        int AddUser(string name, IDictionary<string, string> meta);
        User GetUser(int id);
        IEnumerable<User> GetUsers();
        void ActivateUser(int id);
    }
}
