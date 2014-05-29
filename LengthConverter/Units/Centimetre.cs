using System.ComponentModel.Composition;

namespace LengthConverter.Units
{
    [Export(typeof(Unit))]
    [ExportMetadata("UnitName", "cm")]
    public class Centimetre:Unit
    {
        public Centimetre()
        {
            AsMM = 10;
        }
    }
}