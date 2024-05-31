using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Net40.AsyncStreamDemo.WPF
{
    public static class StreamReaderExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static async Task<string> ReadLineAsync(this StreamReader reader)
        {
            if (reader == null)
            {
                throw new NullReferenceException();
            }
            return await TaskEx.Run(reader.ReadLine);
        }
    }
}