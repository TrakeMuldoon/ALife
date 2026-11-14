using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace ALife.Spatial.Demo;

/// <summary>
/// Draws textured, tinted rectangles in screen pixels, in the order they're added, with far less work per rectangle than a
/// <see cref="SpriteBatch"/>: no sorting, no per-sprite transforms, and big runs of them can be written from several threads
/// at once through <see cref="Reserve"/>. Solid rectangles use the middle of the texture, which has to be opaque white there.
/// Everything is uploaded to the GPU in one go when flushed, at 16 bytes a corner.
/// </summary>
internal sealed class QuadBatch : IDisposable
{
    // The most rectangles per draw call, so their vertices can be numbered with 16-bit indices. Bigger batches share the
    // one index buffer, each chunk's numbers counted from its first vertex.
    private const int ChunkQuads = 16384;

    private static readonly NormalizedShort2 TopLeft = new(0, 0);
    private static readonly NormalizedShort2 TopRight = new(1, 0);
    private static readonly NormalizedShort2 BottomLeft = new(0, 1);
    private static readonly NormalizedShort2 BottomRight = new(1, 1);
    private static readonly NormalizedShort2 Middle = new(0.5f, 0.5f);

    private readonly GraphicsDevice _device;
    private readonly BasicEffect _effect;
    private readonly IndexBuffer _indices;
    private DynamicVertexBuffer? _buffer;
    private Vertex[] _vertices = new Vertex[ChunkQuads * 4];
    private int _count;

    /// <summary>Creates a batch that draws with a texture, premultiplied.</summary>
    public QuadBatch(GraphicsDevice device, Texture2D texture)
    {
        _device = device;
        _effect = new BasicEffect(device) { TextureEnabled = true, VertexColorEnabled = true, Texture = texture };
        short[] indices = new short[ChunkQuads * 6];
        for (int quad = 0; quad < ChunkQuads; quad++)
        {
            int index = quad * 6, vertex = quad * 4;
            indices[index] = (short)vertex;
            indices[index + 1] = (short)(vertex + 1);
            indices[index + 2] = (short)(vertex + 2);
            indices[index + 3] = (short)(vertex + 1);
            indices[index + 4] = (short)(vertex + 3);
            indices[index + 5] = (short)(vertex + 2);
        }

        _indices = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
        _indices.SetData(indices);
    }

    /// <summary>The vertices, four per rectangle, for writing the rectangles <see cref="Reserve"/> handed out.</summary>
    public Vertex[] Vertices => _vertices;

    /// <summary>Adds the whole texture stretched over a rectangle.</summary>
    public void AddSprite(float left, float top, float right, float bottom, Color color)
    {
        // Reserved first: it can swap the vertex array for a bigger one.
        int quad = Reserve(1);
        WriteSprite(_vertices, quad, left, top, right, bottom, color);
    }

    /// <summary>Adds a square turned about its center, showing a column of the texture, as <see cref="WriteRotated"/>.</summary>
    public void AddRotated(float x, float y, float halfSize, float cos, float sin, Color color, float textureLeft, float textureRight)
    {
        int quad = Reserve(1);
        WriteRotated(_vertices, quad, x, y, halfSize, cos, sin, color, textureLeft, textureRight);
    }

    /// <summary>Adds a solid rectangle.</summary>
    public void AddSolid(float left, float top, float right, float bottom, Color color)
    {
        int quad = Reserve(1);
        WriteSolid(_vertices, quad, left, top, right, bottom, color);
    }

    /// <summary>Adds a solid outline drawn inside a rectangle, as four rectangles.</summary>
    public void AddOutline(float left, float top, float right, float bottom, float thickness, Color color)
    {
        int quad = Reserve(4);
        Vertex[] vertices = _vertices;
        WriteSolid(vertices, quad, left, top, right, top + thickness, color);
        WriteSolid(vertices, quad + 1, left, bottom - thickness, right, bottom, color);
        WriteSolid(vertices, quad + 2, left, top, left + thickness, bottom, color);
        WriteSolid(vertices, quad + 3, right - thickness, top, right, bottom, color);
    }

    /// <summary>Makes room for a number of rectangles and returns the first one's number, for <see cref="WriteSprite"/>.</summary>
    public int Reserve(int quads)
    {
        int needed = (_count + quads) * 4;
        if (needed > _vertices.Length)
        {
            Array.Resize(ref _vertices, Math.Max(needed, _vertices.Length * 2));
        }

        int first = _count;
        _count += quads;
        return first;
    }

