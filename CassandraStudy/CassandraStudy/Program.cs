using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CassandraSharp;
using CassandraSharp.CQL;
using CassandraSharp.Config;
using CassandraSharp.CQLPoco;
using CassandraSharp.CQLPropertyBag;
using CassandraSharp.Extensibility;
using System.Diagnostics;
using CassandraStudy.Schemas;


namespace CassandraStudy
{
    class Program
    {
        static void Main(string[] args)
        {
            Dal dal = new Dal();
            Stopwatch st = new Stopwatch();

            //// Generate users
            //Tuple<int, long> done = dal.GenerateUsers(1000);
            //Console.WriteLine("Created {0} users in {1} ms.", done.Item1, done.Item2);

            //// Reading Users
            st.Restart();
            var users = dal.GetUsers("", 0);
            st.Stop();
            int count = users.Count();
            DisplayResult(users);
            Console.WriteLine("Read {0} users in {1} ms.", count, st.ElapsedMilliseconds);


            //XmlConfigurator.Configure();
            //using (ICluster cluster = ClusterManager.GetCluster("TestCassandra"))
            //{
            ////    //Get all columns
            ////    //var cmd = cluster.CreatePocoCommand();
            ////    //const string cqlKeyspaces = "SELECT * from system.schema_columns where keyspace_name = 'dispatch_cql3' AND columnfamily_name = 'users'";
            ////    //var resTask = cmd.Execute<SchemaColumns>(cqlKeyspaces).ContinueWith(res => DisplayResult(res.Result));
            ////    //resTask.Wait();


            //    //    var cmd = cluster.CreatePropertyBagCommand();
            //    //    const string cqlUsers = "SELECT * FROM dispatch_cql3.users";// WHERE uid = '05f7200f-d000-0000-0000-000000000000' AND flow = 'Merlin_1'";
            //    //    var resUsers = cmd.Execute<IDictionary<string, object>>(cqlUsers).ContinueWith(res => DisplayResult(res.Result));
            //    //    resUsers.Wait();

            //    var cmd = cluster.CreatePropertyBagCommand();
            //    const string cql = "SELECT flow FROM dispatch_cql3.flows";// WHERE uid = '05f7200f-d000-0000-0000-000000000000' AND flow = 'Merlin_1'";
            //    var resUsers = cmd.Execute<IDictionary<string, object>>(cql).ContinueWith(res => DisplayResult(res.Result));
            //    resUsers.Wait();
            //}

            //ClusterManager.Shutdown();
            Console.ReadLine();
        }

        private static void DisplayResult(IEnumerable<IDictionary<string, object>> req)
        {
            foreach (var row in req)
            {
                foreach (var col in row)
                {
                    Console.Write("{0}:'{1}' ", col.Key, col.Value);   
                }

                Console.WriteLine();
            }
        }

        private static void DisplayResult(IEnumerable<ColumnSchema> req)
        {
            foreach (ColumnSchema schemaColumns in req)
            {
                Console.WriteLine("KeyspaceName={0} ColumnFamilyName={1} ColumnName={2} IndexName={3}",
                                  schemaColumns.KeyspaceName, schemaColumns.ColumnFamilyName, schemaColumns.ColumnName, schemaColumns.IndexName);
            }
        }
    }
}
