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
                }
                remote {
                    helios.tcp {
                        port = 8090
                        hostname = localhost
                    }
                }
            }");

        Config configremote = ConfigurationFactory.ParseString(@"
            akka {
                actor {
                    provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                }
                remote {
                    helios.tcp {
                        port = 8080
                        hostname = localhost
                    }
                }
            }");

        public void run()
        {
            var localSystem = ActorSystem.Create("localsystem", configlocal);
            var remoteSystem = ActorSystem.Create("remotesystem", configremote);
            var superMasterProps = Props.Create(() => new SuperMaster(localSystem, remoteSystem, 2, 8));
            IActorRef superMaster = localSystem.ActorOf(superMasterProps, "superMaster");
            
            IActorRef localMaster = localSystem.ActorOf(Props.Create(typeof(Master)), "slave1");
            IActorRef remoteMaster = remoteSystem.ActorOf(Props.Create(typeof(Master)), "slave1");

            superMaster.Tell(new StartCalcMessage(0, 255));

            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            new PrimesRun().run();
        }
    }
}
