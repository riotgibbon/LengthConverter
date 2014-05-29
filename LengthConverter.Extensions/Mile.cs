using System.ComponentModel.Composition;
using LengthConverter.Units;

namespace LengthConverter.Extensions
{
    [Export(typeof(Unit))]
    [ExportMetadata("UnitName", "mi")]
    public class Mile: Unit
    {
        public Mile()
        {
            AsMM = 1609344;
        }
    }
}