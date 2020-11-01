using System;


namespace Slot.Model
{
    [Serializable]
    public class Balance
    {
        public decimal Conversion { get; set; }

        public decimal Credit { get; set; }

        public decimal Value { get; set; }

        public static Balance Create(decimal value)
        {
            return new Balance { Conversion = 1, Credit = value, Value = value };
        }

    }
}