using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LengthConverter.Converter;

namespace LengthConverter.Web.Controllers
{
    public class ConverterController : Controller
    {
        private BasicConverter converter;

        public ConverterController()
        {
            converter = new BasicConverter();
        }
        
        public ActionResult Formats()
        {
            return Json(converter.AvailableUnits, JsonRequestBehavior.AllowGet);
        }

        public string Length(double length, string inputFormat, string outputFormat)
        {
            return InputParser.ConvertInput(string.Format("{0} {1} in {2}", length,inputFormat,outputFormat), converter);
        }

    }
}