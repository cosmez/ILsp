using ILsp.Compiler;
using System;
using System.IO;
using System.Text;

namespace ILsp
{
    class Program
    {
        static void Main()
        {
            string input = "( (123_453/124_548 #d45.25 3_243.43 #b1110_1010/1101 #x151DEA0)  ------) ) ( ) ( )";
            Console.WriteLine($"Input: {input}");
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            using var streamr = new StreamReader(stream);
            using var tokenizer = new Tokenizer(streamr);
            while (tokenizer.MoveNext())
            {
                var token = tokenizer.Current;
                Console.WriteLine(token);
            }
        }
    }
}
