// InputEdge.cs
using System.Collections.Generic;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Assignment_4.Rendering
{
    public sealed class InputEdge
    {
        private readonly Dictionary<Keys, bool> _prev = new();

        public bool Pressed(KeyboardState kb, Keys key)
        {
            bool now = kb.IsKeyDown(key);
            bool was = _prev.TryGetValue(key, out var p) && p;
            _prev[key] = now;
            return now && !was;
        }
    }
}
