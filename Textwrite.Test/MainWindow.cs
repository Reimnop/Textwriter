using System.Drawing;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SharpFont;
using Textwriter;

namespace Textwrite.Test;

public class MainWindow : GameWindow
{
    private int program;
    private int vao, vbo;
    private int texture;
    private int vertCount;

    private Library freetype;
    private Font font;
    
    public MainWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        int vert = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vert, File.ReadAllText("shader.vert"));
        GL.CompileShader(vert);

        int frag = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(frag, File.ReadAllText("shader.frag"));
        GL.CompileShader(frag);

        program = GL.CreateProgram();
        GL.AttachShader(program, vert);
        GL.AttachShader(program, frag);
        GL.LinkProgram(program);
        
        GL.DetachShader(program, vert);
        GL.DetachShader(program, frag);
        GL.DeleteShader(vert);
        GL.DeleteShader(frag);

        freetype = new Library();
        font = new Font(freetype, "Montserrat.ttf", 64, 3076, 3076);
        TextBuilder textBuilder = new TextBuilder(font)
            .AddText(new StyledText("Hello ").WithColor(Color.White))
            .AddText(new StyledText("world").WithColor(Color.Lime))
            .AddText(new StyledText("!").WithColor(Color.Red));
        TextVertex[] vertices = TextMeshGenerator.GenerateVertices(textBuilder.Build());
        vertCount = vertices.Length;

        GL.CreateBuffers(1, out vbo);
        GL.NamedBufferData(vbo, vertices.Length * Unsafe.SizeOf<TextVertex>(), vertices, BufferUsageHint.StaticDraw);
        
        GL.CreateVertexArrays(1, out vao);
        
        int stride = Unsafe.SizeOf<TextVertex>();
        
        GL.EnableVertexArrayAttrib(vao, 0);
        GL.VertexArrayVertexBuffer(vao, 0, vbo, new IntPtr(0), stride);
        GL.VertexArrayAttribFormat(vao, 0, 2, VertexAttribType.Float, false, 0);
        GL.VertexArrayAttribBinding(vao, 0, 0);
        
        GL.EnableVertexArrayAttrib(vao, 1);
        GL.VertexArrayVertexBuffer(vao, 1, vbo, new IntPtr(sizeof(float) * 2), stride);
        GL.VertexArrayAttribFormat(vao, 1, 2, VertexAttribType.Float, false, 0);
        GL.VertexArrayAttribBinding(vao, 1, 1);
        
        GL.EnableVertexArrayAttrib(vao, 2);
        GL.VertexArrayVertexBuffer(vao, 2, vbo, new IntPtr(sizeof(float) * 4), stride);
        GL.VertexArrayAttribFormat(vao, 2, 4, VertexAttribType.Float, false, 0);
        GL.VertexArrayAttribBinding(vao, 2, 2);
        
        GL.CreateTextures(TextureTarget.Texture2D, 1, out texture);
        GL.TextureStorage2D(texture, 1, SizedInternalFormat.R8, font.Atlas.Texture.Width, font.Atlas.Texture.Height);
        GL.TextureSubImage2D(texture, 0, 0, 0, font.Atlas.Texture.Width, font.Atlas.Texture.Height, PixelFormat.Red, PixelType.UnsignedByte, font.Atlas.Texture.Pixels);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        GL.Viewport(0, 0, Size.X, Size.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        Matrix4 projection = Matrix4.CreateOrthographic(Size.X, Size.Y, -8.0f, 8.0f);
        
        GL.UseProgram(program);
        GL.UniformMatrix4(0, true, ref projection);
        GL.Uniform1(1, 0);
        GL.BindTextureUnit(0, texture);
        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, vertCount);
        
        SwapBuffers();
    }
}