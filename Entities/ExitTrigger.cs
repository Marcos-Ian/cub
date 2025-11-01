using OpenTK.Mathematics;

namespace Assignment_4.Entities
{
    public class ExitTrigger
    {
        public Vector3 Position;
        public float Radius = 1.0f;
        public bool Completed;

        public ExitTrigger(Vector3 pos) { Position = pos; }

        public void Update(Game game)
        {
            if (game.CardReaderUsed && game.PlayerDistance(Position) < Radius)
            {
                game.SetHint("Level Complete! Press R to restart.");
            }
        }
    }
}
