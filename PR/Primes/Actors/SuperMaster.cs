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
        private int workerCount;
        private int machinesCount;
        private int responseCounter;
        private List<int> result;
        private readonly ActorSelection remoteSlave = Context.ActorSelection("akka.tcp://remotesystem@localhost:8080/user/slave1");
        private readonly ActorSelection localSlave = Context.ActorSelection("akka.tcp://localsystem@localhost:8090/user/slave2");

        /*
         * Jako parametr przyjmuje liczbę workerów, które master ma stworzyć. Tworzy także aktora rutującego wiadomości
         * do poszczególnych workerów, z użyciem strategii RoundRobin
         */
        public SuperMaster(int machinesCount, int workerCount)
        {
            this.workerCount = workerCount;
            this.machinesCount = machinesCount;
            this.result = new List<int>();
            this.responseCounter = 0;
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
                
                localSlave.Tell(new StartMachineCalcMessage(last + 1, chunkSize + last, primesMap, workerCount / machinesCount));
                
                remoteSlave.Tell(new StartMachineCalcMessage(chunkSize + last + 1, 2 * chunkSize + last, primesMap, workerCount / machinesCount));     
            }
            else if (message is CalcDoneMessage)
            {
                CalcDoneMessage msg = (CalcDoneMessage)message;
                result.AddRange(msg.primes);
                this.responseCounter++;

                if (responseCounter == machinesCount)
                {
                    result.Sort();
                    Console.WriteLine("Calculation finished");
                }
            }
        }
    }
}
