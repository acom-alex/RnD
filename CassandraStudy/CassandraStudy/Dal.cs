using System;
using System.Linq;
using System.Collections.Generic;
using CassandraSharp;
using CassandraSharp.Config;
using CassandraSharp.CQL;
using CassandraSharp.CQLPoco;
using CassandraSharp.CQLPropertyBag;
//using CassandraSharp.Extensibility;

namespace CassandraStudy
{
    internal class Dal : IDal
    {
        public Dal()
        {
            XmlConfigurator.Configure();
        }

        public IEnumerable<IDictionary<string, object>> GetUsers(string startKey, int slice)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDictionary<string, object>> GetUser(string uid)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, object> GetUserInFlow(string uid, string flow)
        {
            throw new NotImplementedException();
        }

        public string GetLastStateForUser(string uid, string flow)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDictionary<string, object>> GetFlows(string startKey, int slice)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, object> GetFlow(string flow)
        {
            throw new NotImplementedException();
        }

        public int UpdateLastStateForUser(string uid, string lastState)
        {
            throw new NotImplementedException();
        }

        public int AddUser(IDictionary<string, object> user)
        {
            throw new NotImplementedException();
        }

        public int UpdateUser(IDictionary<string, object> user)
        {
            throw new NotImplementedException();
        }

        public int GenerateUsers(int num)
        {
            int count = 0;
            Random rnd = new Random();
            string[] flows = GetAllFlows().ToArray();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Guid[] ids = GenerateIds((int)Math.Ceiling(num * 0.8)).ToArray();

            using (ICluster cluster = ClusterManager.GetCluster("TestCassandra"))
            {
                var cmd = cluster.CreatePocoCommand();


                for (int i = 0; i < num; i++)
                {
                    string insertUsers = string.Format("INSERT INTO dispatch_cql3.users (uid, flow, last_state, test1, test2)" +
                                                           "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')", 
                                                           ids[rnd.Next(ids.Length)],
                                                           flows[rnd.Next(flows.Length)], 
                                                           "/random", 
                                                           chars[rnd.Next(chars.Length)],
                                                           chars[rnd.Next(chars.Length)]);
                    var resCount = cmd.Execute(insertUsers);
                    resCount.Wait();

                    if (resCount.IsCompleted)
                    {
                        count++;
                    }
                }
            }

            ClusterManager.Shutdown();
            return count; ;
        }

        public IEnumerable<Guid> GenerateIds(int num)
        {
            for (int i = 0; i < num; i++)
            {
                yield return Guid.NewGuid();
            }
        }

        private static IEnumerable<string> GetAllFlows()
        {
            List<string> flows = new List<string>();
            using (ICluster cluster = ClusterManager.GetCluster("TestCassandra"))
            {
                var cmd = cluster.CreatePropertyBagCommand();
                const string cql = "SELECT flow FROM dispatch_cql3.flows";
                var resUsers = cmd.Execute<IDictionary<string, object>>(cql).ContinueWith(res =>
                    {
                        foreach (var item in res.Result)
                        {
                            foreach (var dic in item)
                            {
                                flows.Add(dic.Value.ToString());
                            }
                        }
                    });

                resUsers.Wait();
            }

            return flows;
        }
    }
}
