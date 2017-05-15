using System.Collections;

namespace PR.Primes.Messages
{
    public class StartMachineCalcMessage : RangeMessage
    {
        public BitArray primes { get; set; }
        public int workerCount { get; set; }
        /*
        * Otrzymuje zakres z którego mają zostać obliczone liczby pierwsze, oraz tablice bitów oznaczających,
        * które liczby z zakresu 0 do Math.Sqrt(n) są pierwsze, a które nie
        */
        public StartMachineCalcMessage(int first, int last, BitArray primes, int workerCount) : base(first, last)
        {
            this.primes = primes;
            this.workerCount = workerCount;
        }
    }
}
