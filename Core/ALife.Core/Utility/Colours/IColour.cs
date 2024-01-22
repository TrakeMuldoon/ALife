using System;

namespace ALife.Core.Utility.Colours
{
    /// <summary>
    /// Defines a colour with alpha, red, green and blue components.
    /// </summary>
    public interface IColour : IEquatable<IColour>
    {
        /// <summary>
        /// Gets alpha channel.
        /// </summary>
        /// <value>The alpha channel.</value>
        byte A { get; }

        /// <summary>
        /// Gets blue channel.
        /// </summary>
        /// <value>The blue channel.</value>
        byte B { get; }

        /// <summary>
        /// Gets green channel.
        /// </summary>
        /// <value>The green channel.</value>
        byte G { get; }

        /// <summary>
        /// Gets red channel.
        /// </summary>
        /// <value>The red channel.</value>
        byte R { get; }

        /// <summary>
        /// Gets a value indicating whether this colour [was predefined].
        /// </summary>
        /// <value><c>true</c> if [was predefined]; otherwise, <c>false</c>.</value>
        bool WasPredefined { get; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        IColour Clone();

        /// <summary>
        /// Creates a Colour object from the specified AHSL values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="lightness">The lightness.</param>
        /// <returns>The Colour object.</returns>
        IColour FromAHSL(byte alpha, int hue, double saturation, double lightness);

        /// <summary>
        /// Creates a Colour object from the specified AHSV values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="hue">The hue.</param>
        /// <param name="saturation">The saturation.</param>
        /// <param name="value">The value.</param>
        /// <returns>The Colour object.</returns>
        IColour FromAHSV(byte alpha, int hue, double saturation, double value);

        /// <summary>
        /// Creates a Colour object from the specified ARGB values.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        /// <returns>The Colour object.</returns>
        IColour FromARGB(byte alpha, byte red, byte green, byte blue);
    }
}
