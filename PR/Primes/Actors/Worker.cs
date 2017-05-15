using Akka.Actor;
using PR.Primes.Messages;
using System;
using System.Collections;
using System.Collections.Generic;

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
                Console.WriteLine("Worker started");
                CalcChunkMessage msg = (CalcChunkMessage)message;
                int first = msg.first;
                int last = msg.last;
                BitArray primes = msg.primes;
                BitArray primeNumbers = new BitArray(last - first + 1, true);
                List<int> primesToResponse = new List<int>();

                for (int i = 0; i < primes.Length; i++)
                {
                    if (primes[i])
                    {
                        for (int j = first; j <= last; j++)
                        {
                            if (j % (i + 2) == 0)
                            {
                                primeNumbers[j - first] = false;
                            }
                        }
                    }
                }

                for (int i = 0; i < primeNumbers.Length; i++)
                {
                    if (primeNumbers[i])
                    {
                        primesToResponse.Add(first + i);
                    }
                }
                Sender.Tell(new CalcDoneMessage(primesToResponse));
            }
        }
    }
}
