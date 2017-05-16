using Akka.Actor;
using Akka.Routing;
using PR.Primes.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PR.Primes.Actors
{
    public class Master : UntypedActor
    {
        private List<int> result;
        private IActorRef workerRouter;
        private IActorRef superMaster;
        private int responseCounter;
        private int workerCount { get; set; }
        private Stopwatch watch;

        protected override void OnReceive(object message)
        {
            if (message is StartMachineCalcMessage)
            {
                Console.WriteLine("Master started");
                watch = new Stopwatch();
                watch.Start();

                superMaster = Sender;
                this.result = new List<int>();
                StartMachineCalcMessage msg = (StartMachineCalcMessage)message;
                this.workerCount = msg.workerCount;

                workerRouter = Context.ActorOf(Props.Create<Worker>().WithRouter(new RoundRobinPool(this.workerCount)));

                for (int i = 0; i < workerCount; i++)
                {
                    workerRouter.Tell(new CalcChunkMessage(msg.first + i, msg.last, msg.jumpNumber, msg.primes));
                }
            }
            else if (message is CalcDoneMessage)
            {
                CalcDoneMessage msg = (CalcDoneMessage)message;
                result.AddRange(msg.primes);
                this.responseCounter++;

                if (responseCounter == this.workerCount)
                {
                    result.Sort();
                    superMaster.Tell(new CalcDoneMessage(result));
                    //stop watch
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    Console.WriteLine("Master finished in " + elapsedMs + "ms.");
                }
            }
        }
    }
}
