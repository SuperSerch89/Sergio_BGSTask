public interface IInteractable
{
    InteractionType InteractionType { get; }
    void Perform();
}

public enum InteractionType
{
    None = 0,
    Shop,
}