using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrift.Protocol;
using Thrift.Transport;

namespace ThriftConvert.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            TTransport transport = new TSocket("localhost", 9090);
            TProtocol protocol = new TBinaryProtocol(transport);
            ConvertSvc.Client client= new ConvertSvc.Client(protocol);
            
            transport.Open();
            var units = String.Join(", ", client.availableUnits_func().Select(s => s));
            Console.WriteLine("Please enter lengths to convert in format '<length> <unit> in <unit>', using {0}", units);
            while (true)
            {
                var input = Console.ReadLine();
                var output = client.convert_func(input);
                Console.WriteLine(output);
            }
        }
    }
}
