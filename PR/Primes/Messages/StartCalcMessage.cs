namespace PR.Primes.Messages
{
    /*
     * Obiekt wysyłany do Mastera w celu zakomunikowania że proces obliczania liczb pierwszych ma zostac rozpoczety
     */
    public class StartCalcMessage : RangeMessage
    {
        /*
         * Otrzymuje zakres z którego mają zostać obliczone liczby pierwsze.
         */
        public StartCalcMessage(int first, int last) : base(first, last){}
    }
}
