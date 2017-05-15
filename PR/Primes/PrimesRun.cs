using Akka.Actor;
using Akka.Configuration;
using PR.Primes.Actors;
using PR.Primes.Messages;
using System;

namespace PR
{
    class PrimesRun
    {
        Config configlocal = ConfigurationFactory.ParseString(@"
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
                        hostname = localhost
                    }
                }
            }");

        Config configremote = ConfigurationFactory.ParseString(@"
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
                        hostname = localhost
                    }
                }
            }");

        public void run()
        {
            runRemote();
            runLocal();

            Console.ReadKey();
        }

        private void runLocal()
        {
            var localSystem = ActorSystem.Create("localsystem", configlocal);
            IActorRef superMaster = localSystem.ActorOf(Props.Create(() => new SuperMaster(2, 8)), "superMaster");
            IActorRef localMaster = localSystem.ActorOf(Props.Create(typeof(Master)), "slave2");
            superMaster.Tell(new StartCalcMessage(0, 255));
        }
        private void runRemote()
        {
            var remoteSystem = ActorSystem.Create("remotesystem", configremote);
            remoteSystem.ActorOf(Props.Create(typeof(Master)), "slave1");
        }

        static void Main(string[] args)
        {
            new PrimesRun().run();
        }
    }
}
