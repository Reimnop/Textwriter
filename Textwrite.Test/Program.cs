using SharpFont;
using Textwriter;

using Library lib = new Library();
using Font font = new Font(lib, "Montserrat.ttf", 48, 512, 512);
TextMeshGenerator textMeshGenerator = new TextMeshGenerator(font);
TextVertex[] vertices = textMeshGenerator.GenerateVertices("Hello world");
