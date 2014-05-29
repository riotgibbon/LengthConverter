using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrift.Server;
using Thrift.Transport;

namespace ThriftConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            var handler = new ConverterHandler();
            var processor = new ConvertSvc.Processor(handler);
            var serverTransport = new TServerSocket(9090);
            var server = new TSimpleServer(processor, serverTransport);

            // Use this for a multithreaded server
            // server = new TThreadPoolServer(processor, serverTransport);

            Console.WriteLine("Starting the server...");
            server.Serve();
        }
    }
}
