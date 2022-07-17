public interface IDestructable
{
    bool HandlesDestruction { get; }
    int StartHealth { get; }
    int CurrentHealth { get; }

    void ChangeHealth(int changeAmount);
}
