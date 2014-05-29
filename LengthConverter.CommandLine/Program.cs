using System;
using LengthConverter.Converter;

namespace LengthConverter.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            var converter = new MEFConverter();
            var output = InputParser.ConvertInput(args, converter);
            Console.WriteLine(output);
            Console.ReadKey();
        }
    }
}
