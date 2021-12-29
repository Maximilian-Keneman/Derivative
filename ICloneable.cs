namespace System
{
    public interface ICloneable<T> : ICloneable
    {
        public new T Clone();
    }
}