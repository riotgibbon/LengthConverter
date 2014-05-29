using System.ComponentModel.Composition;
using LengthConverter.Units;

namespace LengthConverter.Extensions
{
    [Export(typeof(Unit))]
    [ExportMetadata("UnitName", "mm")]
    public class Millimetre:Unit
    {
        public Millimetre()
        {
            AsMM = 1;
        }
    }
}