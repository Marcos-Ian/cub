using Assignment_4;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Assignment_4
{
    class Program
    {
        static void Main(string[] args)
        {
            var gw = GameWindowSettings.Default;
            gw.UpdateFrequency = 60; // Only this is needed now

            var nw = new NativeWindowSettings()
            {
                Title = "Collision Tester",
                Size = new Vector2i(1280, 720),
                Flags = ContextFlags.ForwardCompatible
            };

            using var game = new Game(gw, nw);
            game.Run();
        }
    }
}
