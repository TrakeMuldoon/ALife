using System;

namespace ALifeUni.ALife
{
    public interface IPropertyInput
    {
        Type GetMyType();
    }

    public interface IPropertyInput<T> : IPropertyInput
    {
        T GetValue();

        void IncreasePropertyBy(T value);
        void DecreasePropertyBy(T value);
        void ChangePropertyTo(T value);
    }
}
