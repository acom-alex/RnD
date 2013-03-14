using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using ZMQ;

namespace ZClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** ZMQ Client ***");

            using (var context = new Context(1))
            {
                using (Socket requester = context.Socket(SocketType.REQ))
                {
                    requester.Connect("tcp://localhost:5555");

                    while (true)
                    {
                        Console.Write("User ID: ");
                        string input = Console.ReadLine();

                        if (string.IsNullOrEmpty(input))
                        {
                            break;
                        }

                        int uid;
                        if (int.TryParse(input, out uid))
                        {
                            // Send request.
                            byte[] req;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                ProtoBuf.Serializer.Serialize<UserRequest>(ms, new UserRequest(uid));
                                req = ms.ToArray();
                            }
                            requester.Send(req);

                            // Get response.
                            byte[] response = requester.Recv();                            

                            if (response.Length > 0)
                            {
                                User user;
                                using (MemoryStream ms = new MemoryStream(response))
                                {
                                    user = ProtoBuf.Serializer.Deserialize<User>(ms);
                                }

                                Console.WriteLine("Id: {0}, Name: {1}, Active: {2}", user.Id, user.Name, user.Active);
                                Console.WriteLine("Meta: {0}", PrintMeta(user.Metadata));
                            }
                            else
                            {
                                Console.WriteLine("User not found.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Id. Must be an int.");
                        }

                        Console.WriteLine();
                    }
                }
            }
        }

        private static string PrintMeta(IDictionary<string, string> meta)
        {
            if (meta == null || meta.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in meta)
            {
                sb.AppendFormat("\t{0}: {1}\n", item.Key, item.Value);
            }

            return sb.ToString();
        }
    }
}
