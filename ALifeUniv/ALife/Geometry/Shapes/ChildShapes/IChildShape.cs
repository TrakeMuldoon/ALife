using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.UtilityClasses
{
    public interface IChildShape
    {
        IShape CloneChildShape(IShape parent);
    }
}
