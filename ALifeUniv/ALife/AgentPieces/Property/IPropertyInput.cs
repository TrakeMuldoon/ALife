using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
