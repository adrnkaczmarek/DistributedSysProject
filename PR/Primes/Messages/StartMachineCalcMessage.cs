using System.Collections;

namespace PR.Primes.Messages
{
    public class StartMachineCalcMessage
    {
        public int first { get; set; }
        public int last { get; set; }
        public BitArray primes { get; set; }
        public int workerCount { get; set; }
        /*
        * Otrzymuje zakres z którego mają zostać obliczone liczby pierwsze, oraz tablice bitów oznaczających,
        * które liczby z zakresu 0 do Math.Sqrt(n) są pierwsze, a które nie
        */
        public StartMachineCalcMessage(int first, int last, BitArray primes, int workerCount)
        {
            this.first = first;
            this.last = last;
            this.primes = primes;
            this.workerCount = workerCount;
        }
    }
}
