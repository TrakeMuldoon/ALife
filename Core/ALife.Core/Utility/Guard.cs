using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ALife.Core.Utility
{
    /// <summary>
    ///     A thread-safe boolean guard/flag.
    /// </summary>
    [DebuggerDisplay("State = {Check}")]
    public struct Guard
    {
        /// <summary>
        ///     (Immutable)
        ///     The value for false.
        /// </summary>
        private const int False = 0;

        /// <summary>
        ///     (Immutable)
        ///     The value for true.
        /// </summary>
        private const int True = 1;

        /// <summary>
        ///     The current state.
        /// </summary>
        private int _state;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="_">    (Optional) True to. </param>
        public Guard(bool _ = true)
        {
            this._state = False;
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Guard" /> is set to True. NOTE: Will *not*
        ///     set it to true if it is currently set to False!
        /// </summary>
        /// <value>
        ///     <c>true</c> if check; otherwise, <c>false</c>.
        /// </value>
        public bool Check => this._state == True;

        /// <summary>
        ///     Gets a value indicating whether [check set].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [check set]; otherwise, <c>false</c>.
        /// </value>
        public bool CheckSet => Interlocked.Exchange(ref this._state, True) == False;

        /// <summary>
        ///     Mark the guard as being checked.
        /// </summary>
        public void MarkChecked()
        {
            Interlocked.Exchange(ref this._state, True);
        }

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        public void Reset()
        {
            Interlocked.Exchange(ref this._state, False);
        }
    }
}
