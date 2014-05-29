using System.ComponentModel.Composition;
using LengthConverter.Units;

namespace LengthConverter.Extensions
{
    [Export(typeof(Unit))]
    [ExportMetadata("UnitName", "km")]
    public class Kilometre:Unit
    {
        public Kilometre()
        {
            AsMM = 1000000;
        }
    }
}