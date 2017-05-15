namespace PR.Primes.Messages
{
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
}
