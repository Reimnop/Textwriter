using System.Numerics;
using System.Runtime.InteropServices;

namespace Textwriter;

[StructLayout(LayoutKind.Sequential)]
public struct TextVertex
{
    public Vector2 Position;
    public Vector2 Uv;
    public Vector4 Color;

    public TextVertex(float x, float y, float u, float v, float r, float g, float b, float a)
    {
        Position = new Vector2(x, y);
        Uv = new Vector2(u, v);
        Color = new Vector4(r, g, b, a);
    }
}