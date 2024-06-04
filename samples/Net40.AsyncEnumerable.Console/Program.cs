using System.Threading.Tasks;

namespace AsyncEnumerable
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var parser = new AdvansedParser();
            parser.Parse().GetAwaiter().GetResult();
        }
    }
}