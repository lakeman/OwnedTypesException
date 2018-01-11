using System;

namespace OwnedTypes
{
    public class EnumWrapper<T> where T : struct
    {
        public EnumWrapper() : this(Default)
        {
        }
        public EnumWrapper(T value)
        {
            this.Value = value;
        }

        private static T Default = (T)Enum.GetValues(typeof(T)).GetValue(0);

        public T Value { get; set; }

        public string _Raw {
            get {
                return Value.ToString();
            } set
            {
                if (string.IsNullOrWhiteSpace(value))
                    Value = Default;
                else
                    Value = (T)Enum.Parse(typeof(T), value, true);
            }
        }
        public override string ToString()
        {
            return $"{Value}";
        }

        public static implicit operator T? (EnumWrapper<T> value) => value?.Value;
        public static implicit operator T(EnumWrapper<T> value) => value.Value;

        public static implicit operator EnumWrapper<T>(T value) => new EnumWrapper<T> { Value = value };

        public static implicit operator string(EnumWrapper<T> value) => value._Raw;
        public static implicit operator EnumWrapper<T>(string value) => new EnumWrapper<T> { _Raw = value };

    }
}
