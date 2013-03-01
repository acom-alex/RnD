using System;
using System.Linq;
using System.Collections.Generic;
using CassandraSharp;
using CassandraSharp.Config;
using CassandraSharp.CQL;
using CassandraSharp.CQLPoco;
using CassandraSharp.CQLPropertyBag;
using System.Diagnostics;
using System.Threading;
using CassandraStudy.Schemas;
using System.Threading.Tasks;
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
            using (ICluster cluster = ClusterManager.GetCluster("TestCassandra"))
            {
                var cmd = cluster.CreatePropertyBagCommand();
                string cqlUsers = string.IsNullOrEmpty(startKey) ?
                    string.Format("SELECT * FROM dispatch_cql3.users LIMIT {0}", slice) :
                    string.Format("SELECT * FROM dispatch_cql3.users WHERE token(uid) > token('{0}') LIMIT {1}", startKey, slice);

                var users = cmd.Execute<IDictionary<string, object>>(cqlUsers).AsFuture();
                users.Wait();

                return users.Result;
            }
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

        public Tuple<int, long> GenerateUsers(int num)
        {
            int count = 0;
            Random rnd = new Random();
            string[] flows = GetAllFlows().ToArray();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Guid[] ids = GenerateIds((int)Math.Ceiling(num * 0.8)).ToArray();
            //Guid[] ids = GenerateIds(num).ToArray();

            Stopwatch st = new Stopwatch();

            using (ICluster cluster = ClusterManager.GetCluster("TestCassandra"))
            {
                var cmd = cluster.CreatePocoCommand();
                const string countUsers = "select count(*) from dispatch_cql3.users limit 1000000";
                const string insertBatch = "INSERT INTO dispatch_cql3.users (uid, flow, last_state, test1, test2) VALUES (?, ?, ?, ?, ?)";

                // Count users before
                var resCount = cmd.Execute<CountSchema>(countUsers).Result;
                long countBefore = resCount.First().Count;

                // Add users
                st.Start();
                var preparedInsert = cmd.Prepare(insertBatch);
                List<Task> tasks = new List<Task>();


                for (int i = 0; i < num; i++)
                {
                    var res = preparedInsert.Execute(new
                        {
                            uid = ids[rnd.Next(ids.Length)].ToString(),
                            flow = flows[rnd.Next(flows.Length)],
                            last_state = "/random",
                            test1 = chars[rnd.Next(chars.Length)].ToString(),
                            test2 = chars[rnd.Next(chars.Length)].ToString()
                        },
                        ConsistencyLevel.QUORUM);//.ContinueWith(_ => Interlocked.Increment(ref count));
                    tasks.Add(res);

                    //res.Wait();
                }

                Task.WaitAll(tasks.ToArray(), 10);
                st.Stop();

                // Count users after
                resCount = cmd.Execute<CountSchema>(countUsers).Result;
                long countAfter = resCount.First().Count;

                count = (int)(countAfter - countBefore);
            }

            ClusterManager.Shutdown();
            return new Tuple<int, long>(count, st.ElapsedMilliseconds);
        }

        private IEnumerable<Guid> GenerateIds(int num)
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
