using UnityEngine;
using Pinball.Core;
using Pinball.Interfaces;

namespace Pinball.Bonuses
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class BonusBase : MonoBehaviour, IBonus
    {
        [SerializeField] protected int scoreValue = 50;

        public int ScoreValue => scoreValue;
        public abstract bool IsBig { get; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Ball")) return;

            GameManager.Instance?.AddScore(scoreValue);
            BonusSpawner.Instance?.NotifyCollected(this);
            Destroy(gameObject);
        }
    }
}