using System;

namespace ALifeUni.ALife
{
    public class StatisticInput : Input<int>, IPropertyInput<int>
    {
        public int StatisticMaximum;
        public int StatisticMinimum;

        public StatisticInput(string name, int statisticMinimum, int statisticMaximum) : base(name)
        {
            StatisticMaximum = statisticMaximum;
            StatisticMinimum = statisticMinimum;
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
            Value = Math.Clamp(value, StatisticMinimum, StatisticMaximum);
            modified = true;
        }

        public override int Value
        {
            get { return base.Value; }
            set { base.Value = Math.Clamp(value, StatisticMinimum, StatisticMaximum); }

        }

        public Type GetMyType()
        {
            return typeof(int);
        }

        public StatisticInput CloneStatisticInput()
        {
            return new StatisticInput(Name, StatisticMinimum, StatisticMaximum);
        }

        public IPropertyInput Clone()
        {
            return CloneStatisticInput();
        }
    }
}
