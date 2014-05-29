using System;
using System.Linq;
using LengthConverter.Converter;

namespace LengthConverter
{
    public class InputParser
    {
        public static string ConvertInput(string input, IConvertLengths converter)
        {
            return ConvertInput(input.Split(' '), converter);
        }

        public static string ConvertInput(string[] args, IConvertLengths converter)
        {
            const string correctFormat = "Please use format '<length> <unit> in <unit>'";
            

            if (args==null || args.Length != 4) return correctFormat;

            if (args[2] != "in") return "Incorrect command, expected 'in'. " + correctFormat;

            double inputLength;
            var inputLengthParsed = double.TryParse(args[0],out inputLength);
            if (!inputLengthParsed) return string.Format("'{0}' is not a valid number. {1}", args[0], correctFormat);
            var inputFormat = args[1];
            var outputFormat = args[3];

            double converted;
            try
            {
                converted = converter.ConvertLength(inputFormat, outputFormat, inputLength);
            }
            catch (ArgumentOutOfRangeException e)
            {
                switch (e.ParamName)
                {
                    case "inputFormat":
                        return IncorrectUnitMessage(inputFormat, converter);
                    case "outputFormat":
                        return IncorrectUnitMessage(outputFormat, converter);
                    default:
                        return correctFormat;
                }
            }

            var output = string.Format("{0} {1} equals {2} {3}", inputLength, inputFormat, converted, outputFormat);

            return output;
        }

        private static string IncorrectUnitMessage(string format, IConvertLengths converter)
        {
            string correctUnits = "Available units are " + String.Join(", ", converter.AvailableUnits.Select(p => "'" + p.ToString() + "'"));
            return string.Format("'{0}' is not a valid unit. {1}", format, correctUnits);
        }
    }
}