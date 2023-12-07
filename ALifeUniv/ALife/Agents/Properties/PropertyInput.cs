using System;

namespace ALifeUni.ALife.Agents.Properties
{
    public class PropertyInput : Input<double>, IPropertyInput<double>
    {
        public double PropertyMaximum;
        public double PropertyMinimum;

        public PropertyInput(string name) : this(name, 0, 1.0)
        {
        }

        public PropertyInput(string name, double propertyMinimum, double propertyMaximum) : base(name)
        {
            PropertyMaximum = propertyMaximum;
            PropertyMinimum = propertyMinimum;
        }

        public double GetValue()
        {
            return Value;
        }

        public void IncreasePropertyBy(double value)
        {
            if(value < 0)
            {
                throw new Exception("Negative Value for 'increase property'");
            }
            if(value == 0)
            {
                return;
            }
            double temp = Value + value;
            if(temp > PropertyMaximum)
            {
                temp = PropertyMaximum;
            }

            Value = temp;
            modified = true;
        }

        public void DecreasePropertyBy(double value)
        {
            if(value < 0)
            {
                throw new Exception("Negative Value for 'decrease property'");
            }
            if(value == 0)
            {
                return;
            }
            double temp = Value - value;
            if(temp < PropertyMinimum)
            {
                temp = PropertyMinimum;
            }

            Value = temp;
            modified = true;
        }

        public void ChangePropertyTo(double value)
        {
            Value = Math.Clamp(value, PropertyMinimum, PropertyMaximum);
            modified = true;
        }

        public override double Value
        {
            get { return base.Value; }
            set { base.Value = Math.Clamp(value, PropertyMinimum, PropertyMinimum); }

        }

        public Type GetMyType()
        {
            return typeof(double);
        }

        public PropertyInput ClonePropertyInput()
        {
            return new PropertyInput(Name, PropertyMinimum, PropertyMaximum);
        }

        public IPropertyInput Clone()
        {
            return ClonePropertyInput();
        }

    }
}
