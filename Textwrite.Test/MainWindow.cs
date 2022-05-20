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
    private int vertCount;

    private Library freetype;
    private List<Font> fonts = new List<Font>();
    private List<int> textures = new List<int>();
    
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

        freetype = new Library();

        fonts = new List<Font>()
        {
            new Font(freetype, "Roboto-Regular.ttf", 32, 2048),
            new Font(freetype, "Roboto-Bold.ttf", 32, 2048),
            new Font(freetype, "Roboto-Italic.ttf", 32, 2048),
            new Font(freetype, "PlayfairDisplay-Regular.ttf", 32, 2048),
            new Font(freetype, "C:\\Windows\\Fonts\\seguiemj.ttf", 32, 2048)
        };

        TextBuilder textBuilder = new TextBuilder(fonts.ToArray())
            .WithBaselineOffset(-fonts[0].Height)
            .AddText(new StyledText("The quick brown fox jumps over the lazy dog. 0123456789\n", fonts[0])
                .WithColor(Color.White))
            .AddText(new StyledText("Sphinx of black quartz, judge my vow. 0123456789\n", fonts[1])
                .WithColor(Color.Aqua))
            .AddText(new StyledText("The quick brown fox jumps over the lazy dog. 0123456789\n", fonts[2])
                .WithColor(Color.Gold))
            .AddText(new StyledText("Sphinx of black quartz, judge my vow. 0123456789\n", fonts[0])
                .WithColor(Color.Red)
                .WithUnderline(true))
            .AddText(new StyledText("The quick brown fox jumps over the lazy dog. 0123456789\n", fonts[2])
                .WithColor(Color.LawnGreen)
                .WithStrikethrough(true))
            .AddText(new StyledText("Changing style", fonts[2])
                .WithColor(Color.White)
                .WithStrikethrough(true))
            .AddText(new StyledText(" ", fonts[0]))
            .AddText(new StyledText("in the middle of", fonts[0])
                .WithColor(Color.Magenta)
                .WithUnderline(true))
            .AddText(new StyledText(" text is also supported! ", fonts[1])
                .WithColor(Color.Wheat))
            .AddText(new StyledText("(even font)\n", fonts[3])
                .WithColor(Color.White))
            .AddText(new StyledText("Also supports ", fonts[0])
                .WithColor(Color.White))
            .AddText(new StyledText("(это русский)", fonts[3])
                .WithColor(Color.Yellow))
            .AddText(new StyledText(" multiple languages!\n", fonts[0])
                .WithColor(Color.White))
            .AddText(new StyledText("Emojis! ", fonts[0])
                .WithColor(Color.White))
            .AddText(new StyledText("💀🙂🤓🤯🎉🍾❤️✨😭😍💵\n", fonts[4])
                .WithColor(Color.White))
            .AddText(new StyledText("All drawn within a single drawcall!\n", fonts[0])
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
        GL.VertexArrayAttribFormat(vao, 1, 4, VertexAttribType.Float, false, 0);
        GL.VertexArrayAttribBinding(vao, 1, 1);
        
        GL.EnableVertexArrayAttrib(vao, 2);
        GL.VertexArrayVertexBuffer(vao, 2, vbo, new IntPtr(sizeof(float) * 6), stride);
        GL.VertexArrayAttribFormat(vao, 2, 2, VertexAttribType.Float, false, 0);
        GL.VertexArrayAttribBinding(vao, 2, 2);

        GL.EnableVertexArrayAttrib(vao, 3);
        GL.VertexArrayVertexBuffer(vao, 3, vbo, new IntPtr(sizeof(float) * 8), stride);
        GL.VertexArrayAttribIFormat(vao, 3, 1, VertexAttribType.Int, 0);
        GL.VertexArrayAttribBinding(vao, 3, 3);

        foreach (Font font in fonts)
        {
            if (font.GrayscaleAtlas != null)
            {
                AtlasTexture atlasTexture = font.GrayscaleAtlas;
                GL.CreateTextures(TextureTarget.Texture2D, 1, out int texture);
                GL.TextureStorage2D(texture, 1, SizedInternalFormat.R8, atlasTexture.Texture.Width, atlasTexture.Texture.Height);
                GL.TextureSubImage2D(texture, 0, 0, 0, atlasTexture.Texture.Width, atlasTexture.Texture.Height, PixelFormat.Red, PixelType.UnsignedByte, atlasTexture.Texture.Pixels);
                textures.Add(texture);
            }
            
            if (font.ColoredAtlas != null)
            {
                AtlasTexture atlasTexture = font.ColoredAtlas;
                GL.CreateTextures(TextureTarget.Texture2D, 1, out int texture);
                GL.TextureStorage2D(texture, 1, SizedInternalFormat.Rgba8, atlasTexture.Texture.Width, atlasTexture.Texture.Height);
                GL.TextureSubImage2D(texture, 0, 0, 0, atlasTexture.Texture.Width, atlasTexture.Texture.Height, PixelFormat.Bgra, PixelType.UnsignedByte, atlasTexture.Texture.Pixels);
                textures.Add(texture);
            }
        }
        
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
        GL.FrontFace(FrontFaceDirection.Ccw);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
        
        GL.Viewport(0, 0, Size.X, Size.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0.0f, Size.X, -Size.Y, 0.0f, -8.0f, 8.0f);
        
        GL.UseProgram(program);
        GL.UniformMatrix4(0, true, ref projection);

        for (int i = 0; i < textures.Count; i++)
        {
            GL.Uniform1(1 + i, i);
            GL.BindTextureUnit(i, textures[i]);
        }
        
        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, vertCount);
        
        SwapBuffers();
    }
}