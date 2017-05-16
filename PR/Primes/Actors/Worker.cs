using Akka.Actor;
using PR.Primes.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PR.Primes.Actors
{
    /*
     *Aktor odpowiedzialny za obliczenie liczb pierwszych z otrzymanego zakresu 
     */
    public sealed class Worker : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is CalcChunkMessage)
            {
                CalcChunkMessage msg = (CalcChunkMessage)message;
                int first = msg.first;
                int last = msg.last;
                int jump = msg.jumpNumber;
                BitArray primes = msg.primes;
                List<int> primesToResponse = new List<int>();

                Console.WriteLine("Worker started with first=" + first + " and jump=" + jump);
                Stopwatch watch = new Stopwatch();
                watch.Start();

                if(!(first%2 == 0 && jump%2==0))
                {
                    for (int i = first; i <= last; i += jump)
                    {
                        for (int j = 0; j < primes.Length; j++)
                        {
                            if (primes[j])
                            {
                                if(i % (j + 2) == 0)
                                {
                                    break;
                                }
                            }

                            if (j == (primes.Length - 1))
                            {
                                primesToResponse.Add(i);
                            }
                        }
                    }
                }
               
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("Worker finished in " + elapsedMs + "ms.");
                Sender.Tell(new CalcDoneMessage(primesToResponse));
            }
        }
    }
}
