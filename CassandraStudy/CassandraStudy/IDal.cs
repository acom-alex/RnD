using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassandraStudy
{
    internal interface IDal
    {
        IEnumerable<IDictionary<string, object>> GetUsers(string startKey, int slice);

        IEnumerable<IDictionary<string, object>> GetUser(string uid);

        IDictionary<string, object> GetUserInFlow(string uid, string flow);

        string GetLastStateForUser(string uid, string flow);

        IEnumerable<IDictionary<string, object>> GetFlows(string startKey, int slice);

        IDictionary<string, object> GetFlow(string flow);

        int UpdateLastStateForUser(string uid, string lastState);

        int AddUser(IDictionary<string, object> user);

        int UpdateUser(IDictionary<string, object> user);
    }
}
