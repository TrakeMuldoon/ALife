namespace ALife.Core.WorldInfoObjects.Colours;

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
}

