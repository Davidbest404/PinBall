using UnityEngine;
using UnityEngine.UI;
using Pinball.Core;

namespace Pinball.UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text ballsText;

        private void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            gm.Score.OnScoreChanged += UpdateScore;
            gm.Balls.OnBallsChanged += UpdateBalls;

            UpdateScore(gm.Score.CurrentScore);
            UpdateBalls(gm.Balls.BallsLeft);
        }

        private void OnDestroy()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            gm.Score.OnScoreChanged -= UpdateScore;
            gm.Balls.OnBallsChanged -= UpdateBalls;
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null) scoreText.text = $"Score: {score}";
        }

        private void UpdateBalls(int balls)
        {
            if (ballsText != null) ballsText.text = $"Balls: {balls}";
        }
    }
}