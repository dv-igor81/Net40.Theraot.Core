namespace AsyncEnumerable
{
    public class ParseResult
    {
        public string Url { get; set; }
        public override string ToString()
        {
            return $"Item from {Url}";
        }
    }
}