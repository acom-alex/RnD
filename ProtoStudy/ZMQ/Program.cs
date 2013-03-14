using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal;
using Models;
using ProtoBuf;
using ZMQ;

namespace ZMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** ZMQ Server ***");

            using (var context = new Context(1))
            {
                using (Socket responder = context.Socket(SocketType.REP))
                {
                    responder.Bind("tcp://*:5555");

                    IDao dal = new MockedUserDao();

                    string proto = Serializer.GetProto<UserRequest>();

                    while (true)
                    {
                        try
                        {
                            var request = responder.Recv();
                            UserRequest uid;
                            using (MemoryStream ms = new MemoryStream(request))
                            {
                                uid = Serializer.Deserialize<UserRequest>(ms);
                            }

                            Console.Write("Received request for {0}.", uid.UserId);

                            User u = dal.GetUser(uid.UserId);
                            if (u != null)
                            {
                                Console.WriteLine(" Found.");
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    Serializer.Serialize<User>(ms, u);
                                    responder.Send(ms.ToArray());
                                }
                            }
                            else
                            {
                                Console.WriteLine(" Not found.");
                                responder.Send();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Console.WriteLine("Error: {0}", ex.Message);
                            break;
                        }
                    }
                }
            }
        }
    }
}
