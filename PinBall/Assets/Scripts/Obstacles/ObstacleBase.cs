using UnityEngine;
using Pinball.Core;
using Pinball.Gameplay;
using Pinball.Interfaces;

namespace Pinball.Obstacles
{
    /// <summary>
    /// База для всех препятствий. Даёт очки при контакте с мячом.
    /// Наследники могут переопределить OnBallHit для своей логики.
    /// </summary>
    public abstract class ObstacleBase : MonoBehaviour, IBallContactHandler
    {
        [Header("Очки")]
        [SerializeField] protected int scoreValue = 50;

        public virtual void OnBallContact(Ball ball, Collider2D ballCollider)
        {
            GameManager.Instance?.AddScore(scoreValue);
            OnBallHit(ball, ballCollider);
        }

        /// <summary>
        /// Точка расширения для наследников.
        /// </summary>
        protected virtual void OnBallHit(Ball ball, Collider2D ballCollider) { }
    }
}