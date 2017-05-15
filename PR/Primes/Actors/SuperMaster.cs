using Akka.Actor;
using Akka.Routing;
using PR.Primes.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PR.Primes.Actors
{
    public sealed class SuperMaster : UntypedActor
    {
        private int workerCount;
        private int machinesCount;
        private int responseCounter;
        private List<int> result;
        private string localIP;
        private string remoteIP;
        private ActorSelection remoteSlave = Context.ActorSelection("akka.tcp://remotesystem@192.168.0.17:8080/user/slave1");
        private ActorSelection localSlave = Context.ActorSelection("akka.tcp://localsystem@192.168.0.17:8090/user/slave2");
        private Stopwatch watch;
        /*
         * Jako parametr przyjmuje liczbę workerów, które master ma stworzyć. Tworzy także aktora rutującego wiadomości
         * do poszczególnych workerów, z użyciem strategii RoundRobin
         */
        public SuperMaster(int machinesCount, int workerCount, string localIP, string remoteIP)
        {
            this.remoteIP = remoteIP;
            this.localIP = localIP;
            this.watch = new Stopwatch();
            this.workerCount = workerCount;
            this.machinesCount = machinesCount;
            this.result = new List<int>();
            this.responseCounter = 0;

            remoteSlave = Context.ActorSelection("akka.tcp://remotesystem@" + remoteIP + ":8080/user/slave1");
            localSlave = Context.ActorSelection("akka.tcp://localsystem@" + localIP + ":8090/user/slave2");
        }

        protected override void OnReceive(object message)
        {
            if (message is StartCalcMessage)
            {
                Console.WriteLine("SuperMaster started");
                watch.Start();
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
                    //stop watch
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    Console.WriteLine("SuperMaster finished in " + elapsedMs + "ms.");

                    //Save to file
                    result.Sort();
                    using (TextWriter tw = new StreamWriter("SavedList.txt"))
                    {
                        foreach (int i in result)
                            tw.Write(i.ToString() + ", ");
                    }


                }
            }
        }
    }
}
