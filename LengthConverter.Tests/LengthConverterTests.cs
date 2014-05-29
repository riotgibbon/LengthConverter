using System;
using LengthConverter.Converter;
using LengthConverter.Units;
using NUnit.Core;
using NUnit.Framework;


namespace LengthConverter.Tests
{
    /*Normally, I try my best to avoid writing comments in code, as they often create a dependency. I try to
     * have self-describing code, with tests and descriptive variable/method names. I've put comments in to describe 
     * my process in more detail just this once
     * 
     * The problem describes converting lengths in one of 5 different units to another
     * As this creates 20 possible permutations (+5 for converting to self) , with a real combinatorial explosion if more are
     * added, clearly an elegant solution is called for, but this TDD process will start from the most
     * naive, 2 unit scenario in the specification, and build up - this will allow us to see
     * if any simple solution that satisfies the conditions is available before diving into something more complicated,
     * which is thus harder to maintain
     * 
     * We won't even think about the UI/command line parsing yet - that's a solved problem, we are thinking about length
     * conversion, and we want a nice solution that we can just plug our converter into, be it command line, 
     * unit tests, REST, Thrift, Avro - the list is endless, but the  principle is  the  same 
     * 
 
     */

    public class LengthConverterTests
    {
        

        //First test, before writing any code, to make sure that the test environment is working properly
        //This test would normally be deleted after confirming test framework is active.
        [Test]
        public void FrameworkTest()
        {
            Assert.IsTrue(true);
        }

        /*
         * Second test, using the example from spec. Using a "BasicConverter" class. Answer is hard coded to "return 0.1", 
         * you can enter anything and will get this result, on the "only write the code you need" XP principle 
         * First pass of this test had all the code in place, was refactored to resuable TestConversion method after passing
         * refactoring - all variables explicitly defined, ExtractMethod called, then all variables inlined
         */

        [Test]
        public void Convert_CMtoM_1()
            //test names can be creative, illustrating the ArrangeActAssert process, but this is being kept simple
        {
            TestConversion("cm", "m", 10, 0.1);
        }

        //utility method left here to demonstrate order of events, would normally be moved somewhere else
        private static void TestConversion(string inputFormat, string outputFormat, double inputLength, double expected)
        {
            var converter = new BasicConverter();
            TestConversion(inputFormat, outputFormat, inputLength, expected, converter);
        }

        private static void TestConversion(string inputFormat, string outputFormat, double inputLength, double expected,
            BasicConverter converter)
        {
            var converted = converter.ConvertLength(inputFormat, outputFormat, inputLength);
                //this could be just one parameter object
            Assert.That(expected, Is.EqualTo(converted).Within(0.01));
                //asserting equal on doubles needs a bit of precision management
        }

        /* next test still uses same units, but different values to force some sort of actual conversion work - triangulation
         * now code is "return inputLength/100";
         * This is now a fully functioning cm to m converter!         */

        [Test]
        public void Convert_CMtoM_2()
        {
            TestConversion("cm", "m", 25, 0.25); //in real tests, this would be the next line of Convert_CMtoM_1, to avoid clutter
        }

        /*Time for a new conversion, how about m>cm
         * As only 2 units are being used, then we can get away with this:
          
         * switch (inputFormat + outputFormat)
            {
                case "cmm":
                    return inputLength / 100;
                case "mcm":
                    return inputLength * 100;
            }             
         */
        [Test]
        public void Convert_MtoCM_1()
        {
            TestConversion("m", "cm", .5, 50);
        }

