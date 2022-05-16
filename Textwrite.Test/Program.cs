using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Textwrite.Test;

GameWindowSettings gws = GameWindowSettings.Default;
NativeWindowSettings nws = new NativeWindowSettings
{
    APIVersion = new Version(4, 3),
    Profile = ContextProfile.Core,
    Size = new Vector2i(1600, 900),
    Flags = ContextFlags.ForwardCompatible
};

using (MainWindow mw = new MainWindow(gws, nws))
{
    mw.Run();
}