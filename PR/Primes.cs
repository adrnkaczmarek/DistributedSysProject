using Akka.Actor;
using Akka.Routing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PR
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

    /*
     * Aktor znajdujący liczby pierwsze z zakresu 0 - Math.Sqrt(n), pozostałe liczby dzieli pomiędzy 
     * aktorów typu Worker.
     */
    public sealed class Master : UntypedActor
    {
        private IActorRef workerRouter;
        private int workerCount;
        private int responseCounter;
        private List<int> result;

        /*
         * Jako parametr przyjmuje liczbę workerów, które master ma stworzyć. Tworzy także aktora rutującego wiadomości
         * do poszczególnych workerów, z użyciem strategii RoundRobin
         */
        public Master(int workerCount)
        {
            this.workerCount = workerCount;
            workerRouter = Context.ActorOf(Props.Create<Worker>().WithRouter(new RoundRobinPool(workerCount)));
            result = new List<int>();
            responseCounter = 0;
        }

        protected override void OnReceive(object message)
        {
            if (message is StartCalcMessage)
            {
                StartCalcMessage msg = (StartCalcMessage)message;
                int last = (int)Math.Floor(Math.Sqrt(msg.last));
                int chunkSize = (msg.last - last) / workerCount;
                BitArray primesMap = new BitArray(last - 1, true);

                /*
                 * pętla obliczająca liczby pierwsze z zakresu 2 Math.Floor(n). Każda liczba ma przyporządkowany indeks
                 * w tablicy primesMap, jezeli liczba jest pierwszą indeks będzie zawierał wartość true
                 */
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

                for (int i = 0; i < workerCount; i++)
                {
                    //Wysyłanie zakresów liczb do poszczególnych workerów, przy pomocy routera
                    workerRouter.Tell(new CalcChunkMessage(chunkSize * i + last + 1, chunkSize * i + chunkSize + last, primesMap));
                }
            }
            else if (message is CalcDoneMessage)
            {
                CalcDoneMessage msg = (CalcDoneMessage)message;
                result.AddRange(msg.primes);
                this.responseCounter++;

                if (responseCounter == workerCount)
                {
                    result.Sort();
                }
            }
        }
    }

    /*
     * Obiekt wysyłany do Mastera w celu zakomunikowania że proces obliczania liczb pierwszych ma zostac rozpoczety
     */
    public class StartCalcMessage
    {
        public int first { get; set; }
        public int last { get; set; }

        /*
         * Otrzymuje zakres z którego mają zostać obliczone liczby pierwsze.
         */
        public StartCalcMessage(int first, int last)
        {
            this.first = first;
            this.last = last;
        }
    }

    /*
     * Obiekt wysyłany do Workera w celu zakomunikowania że ma on obliczyc liczby pierwsze
     */
    public class CalcChunkMessage
    {
        public int first { get; set; }
        public int last { get; set; }
        public BitArray primes { get; set; }

        /*
        * Otrzymuje zakres z którego mają zostać obliczone liczby pierwsze, oraz tablice bitów oznaczających,
        * które liczby z zakresu 0 do Math.Sqrt(n) są pierwsze, a które nie
        */
        public CalcChunkMessage(int first, int last, BitArray primes)
        {
            this.first = first;
            this.last = last;
            this.primes = primes;
        }
    }

    /*
     * Obiekt wysyłany od Workera do Mastera, zawiera liste obiczonych liczb pierwszych
     */
    public class CalcDoneMessage
    {
        public List<int> primes { get; set; }

        /*
         * Orzymuje obliczone liczb pierwsze
         */
        public CalcDoneMessage(List<int> primes)
        {
            this.primes = primes;
        }
    }
    
    class Primes
    {
        public void run()
        {
            var system = ActorSystem.Create("PrimeValuesSystem");

            var props = Props.Create(() => new Master(5));
            IActorRef master = system.ActorOf(props, "master");

            master.Tell(new StartCalcMessage(0, 255));

            Console.ReadLine();
        }
    }
}
