using System.ComponentModel.Composition;
using LengthConverter.Units;

namespace LengthConverter.Extensions
{
    [Export(typeof(Unit))]
    [ExportMetadata("UnitName", "cu")]
    public class Cubit:Unit
    {
        public Cubit()
        {
            AsMM = 457.2;
        }
    }
}