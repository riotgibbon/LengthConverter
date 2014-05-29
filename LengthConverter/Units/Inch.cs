using System.ComponentModel.Composition;

namespace LengthConverter.Units
{
    [Export(typeof(Unit))]
    [ExportMetadata("UnitName", "in")]
    public class Inch:Unit
    {
        public Inch()
        {
            AsMM = 25.4;
        }
    }
}