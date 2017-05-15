namespace PR.Primes.Messages
{
    public class RangeMessage
    {
        public int first { get; set; }
        public int last { get; set; }

        public RangeMessage(int first, int last)
        {
            this.first = first;
            this.last = last;
        }
    }
}
