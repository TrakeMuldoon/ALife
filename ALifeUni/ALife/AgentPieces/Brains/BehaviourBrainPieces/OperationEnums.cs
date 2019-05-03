using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.Brains.BehaviourBrainPieces
{
    public enum NumericalOperationEnum
    {
        GreaterThan,
        LessThan,
        EqualTo,
        NotEqualTo,
        LessThanOrEqualTo,
        GreaterThanOrEqualTo,
        IntegerEqual,
        DecimalEqual
    }

    public enum StringOperationEnum
    {
        EqualTo,
        NoEqualTo,
        Contains,
        DoesNotContain
    }

    public enum BoolOperationEnum
    {
        EqualTo,
        NoEqualTo,
        AND,
        OR,
        NAND,
        NOR,
        XOR,
        XNOR,
        IsTrue,
        IsFalse
    }
}
