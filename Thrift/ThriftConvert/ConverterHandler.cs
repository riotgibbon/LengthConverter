using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using LengthConverter;
using LengthConverter.Converter;

namespace ThriftConvert
{
    public class ConverterHandler:ConvertSvc.Iface
    {
        private MEFConverter _converter;

        public ConverterHandler()
        {
            _converter = new MEFConverter();
        }

        public string convert_func(string input)
        {

            Console.WriteLine("Received: {0}", input);
            var output = InputParser.ConvertInput(input, _converter);
            Console.WriteLine("Converted: {0}", output);
            return output;
        }

        public List<string> availableUnits_func()
        {
            return _converter.AvailableUnits.ToList();
        }
    }
}
