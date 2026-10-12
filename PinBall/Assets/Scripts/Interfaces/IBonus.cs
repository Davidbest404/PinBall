namespace Pinball.Interfaces
{
    public interface IBonus
    {
        int ScoreValue { get; }
        bool IsBig { get; }
    }
}