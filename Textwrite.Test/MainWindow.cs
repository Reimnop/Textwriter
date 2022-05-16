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
    private int[] textures = new int[3];
    private int vertCount;

    private Library freetype;
    private Font fontRegular;
    private Font fontBold;
    private Font fontItalic;
    
    public MainWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        int vert = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vert, File.ReadAllText("shader.vert"));
        GL.CompileShader(vert);
        
        GL.GetShader(vert, ShaderParameter.CompileStatus, out int vertCode);
        if (vertCode != (int)All.True)
        {
            string infoLog = GL.GetShaderInfoLog(vert);
            throw new Exception($"Error occurred whilst compiling Shader({vert}).\n\n{infoLog}");
        }

        int frag = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(frag, File.ReadAllText("shader.frag"));
        GL.CompileShader(frag);
        
        GL.GetShader(frag, ShaderParameter.CompileStatus, out int fragCode);
        if (fragCode != (int)All.True)
        {
            string infoLog = GL.GetShaderInfoLog(frag);
            throw new Exception($"Error occurred whilst compiling Shader({frag}).\n\n{infoLog}");
        }

        program = GL.CreateProgram();
        GL.AttachShader(program, vert);
        GL.AttachShader(program, frag);
        GL.LinkProgram(program);
        
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int linkCode);
        if (linkCode != (int)All.True)
        {
            throw new Exception($"Error occurred whilst linking Program({program})");
        }
        
        GL.DetachShader(program, vert);
        GL.DetachShader(program, frag);
        GL.DeleteShader(vert);
        GL.DeleteShader(frag);
        
        var text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam in nunc vitae
ex pharetra condimentum id in magna. Duis lacinia ullamcorper tellus, pulvinar
fermentum ipsum efficitur sed. Proin bibendum augue ut urna finibus viverra. Integer
eleifend erat eget neque tincidunt aliquam. Pellentesque at nisi cursus, tristique
tellus in, lobortis mi. Curabitur vel enim dolor. Maecenas quis turpis a nisi luctus
consequat ac sit amet neque. Cras ac justo diam. Donec nulla nisi, mollis vitae felis
consectetur, pharetra ornare odio. Donec sit amet sapien eget lacus facilisis faucibus.
Phasellus rutrum sed ligula nec aliquam. Nullam vitae volutpat felis, sit amet tristique
felis. Aliquam dictum sapien semper tortor elementum, congue convallis ligula
ultricies. Vivamus imperdiet maximus urna.";

        freetype = new Library();
        fontRegular = new Font(freetype, "Roboto-Regular.ttf", 48, 2048, 2048);
        fontBold = new Font(freetype, "Roboto-Bold.ttf", 48, 2048, 2048);
        fontItalic = new Font(freetype, "Roboto-Italic.ttf", 48, 2048, 2048);
        TextBuilder textBuilder = new TextBuilder(fontRegular, fontBold, fontItalic)
            .WithBaselineOffset(-fontRegular.Size * 64)
            .AddText(new StyledText(text, fontRegular)
                .WithColor(Color.White)
                .WithUnderline(true));
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
        
        GL.EnableVertexArrayAttrib(vao, 3);
        GL.VertexArrayVertexBuffer(vao, 3, vbo, new IntPtr(sizeof(float) * 8), stride);
        GL.VertexArrayAttribIFormat(vao, 3, 1, VertexAttribType.Int, 0);
        GL.VertexArrayAttribBinding(vao, 3, 3);
        
        GL.CreateTextures(TextureTarget.Texture2D, 3, out textures[0]);
        
        GL.TextureStorage2D(textures[0], 1, SizedInternalFormat.R8, fontRegular.Atlas.Texture.Width, fontRegular.Atlas.Texture.Height);
        GL.TextureSubImage2D(textures[0], 0, 0, 0, fontRegular.Atlas.Texture.Width, fontRegular.Atlas.Texture.Height, PixelFormat.Red, PixelType.UnsignedByte, fontRegular.Atlas.Texture.Pixels);
        
        GL.TextureStorage2D(textures[1], 1, SizedInternalFormat.R8, fontBold.Atlas.Texture.Width, fontBold.Atlas.Texture.Height);
        GL.TextureSubImage2D(textures[1], 0, 0, 0, fontBold.Atlas.Texture.Width, fontBold.Atlas.Texture.Height, PixelFormat.Red, PixelType.UnsignedByte, fontBold.Atlas.Texture.Pixels);
        
        GL.TextureStorage2D(textures[2], 1, SizedInternalFormat.R8, fontItalic.Atlas.Texture.Width, fontItalic.Atlas.Texture.Height);
        GL.TextureSubImage2D(textures[2], 0, 0, 0, fontItalic.Atlas.Texture.Width, fontItalic.Atlas.Texture.Height, PixelFormat.Red, PixelType.UnsignedByte, fontItalic.Atlas.Texture.Pixels);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        GL.Viewport(0, 0, Size.X, Size.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0.0f, Size.X, -Size.Y, 0.0f, -8.0f, 8.0f);
        
        GL.UseProgram(program);
        GL.UniformMatrix4(0, true, ref projection);
        GL.Uniform1(1, 0);
        GL.Uniform1(2, 1);
        GL.Uniform1(3, 2);
        GL.BindTextureUnit(0, textures[0]);
        GL.BindTextureUnit(1, textures[1]);
        GL.BindTextureUnit(2, textures[2]);
        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, vertCount);
        
        SwapBuffers();
    }
}