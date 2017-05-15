using Akka.Actor;
using Akka.Routing;
using PR.Primes.Messages;
using System;
using System.Collections.Generic;

namespace PR.Primes.Actors
{
    public class Master : UntypedActor
    {
        private List<int> result;
        private IActorRef workerRouter;
        private IActorRef superMaster;
        private int responseCounter;
        private int workerCount { get; set; }

        protected override void OnReceive(object message)
        {
            if (message is StartMachineCalcMessage)
            {
                Console.WriteLine("Master started");
                superMaster = Sender;
                this.result = new List<int>();
                StartMachineCalcMessage msg = (StartMachineCalcMessage)message;
                int last = (int)Math.Floor(Math.Sqrt(msg.last));
                this.workerCount = msg.workerCount;
                int chunkSize = (msg.last - last) / this.workerCount;

                workerRouter = Context.ActorOf(Props.Create<Worker>().WithRouter(new RoundRobinPool(this.workerCount)));

                for (int i = 0; i < workerCount; i++)
                {
                    workerRouter.Tell(new CalcChunkMessage(chunkSize * i + last + 1,
                        chunkSize * i + chunkSize + last, msg.primes));
                }
            }
            else if (message is CalcDoneMessage)
            {
                CalcDoneMessage msg = (CalcDoneMessage)message;
                result.AddRange(msg.primes);
                this.responseCounter++;

                Console.WriteLine("Master received");
                if (responseCounter == this.workerCount)
                {
                    result.Sort();
                    superMaster.Tell(new CalcDoneMessage(result));
                    Console.WriteLine("Master finished");
                }
            }
        }
    }
}
