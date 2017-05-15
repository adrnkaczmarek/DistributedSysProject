using System.Collections;

namespace PR.Primes.Messages
{
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
}
