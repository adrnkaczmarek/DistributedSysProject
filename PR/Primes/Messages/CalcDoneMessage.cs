using System.Collections.Generic;

namespace PR.Primes.Messages
{
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
}