        /*But once we start thinking about extra units, and the user who wants to convert the same unit to itself
         * just to see what happens, then we start to enter combinatorial explosion, and we need to think about 
         * making this code more flexible. As (the first) users are entering text via a command line, there is also a
         * strong possibility of error to manage. That can come later though
         * 
         * If we were only managing metric units, then we could still keep this fairly simple, but as there 
         * already imperial (inches, feet and yards), and experience tells us that miles, km and even cubits are just
         * a feature request away, we should take the plunge now
         * 
         * So, we know that we can convert any length dimension to and from any other, as they all describe the same physical
         * characteristic. The basic pattern that springs to mind is to convert input lengths to a common format (eg mm), 
         * then convert out to the output format. This means we will only require one conversion rate per unit, 
         * eg however many mm there are in a centimetre, metre or whatever. This is potentially less efficient than having a 
         * conversion rate for every combination of units, as there needs to be 2 calculations, not just one, but it is a lot
         * simpler to add new units. Each unit only has to know about one other format, mm, for it to be converted to 
         * every other format.
         * 
         * This means we can have an object (Unit?) that describes the measurement type, and how to convert its units to the standard format
         * 
         * A design decision to take here is to either orchestrate the entire conversion within the existing "ConvertLength" 
         * method, or to pass one of the units to the other and let them manage that internally. We will continue on our existing 
         * path for now with just our 2 "Centimetre" and "M" objects for now, 
         * and see if there is a pressing need to make the objects more autonomous as we add more later
         * 
         * As the user will supply the input formats by name, then this seems a likely candidate for the  Factory Pattern.
         * If all our Unit objects inherit a common base/implement interface,  then the required implementation can be 
         * generated by the Factory - the client code will not know the identity of each Unit, but know what to do with them
         * 
         * So, our Unit objects need to be able to convert a supplied value to and from mm. This means that they need to know 
         * a constant that describes this relationship, and being able to perform that calculation would be good to, suggesting
         * Unit be an abstract base class that has a constant that needs to be defined in each implementation, and
         * ToMM and FromMM methods that perform the calculations

         * To start, lets create the first Unit class, Centimetre, and test 
         * the conversion of cm to mm and back just in the new Centimetre object to see what feels best.
         * 
         * Centimetre starts just as simple class with no inheritance. Once we are happy with our pattern, then we can 
         * extract the required common functionality in the way that makes sense
         */ 
        [Test]
        public void Convert_CMtoMM_in_object()
        {
            var cm = new Centimetre();
            Assert.AreEqual(10,cm.ToMM(1.0));
        }
        [Test]
        public void Convert_MMtoCM_in_object()
        {
            var cm = new Centimetre();
            Assert.AreEqual(25, cm.FromMM(250));
        }

        /*extract the expected common functionality to the Unit base class, 
         * using "Pull Members Up" refactoring and re-run tests
         * 
         * Centimetre object now only has the differing  "AsMM" protected variable,used in ToMM and FromMM. This has to be set, 
         * probably in the constructor. This  means you can compile the class with this required mm conversion missing. This
         * is  OK for this application, but you  would probably  want to be a bit  stricter if you were making this more extensible
         * 
         * 
         * Specifying "AsMM" and "ToMM" actually in the public methods feels something of a blunt instrument. It feels like 
         * we are exposing too many implementation details.
         * Maybe the ConvertTo pattern, where you pass in the target Unit object does make sense. We can still test just with 
         * Centimetres, as the result will be the same as in the input. We will only know for sure when we introduce different 
         * unit types though. The ConvertTo method will still use the AsMM and ToMM methods, but we make them "internal" instead
         * 
         * So we don't break the previous tests, and allow us to do unit testing  on subsequent Unit implementations, we can
         * make the internals of the  LengthConverter class library available to our tests with this addition to AssemblyInfo.cs
         * [assembly: InternalsVisibleTo("LengthConverter.Tests")]
         * 
         * This might not be a great idea in production though
         * 
         */

        [Test]
        public void Convert_CMtoCM_WithConvertTo()
        {
            Unit from = new Centimetre();
            Unit to = new Centimetre();
            double value = 3.142;
            var converted = from.ConvertTo(value, to);
            Assert.AreEqual(value, converted);
        }

        /*Now that we have a working method on a base class, lets create a Factory method to create the Unit objects
         * and do cm>cm conversion. We are going to explicitly link our names and implementations in a method, which is 
         * the  quickest way  to go now, but requires recomplilation if we wish to add new units. We may return to this once the 
         * core, specified application is working
         * 
         * public static Unit GetUnit(string unitName)
        {
            switch (unitName)
            {
                case "cm":
                    return new Centimetre();
                
         */

        [Test]
        public void Create_CM_Unit_Object()
        {
            Test_UnitFactory<Centimetre>("cm");
        }

        private static void Test_UnitFactory<T>(string unitName)
        {
            var converter = new BasicConverter();
            Unit unit = converter.GetUnit(unitName);
            Assert.IsInstanceOf<T>(unit);
        }

        /*
         * Now that we can do cm>cm conversions with a Factory method, lets plug that into the "cmcm" switch case 
         * (we'll come to the others in a minute).
         * 
         *  switch (inputFormat + outputFormat)
            {
                case "cmcm":
                    Unit from = GetUnit(inputFormat);
                    Unit to = GetUnit(outputFormat);
                    return from.ConvertTo(inputLength, to);
                case "cmm":
                    return inputLength / 100;
                case "mcm":
                    return inputLength * 100;
            }
         * 
         */
        [Test]
        public void Convert_CMtoCM_1()
        {
            TestConversion("cm", "cm", 1.618, 1.618);
        }

        //Now that we are happy that we have a working naive implementation, we can implement Metre as a Unit
        [Test]
        public void Create_Metre_Unit_Object()
        {
            Test_UnitFactory<Metre>("m");
        }

        /*
         * Test the m>mm conversions
         */

