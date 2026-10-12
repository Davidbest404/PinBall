using UnityEngine;
using Pinball.Core;
using Pinball.Interfaces;

namespace Pinball.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Ball : MonoBehaviour
    {
        [Header("��������")]
        [SerializeField] private float maxSpeed = 18f;
        [SerializeField] private float minSpeed = 1.2f;

        [Header("����� �� �������")]
        [SerializeField] private int baseBounceScore = 10;
        [SerializeField] private float comboResetTime = 1.5f;

        private Rigidbody2D rb;
        private int bounceStreak;
        private float lastBounceTime;
        private Vector3 originalScale;

        public Rigidbody2D Rigidbody => rb;

        [Header("Телепорт")]
        [Tooltip("До какого времени мяч игнорирует телепорты (защита от петель).")]
        public float TeleportCooldownUntil { get; set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            originalScale = transform.localScale;
        }

        private void FixedUpdate()
        {
            float speed = rb.linearVelocity.magnitude;

            if (speed > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

            if (speed > 0.01f && speed < minSpeed)
                rb.linearVelocity += rb.linearVelocity.normalized * 0.05f;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            HandleScoreCombo(col);
            NotifyHandler(col.collider, col);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            NotifyHandler(other, null);
        }

        private void NotifyHandler(Collider2D other, Collision2D col)
        {
            // ���� �� ������� ��� ��� ��������� ����� IBallContactHandler
            var handlers = other.GetComponentsInParent<IBallContactHandler>();
            foreach (var h in handlers)
            {
                h.OnBallContact(this, other);
            }
        }

        private void HandleScoreCombo(Collision2D col)
        {
            if (col.collider.isTrigger) return;

            if (Time.time - lastBounceTime > comboResetTime)
                bounceStreak = 0;

            lastBounceTime = Time.time;
            bounceStreak++;

            GameManager.Instance?.AddScore(baseBounceScore * bounceStreak);
        }

        public void ApplyImpulse(Vector2 direction, float force)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        }

        public void ResetState()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            transform.localScale = originalScale;
            bounceStreak = 0;
            lastBounceTime = 0f;
            TeleportCooldownUntil = 0f;
        }
    }
}