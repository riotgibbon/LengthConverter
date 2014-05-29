using System.ComponentModel.Composition;

namespace LengthConverter.Units
{
    [Export(typeof(Unit))]
    [ExportMetadata("UnitName", "ft")]
    public class Feet: Unit
    {
        public Feet()
        {
            AsMM = 304.8;
        }
    }
}