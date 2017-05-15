using Akka.Actor;
using Akka.Configuration;
using PR.Primes.Actors;
using PR.Primes.Messages;
using System;

namespace PR
{
    class PrimesRun
    {
        private string localIP;
        private string remoteIP;
        private int hosts;
        private int agents;
        public Config configremote;
        public Config configlocal;


        public PrimesRun(int hosts, int agents, string localIP, string remoteIP)
        {
            this.localIP = localIP;
            this.remoteIP = remoteIP;
            this.hosts = hosts;
            this.agents = agents;

            this.configlocal = ConfigurationFactory.ParseString(@"
            akka {
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    serializers {
                          hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                    }
                    serialization-bindings {
                      ""System.Object"" = hyperion
                    }
                }
                remote {
                    dot-netty.tcp {
                        port = 8090
                        hostname = " + localIP + @"
                        maximum-frame-size = 30000000
                    }
                }
            }");

            this.configremote = ConfigurationFactory.ParseString(@"
            akka {
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    serializers {
                          hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                    }
                    serialization-bindings {
                      ""System.Object"" = hyperion
                    }
                }
                remote {
                    dot-netty.tcp {
                        port = 8080
                        hostname = " + remoteIP + @"
                        maximum-frame-size = 30000000
                    }
                }
            }");
        }

        public void run(int count)
        {
            runRemote();
            runLocal(count);
        }

        public void runLocal(int count)
        {
            var localSystem = ActorSystem.Create("localsystem", configlocal);
            IActorRef superMaster = localSystem.ActorOf(Props.Create(() => new SuperMaster(hosts, agents, localIP, remoteIP)), "superMaster");
            IActorRef localMaster = localSystem.ActorOf(Props.Create(typeof(Master)), "slave2");
            superMaster.Tell(new StartCalcMessage(0, count));
        }

        public void runRemote()
        {
            var remoteSystem = ActorSystem.Create("remotesystem", configremote);
            remoteSystem.ActorOf(Props.Create(typeof(Master)), "slave1");
        }

        static void Main(string[] args)
        {
            int range = 0;
            Console.WriteLine("Podaj zakres:");
            while (range <= 0)
            {
                range = int.Parse(Console.ReadLine());
            }

            // hosts count, agents count, localIP, remoteIP 
            new PrimesRun(2, 16, "192.168.0.14", "192.168.0.17").runLocal(range);

            //new PrimesRun(1, 4, "192.168.0.17", "192.168.0.17").runRemote();

            Console.ReadKey();
        }
    }
}
