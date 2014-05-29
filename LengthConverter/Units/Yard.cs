using System.ComponentModel.Composition;

namespace LengthConverter.Units
{
    [Export(typeof(Unit))]
    [ExportMetadata("UnitName", "yd")]
    public class Yard:Unit
    {
        public Yard()
        {
            AsMM = 914.4;
        }
    }
}