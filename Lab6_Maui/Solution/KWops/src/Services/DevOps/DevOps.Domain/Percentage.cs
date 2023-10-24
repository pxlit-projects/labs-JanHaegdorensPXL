using Domain;

namespace DevOps.Domain
{
    public class Percentage : ValueObject<Percentage>
    {
        private readonly double _value;

        public Percentage(double value)
        {
            if (value < 0 || value > 1)
            {
                throw new ContractException("Bad input");
            }

            _value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _value;
        }

        public static implicit operator string(Percentage v)
        {
            return $"{ Math.Round(v._value*100, 2)}%";
        }
        public static implicit operator double(Percentage v)
        {
            return v._value;
        }
        public static implicit operator Percentage(double value)
        {
            return new Percentage(value);
        }
        public override string ToString()
        {
            return $"{ Math.Round(_value*100, 2)}%";
        }

    }
}