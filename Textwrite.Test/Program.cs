using System.Drawing;
using SharpFont;
using Textwriter;

using Library lib = new Library();
using Font font = new Font(lib, "Montserrat.ttf", 48, 512, 512);
TextMeshGenerator textMeshGenerator = new TextMeshGenerator(font);

StyledText styledText = new StyledText("Hello world!")
    .WithBold(true)
    .WithStrikethrough(true)
    .WithUnderline(true)
    .WithColor(Color.Green);

TextVertex[] vertices = textMeshGenerator.GenerateVertices(styledText);
// upload vertices to GPU then render...

