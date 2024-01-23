using System;
using System.Runtime.InteropServices;
using ALife.Core.Utility;
using ALife.Core.Utility.Maths;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.Properties;

namespace ALife.Core.WorldObjects.Agents.Properties
{
    public enum StatisticInputType
    {
        Incrementing,
        Decrementing,
        Manual
    }

    public class StatisticInput : Input<int>, IPropertyInput<int>
    {
        public int StatisticMaximum;
        public int StatisticMinimum;
        public StatisticInputType Disposition;
        public int StartValue;

        public StatisticInput(string name, int statisticMinimum, int statisticMaximum, [Optional] int startValue) 
            : this(name, statisticMinimum, statisticMaximum, StatisticInputType.Manual, startValue)
        {
        }

        public StatisticInput(string name, int statisticMinimum, int statisticMaximum, StatisticInputType disposition, [Optional] int startValue) : base(name)
        {
            StatisticMaximum = statisticMaximum;
            StatisticMinimum = statisticMinimum;
            Disposition = disposition;
            StartValue = startValue;
            Value = startValue;
        }

        public int GetValue()
        {
            return Value;
        }

        public void IncreasePropertyBy(int value)
        {
            if(value < 0)
            {
                throw new Exception("Negative Value for 'increase property'");
            }
            if(value == 0)
            {
                return;
            }
            int temp = Value + value;
            if(temp > StatisticMaximum)
            {
                temp = StatisticMaximum;
            }

            Value = temp;
            modified = true;
        }

        public void DecreasePropertyBy(int value)
        {
            if(value < 0)
            {
                throw new Exception("Negative Value for 'decrease property'");
            }
            if(value == 0)
            {
                return;
            }
            int temp = Value - value;
            if(temp < StatisticMinimum)
            {
                temp = StatisticMinimum;
            }

            Value = temp;
            modified = true;
        }

        public void ChangePropertyTo(int value)
        {
            Value = ExtraMaths.Clamp(value, StatisticMinimum, StatisticMaximum);
            modified = true;
        }

        public override int Value
        {
            get { return base.Value; }
            set { base.Value = ExtraMaths.Clamp(value, StatisticMinimum, StatisticMaximum); }

        }

        public Type GetMyType()
        {
            return typeof(int);
        }

        public StatisticInput CloneStatisticInput()
        {
            return new StatisticInput(Name, StatisticMinimum, StatisticMaximum, Disposition, StartValue);
        }

        public IPropertyInput Clone()
        {
            return CloneStatisticInput();
        }
    }
}
