using System.ComponentModel.Composition;
using LengthConverter.Units;

namespace LengthConverter.Extensions
{
    [Export(typeof(Unit))]
    [ExportMetadata("UnitName", "my")]
    public class MegalithicYard:Unit
    {
        public MegalithicYard()
        {
            AsMM = 829.66;
        }
    }
}