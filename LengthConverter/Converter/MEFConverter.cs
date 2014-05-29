using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LengthConverter.Units;

namespace LengthConverter.Converter
{
    public class MEFConverter : BasicConverter
    {
        private CompositionContainer _container;
        [ImportMany]
        private IEnumerable<Lazy<Unit, UnitData>> units;

        public MEFConverter()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(MEFConverter).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog("."));
            if(Directory.Exists("Extensions")) catalog.Catalogs.Add(new DirectoryCatalog("Extensions"));

            _container = new CompositionContainer(catalog);

            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        public override string[] AvailableUnits
        {
            get
            {
                return units.Select(u => u.Metadata.UnitName).Distinct().ToArray();
            }
        }

        public override Unit GetUnit(string unitName)
        {
            return (from i in units where i.Metadata.UnitName.Equals(unitName) select i.Value).FirstOrDefault();
        }
    }
}
