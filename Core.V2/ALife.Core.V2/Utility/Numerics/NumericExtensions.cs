using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALife.Core.Utility.Numerics
{
    /// <summary>
    /// Extensions for numeric utility classes.
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// Converts a BoundedManualNumber to a BoundedAutoNumber.
        /// </summary>
        /// <param name="BoundedManualNumber">The BoundedManualNumber to convert</param>
        /// <returns></returns>
        public static BoundedAutoNumber ToAutoBoundedNumber(this BoundedManualNumber number)
        {
            return new BoundedAutoNumber(number.Value, number.MinValue, number.MaxValue);
        }
        /// <summary>
        /// Converts a BoundedAutoNumber to a BoundedManualNumber.
        /// </summary>
        /// <param name="BoundedAutoNumber">The BoundedManualNumber to convert</param>
        /// <returns></returns>
        public static BoundedManualNumber ToAutoBoundedNumber(this BoundedAutoNumber number)
        {
            return new BoundedManualNumber(number.Value, number.MinValue, number.MaxValue);
        }
    }
}