        [Test]
        public void Unit_Metre_MM_tests()
        {
            var m = new Metre();
            Assert.AreEqual(4500, m.ToMM(4.5));
            Assert.AreEqual(3.6, m.FromMM(3600));
        }

        //remove the switch condition in ConvertLength and test cm/m conversion - just a refactoring, no need for new tests

        /*
         * New unit time - inches first, with 25.4 mm in an inch. At this point, the combinations begin to expand, and unit tests
         * start to become a bit unwieldy, and a specification table as found in Fitnesse, Gherkin-based languages, or Acceptance tests
         *
         * These are good for when you have to generate a large number of combinations, which are cumbersome to manage in unit tests
         * 
         * Expected conversion data from the Google Search converter
         */

        [Test]
        public void Unit_Inch_tests()
        {
            Unit_Test_Unit<Inch>("in", 25.4, 1.9685);
        }

        //refactor to a common set of tests per unit (create, basic toMM test, convert to 5cm)
        public void Unit_Test_Unit<T>(string unitName , double convertedToMM, double convertedTo5CM )
        {
            var converter = new BasicConverter();
            Unit unit = converter.GetUnit(unitName);
            Assert.IsInstanceOf<T>(unit);
            Assert.AreEqual(convertedToMM, unit.ToMM(1));
            Assert.AreEqual(1, unit.FromMM(convertedToMM));
            TestConversion("cm", unitName, 5, convertedTo5CM);

        }

        // Tests for feet and yards
        [Test]
        public void Unit_Feet_tests()
        {
            Unit_Test_Unit<Feet>("ft", 304.8, 0.16404);
        }

        [Test]
        public void Unit_yards_tests()
        {
            Unit_Test_Unit<Yard>("yd", 914.4, 0.05468);
        }

        //since we now know the basic conversions between each unit to mm and 5cm  work, lets throw in a few other combinations, just to make sure
        [Test]
        public void Convert_Various_Units()
        {
            TestConversion("in", "ft", 12, 1);
            TestConversion("yd", "ft", 1, 3);
            TestConversion("yd", "in", 1, 36);
            TestConversion("m", "cm", 1, 100);
            TestConversion("in", "cm", 12, 30.48);

            TestConversion("in", "yd", 25, 0.694444);
            TestConversion("ft", "m", 767, 233.782);
            TestConversion("yd", "in", 45.42, 1635.12);
            TestConversion("m", "yd", 21.87, 23.917323);
        }

