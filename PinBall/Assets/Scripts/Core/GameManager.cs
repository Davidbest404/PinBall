using System;
using UnityEngine;
using Pinball.Gameplay;

namespace Pinball.Core
{
    /// <summary>
    /// Единственный MonoBehaviour в ядре. Связывает сервисы и предоставляет точку входа.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Ссылки")]
        [SerializeField] private Ball ballPrefab;
        [SerializeField] private Transform ballSpawnPoint;

        [Header("Параметры")]
        [SerializeField] private int startingBalls = 3;

        public ScoreService Score { get; private set; }
        public BallLifecycleService Balls { get; private set; }

        public event Action OnGameOver;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Score = new ScoreService();
            Balls = new BallLifecycleService(ballPrefab, ballSpawnPoint, startingBalls);
            Balls.OnAllBallsLost += HandleAllBallsLost;
        }

        private void Start()
        {
            Balls.StartNewGame();
            Balls.SpawnBall();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        private void HandleAllBallsLost()
        {
            OnGameOver?.Invoke();
        }

        public void AddScore(int amount) => Score.Add(amount);
    }
}