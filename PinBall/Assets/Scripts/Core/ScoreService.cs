using System;

namespace Pinball.Core
{
    /// <summary>
    /// Единственная ответственность: хранение и изменение счёта.
    /// Не MonoBehaviour — легко тестируется.
    /// </summary>
    public class ScoreService
    {
        public event Action<int> OnScoreChanged;

        public int CurrentScore { get; private set; }

        public ScoreService()
        {
            CurrentScore = 0;
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;
            CurrentScore += amount;
            OnScoreChanged?.Invoke(CurrentScore);
        }

        public void Reset()
        {
            CurrentScore = 0;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}