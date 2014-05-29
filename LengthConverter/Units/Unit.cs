namespace LengthConverter.Units
{
    public abstract class Unit
    {
        protected double AsMM;

        //public abstract double FromMM();
        internal double ToMM(double toConvert)
        {
            return AsMM * toConvert;
        }

        internal double FromMM(double mm)
        {
            return mm/AsMM;
        }

        public double ConvertTo(double value, Unit to)
        {
            return to.FromMM(ToMM(value));

        }
    }

    public interface UnitData
    {
        string UnitName { get; }
    }
}
