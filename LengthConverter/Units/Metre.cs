using System.ComponentModel.Composition;

namespace LengthConverter.Units
{
    [Export(typeof(Unit))]
    [ExportMetadata("UnitName", "m")]
    public class Metre: Unit
    {
        public Metre()
        {
            AsMM = 1000;
        }
    }
}