    /// <summary>Writes the whole texture stretched over a rectangle into a reserved slot.</summary>
    public static void WriteSprite(Vertex[] vertices, int quad, float left, float top, float right, float bottom, Color color)
    {
        int vertex = quad * 4;
        vertices[vertex] = new Vertex(left, top, color, TopLeft);
        vertices[vertex + 1] = new Vertex(right, top, color, TopRight);
        vertices[vertex + 2] = new Vertex(left, bottom, color, BottomLeft);
        vertices[vertex + 3] = new Vertex(right, bottom, color, BottomRight);
    }

    /// <summary>
    /// Writes a square turned about its center into a reserved slot, showing the texture from <paramref name="textureLeft"/>
    /// to <paramref name="textureRight"/> across and all of it down: one cell of a row of pictures. A positive angle, given by
    /// its cosine and sine, turns it clockwise on screen.
    /// </summary>
    public static void WriteRotated(
        Vertex[] vertices, int quad, float x, float y, float halfSize, float cos, float sin, Color color, float textureLeft, float textureRight)
    {
        float a = cos * halfSize, b = sin * halfSize;
        int vertex = quad * 4;
        vertices[vertex] = new Vertex(x - a + b, y - b - a, color, new NormalizedShort2(textureLeft, 0));
        vertices[vertex + 1] = new Vertex(x + a + b, y + b - a, color, new NormalizedShort2(textureRight, 0));
        vertices[vertex + 2] = new Vertex(x - a - b, y - b + a, color, new NormalizedShort2(textureLeft, 1));
        vertices[vertex + 3] = new Vertex(x + a - b, y + b + a, color, new NormalizedShort2(textureRight, 1));
    }

    private static void WriteSolid(Vertex[] vertices, int quad, float left, float top, float right, float bottom, Color color)
    {
        int vertex = quad * 4;
        vertices[vertex] = new Vertex(left, top, color, Middle);
        vertices[vertex + 1] = new Vertex(right, top, color, Middle);
        vertices[vertex + 2] = new Vertex(left, bottom, color, Middle);
        vertices[vertex + 3] = new Vertex(right, bottom, color, Middle);
    }

    /// <summary>Draws everything added since the last flush, then starts again.</summary>
    public void Flush()
    {
        if (_count == 0)
        {
            return;
        }

        // The buffer is sized to the vertex array, so it only grows when that does. Discarding hands the old contents back
        // to the driver rather than waiting for the GPU to finish with them.
        if (_buffer is null || _buffer.VertexCount < _vertices.Length)
        {
            _buffer?.Dispose();
            _buffer = new DynamicVertexBuffer(_device, Vertex.Declaration, _vertices.Length, BufferUsage.WriteOnly);
        }

        _buffer.SetData(_vertices, 0, _count * 4, SetDataOptions.Discard);

        Viewport viewport = _device.Viewport;
        _effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
        _device.BlendState = BlendState.AlphaBlend;
        _device.SamplerStates[0] = SamplerState.LinearClamp;
        _device.DepthStencilState = DepthStencilState.None;
        _device.RasterizerState = RasterizerState.CullNone;
        _device.SetVertexBuffer(_buffer);
        _device.Indices = _indices;
        _effect.CurrentTechnique.Passes[0].Apply();
        for (int first = 0; first < _count; first += ChunkQuads)
        {
            int quads = Math.Min(ChunkQuads, _count - first);
            _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, first * 4, 0, quads * 2);
        }

        _count = 0;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _effect.Dispose();
        _indices.Dispose();
        _buffer?.Dispose();
    }

    /// <summary>
    /// A corner of a rectangle: a position in pixels, a tint and a texture coordinate, in 16 bytes rather than the 24 of a
    /// <see cref="VertexPositionColorTexture"/>, since there's no depth and the texture coordinates are all between 0 and 1.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Vertex(float x, float y, Color color, NormalizedShort2 textureCoordinate) : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new(
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(12, VertexElementFormat.NormalizedShort2, VertexElementUsage.TextureCoordinate, 0));

        private readonly Vector2 _position = new(x, y);
        private readonly Color _color = color;
        private readonly NormalizedShort2 _textureCoordinate = textureCoordinate;

        VertexDeclaration IVertexType.VertexDeclaration => Declaration;
    }
}
