using Akka.Actor;
using Akka.Routing;
using PR.Primes.Messages;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PR.Primes.Actors
{
    public sealed class SuperMaster : UntypedActor
    {
        private ActorSystem localSystem;
        private ActorSystem remoteSystem;
        private int workerCount;
        private int machinesCount;
        private int responseCounter;
        private List<int> result;
        
        /*
         * Jako parametr przyjmuje liczbę workerów, które master ma stworzyć. Tworzy także aktora rutującego wiadomości
         * do poszczególnych workerów, z użyciem strategii RoundRobin
         */
        public SuperMaster(ActorSystem localSystem, ActorSystem remoteSystem, int machinesCount, int workerCount)
        {
            this.workerCount = workerCount;
            this.machinesCount = machinesCount;
            this.result = new List<int>();
            this.responseCounter = 0;
            this.localSystem = localSystem;
            this.remoteSystem = remoteSystem;
        }

        protected override void OnReceive(object message)
        {
            if (message is StartCalcMessage)
            {
                Console.WriteLine("SuperMaster started");
                StartCalcMessage msg = (StartCalcMessage)message;
                int last = (int)Math.Floor(Math.Sqrt(msg.last));
                int chunkSize = (msg.last - last) / machinesCount;
                BitArray primesMap = new BitArray(last - 1, true);
                
                for (int i = 2; i < last; i++)
                {
                    if (primesMap[i - 2])
                    {
                        result.Add(i);

                        for (int j = i * i; j <= last; j += i)
                        {
                            primesMap[j - 2] = false;
                        }
                    }
                }
                
                var localSlave = localSystem.ActorSelection("akka.tcp://localsystem@localhost:8090/user/slave1");
                localSlave.Tell(new StartMachineCalcMessage(last + 1, chunkSize + last, primesMap, workerCount / machinesCount));

                for (int i = 1; i < machinesCount; i++)
                {
                    var path = "akka.tcp://remotesystem@localhost:8080/user/slave" + i;
                    var remoteSlave = remoteSystem.ActorSelection(path);
                    remoteSlave.Tell(new StartMachineCalcMessage(chunkSize * i + last + 1,
                        chunkSize * i + chunkSize + last, primesMap, workerCount / machinesCount));
                }
            }
            else if (message is CalcDoneMessage)
            {
                CalcDoneMessage msg = (CalcDoneMessage)message;
                result.AddRange(msg.primes);
                this.responseCounter++;
                Console.WriteLine("SuperMaster received");

                if (responseCounter == machinesCount)
                {
                    result.Sort();
                    Console.WriteLine("Calculation finished");
                }
            }
        }
    }
}
