using Assignment_4;
using OpenTK.Windowing.Desktop;

namespace Assignment_4
{
    class Program
    {
        static void Main(string[] args)
        {
            var gw = GameWindowSettings.Default;
            var nw = new NativeWindowSettings()
            {
                Title = "Assignment 4 - Texture Mapping",
                Size = new OpenTK.Mathematics.Vector2i(800, 600)
            };

            using (var game = new Game(gw, nw))
            {
                game.Run();
            }
        }
    }
}