        //now that we know it works properly with the expected units, lets see what it does when you hand it an invalid unit format. Fixed with bounds checking
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), ExpectedMessage = "Unknown input format", MatchType = MessageMatch.Contains)]
        public void Invalid_InputFormat_ThrowException()
        {
            TestConversion("foo", "yd", 25, 0.694444);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), ExpectedMessage = "Unknown output format", MatchType = MessageMatch.Contains)]
        public void Invalid_OutputFormat_ThrowException()
        {
            TestConversion("cm", "bar", 25, 0.694444);
        }

        /*now that we have a working core library and the first interface, the tests,
         * we can now implement another interface, the commmand line UI
         * 
         * The first implementation is a naive version, accepting the incoming arguments as valid and returning the expected result:
         * 
         *                 
                var converter = new BasicConverter();
                var inputLength = double.Parse(args[0]);
                var inputFormat = args[1];
                var outputFormat = args[3];
                var converted = converter.ConvertLength(inputFormat, outputFormat, inputLength);
                Console.WriteLine("{0} {1} equals {2} {3}", inputLength, inputFormat, converted, outputFormat);
         * 
         * However, this depends on the user getting everything right, which is possible, but maybe unlikely. So lets build an
         * arguments parser for the UI to use. As all we are required to do is return a line of text for the specified inputs, then
         * we will just have one method that takes the args[] string array that the "main" method is passed, and returns a string for the
         * console to write out. There are other options, but this will suit our specified purpose.
         * 
         */

        [Test]
        public void Parser_HappyPath()
        {
            TestInputParsing("10 cm in m", "10 cm equals 0.1 m");
            TestInputParsing("12 in in ft", "12 in equals 1 ft");
        }

        private static void TestInputParsing(string input, string expected)
        {
            string[] args = input.Split(' ');
            TestInputParsingWithArrayArgs(args, expected);
        }

        private static void TestInputParsingWithArrayArgs(string[] args, string expected)
        {
            var converter = new BasicConverter();
            string result = InputParser.ConvertInput(args, converter);
            Assert.That(expected, Is.EqualTo(result));
        }

        //return standard error message if incorrect number of arguments are not supplied - has to be 4. 
        //Could be more sophisticated, this will cover a lot of scenarios
        private const string correctFormat = "Please use format '<length> <unit> in <unit>'";
        [Test]
        public void Parser_NoArguments()
        {
            TestInputParsing("", correctFormat);
            TestInputParsingWithArrayArgs(null, correctFormat);
        }
        // test if the number supplied at the start is valid. Return error message if not, explaining what the error was
        [Test]
        public void Parser_InvalidNumber()
        {
            TestInputParsing("ten cm in m", "'ten' is not a valid number. " + correctFormat);
        }

        private const string correctUnits = "Available units are 'm', 'cm', 'in', 'ft', 'yd'";
        //test to see if the input unit is valid. Get valid units from converter    
        [Test]
        public void Parser_InvalidInputUnit()
        {
            TestInputParsing("2 km in ft", "'km' is not a valid unit. " + correctUnits);
        }

        [Test]
        public void Parser_InvalidOutputUnit()
        {
            TestInputParsing("2 m in mm", "'mm' is not a valid unit. " + correctUnits);
        }

        /*The third word, the command/instruction is supposed to be 'in'. 
         * At the moment, it doesn't make much difference if it's not
         * but we should check for this, as future versions may allow different versions
         * 
         * We can extend this code to define the commands in the Converter, but we can hard-code "in" for now
         */
        [Test]
        public void Parser_InvalidInstruction()
        {
            TestInputParsing("14 cm as m", "Incorrect command, expected 'in'. " + correctFormat);
        }

        //This completes the input parsing sections.



        /* We know that our users are going to want add new unit types. It's inevitable.
         * 
         * What we don't want to be doing is rebuilding our application every time a new one is required
         * 
         * The Managed Extensibility Framework comes to the rescue here. It allows us to dynamically import code on
         * the fly, based on interfaces and metadata. We annotate the classes we wish to import:
         *    
                [Export(typeof(Unit))]
                [ExportMetadata("UnitName", "m")]
                public class Metre: Unit {
         * 
         * In our new MEFConverter class, inheriting from BasicConverter, we build and compose the components we wish to use 
         * 
         * 
                var catalog = new AggregateCatalog();
                catalog.Catalogs.Add(new AssemblyCatalog(typeof(MEFConverter).Assembly));
                this._container.ComposeParts(this)
         * 
         * We are inheriting from BasicConverter because we wish to reuse the application logic in ConvertLength, 
         * we only want to change the way that units are discovered - in BasicConverter, we use a hard-coded
         * factory method. In MEFConverter, we override this method to use MEF:
                public override Unit GetUnit(string unitName)
                {
                    return (from i in units where i.Metadata.UnitName.Equals(unitName) select i.Value).FirstOrDefault();
                }
         * 
         * This is the equivalent of a factory  method, but looking in the metadata for the matching unit name (eg "m")
         * 
         * Marking all the existing Units with appropriate MEF metadata will load them dynamically
         */

        [Test]
        public void Convert_StandardUnits_Using_MEF()
        {
            TestMEFConversion("cm", "m", 10, 0.1);
            TestMEFConversion("in", "ft", 12, 1);
            TestMEFConversion("yd", "ft", 1, 3);
            TestMEFConversion("yd", "in", 1, 36);
            TestMEFConversion("m", "cm", 1, 100);
            TestMEFConversion("in", "cm", 12, 30.48);
        }


        private static void TestMEFConversion(string inputFormat, string outputFormat, double inputLength, double expected)
        {
            TestConversion(inputFormat, outputFormat, inputLength, expected, new MEFConverter());
        }


        /*
         * Now that we can load dynamically, we can add new independent components, allowing us to add new Units
         * without rebuilding the core components
         * 
         * LengthConverter.Extensions contains new Units, defined by their unitName and number of mm per unit
         * 
         * This assembly is loaded by using a DirectoryCatalog:
                catalog.Catalogs.Add(new DirectoryCatalog("Extensions"));
         * This would normally be an "Extensions" subfolder, or similar. The assembly containing the extensions
         * must have a reference to the core Units object (or similar interface). We are doing a post-build XCOPY
         * to move the Extensions assembly to the "Extensions" subfolder, but it is all independently deployable.
         * 
         * extension units:
         * km = kilometre
         * mi = mile
         * mm = millimetre
         * cu = cubit
         * my = megalithic yard
         */
        [Test]
        public void Convert_ExtensionUnits_Using_MEF()
        {
            TestMEFConversion("km", "m", 1, 1000);
            TestMEFConversion("mi", "mm", 1, 1609344);
            TestMEFConversion("mm", "cm", 10, 1);
            TestMEFConversion("cu", "mm", 1, 457.2);
            TestMEFConversion("my", "mm", 1, 829.66);
        }

        [Test]
        public void MEF_UnitTypes()
        {
            var converter = new MEFConverter();
            var units = converter.AvailableUnits;
            Assert.That(10, Is.EqualTo(units.Length));
            
        }
      
    }
}
