namespace Assignment_4.Entities
{
    public interface IInteractable
    {
        string Prompt { get; }
        // Returns true if something changed (e.g., picked up, toggled)
        bool Interact(Game game);
        bool CanInteract(Game game);
    }
}
