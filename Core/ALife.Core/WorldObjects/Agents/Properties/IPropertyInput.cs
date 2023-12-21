using System;
using ALife.Core.WorldObjects.Agents.Properties;

namespace ALife.Core.WorldObjects.Agents.Properties
{
    public interface IPropertyInput
    {
        Type GetMyType();

        IPropertyInput Clone();
    }

    public interface IPropertyInput<T> : IPropertyInput
    {
        T GetValue();

        void IncreasePropertyBy(T value);
        void DecreasePropertyBy(T value);
        void ChangePropertyTo(T value);
    }
}
