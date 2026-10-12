using System;
using UnityEngine;
using Pinball.Gameplay;

namespace Pinball.Core
{
    /// <summary>
    /// Отвечает за спавн/потерю мячей и подсчёт оставшихся.
    /// </summary>
    public class BallLifecycleService
    {
        public event Action<int> OnBallsChanged;
        public event Action OnAllBallsLost;

        public int BallsLeft { get; private set; }
        public Ball CurrentBall { get; private set; }
        public bool BallInPlay { get; private set; }

        private readonly Ball ballPrefab;
        private readonly Transform spawnPoint;

        public BallLifecycleService(Ball prefab, Transform spawnPoint, int startingBalls)
        {
            ballPrefab = prefab;
            this.spawnPoint = spawnPoint;
            BallsLeft = startingBalls;
        }

        public void StartNewGame()
        {
            BallsLeft = Mathf.Max(0, BallsLeft);
            OnBallsChanged?.Invoke(BallsLeft);
        }

        public Ball SpawnBall()
        {
            if (ballPrefab == null || spawnPoint == null)
            {
                Debug.LogError("[BallLifecycleService] Не назначены prefab или spawnPoint.");
                return null;
            }

            if (CurrentBall != null)
                UnityEngine.Object.Destroy(CurrentBall.gameObject);

            CurrentBall = UnityEngine.Object.Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
            CurrentBall.ResetState();
            BallInPlay = false;
            return CurrentBall;
        }

        public void NotifyBallLaunched()
        {
            BallInPlay = true;
        }

        public void NotifyBallLost()
        {
            if (!BallInPlay) return;

            BallInPlay = false;
            BallsLeft--;
            OnBallsChanged?.Invoke(BallsLeft);

            if (BallsLeft <= 0)
            {
                OnAllBallsLost?.Invoke();
            }
            else
            {
                SpawnBall();
            }
        }
    }
}