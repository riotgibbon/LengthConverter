namespace LengthConverter.Converter
{
    public interface IConvertLengths
    {
        string[] AvailableUnits { get; }
        double ConvertLength(string inputFormat, string outputFormat, double inputLength);
    }
}