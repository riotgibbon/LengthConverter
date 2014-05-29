using System;
using LengthConverter.Units;

namespace LengthConverter.Converter
{
    public class BasicConverter : IConvertLengths
    {
        public virtual string[] AvailableUnits
        {
            get
            {
              return  new[]{"m", "cm", "in", "ft", "yd"};
            } 
        }

        public double ConvertLength(string inputFormat, string outputFormat, double inputLength)
        {
            var from = GetUnit(inputFormat);
            if (from == null) throw new ArgumentOutOfRangeException("inputFormat","Unknown input format");
            var to = GetUnit(outputFormat);
            if (to == null) throw new ArgumentOutOfRangeException("outputFormat", "Unknown output format");
            return from.ConvertTo(inputLength, to);
        }

        public virtual Unit GetUnit(string unitName)
        {
            switch (unitName)
            {
                case "cm":
                    return new Centimetre();
                case "m":
                    return new Metre();
                case "in":
                    return new Inch();
                case "ft":
                    return new Feet(); 
                case "yd":
                    return new Yard();
            }
            return null;
        }
    }
}
