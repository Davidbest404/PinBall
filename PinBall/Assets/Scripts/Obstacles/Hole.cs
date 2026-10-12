using UnityEngine;
using Pinball.Gameplay;

namespace Pinball.Obstacles
{
    [RequireComponent(typeof(Collider2D))]
    public class Hole : ObstacleBase
    {
        [Header("Притяжение")]
        [SerializeField] private float pullStrength = 8f;
        [SerializeField] private float maxPullDistance = 2f;

        [Header("Эффект падения")]
        [SerializeField] private float minScale = 0.3f;
        [SerializeField] private float scaleSpeed = 2f;

        private Ball ballInside;
        private Vector3 originalScale;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Ball ball)) return;

            ballInside = ball;
            originalScale = ball.transform.localScale;
            base.OnBallContact(ball, other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Ball ball)) return;
            if (ballInside != ball) return;

            ball.transform.localScale = originalScale;
            ballInside = null;
        }

        private void FixedUpdate()
        {
            if (ballInside == null) return;

            Vector2 toCenter = (Vector2)transform.position - ballInside.Rigidbody.position;
            float dist = toCenter.magnitude;
            if (dist > maxPullDistance) return;

            float t = 1f - dist / maxPullDistance;
            Vector2 pullDir = toCenter.normalized;
            Vector2 tangent = new Vector2(-pullDir.y, pullDir.x);

            ballInside.Rigidbody.AddForce(pullDir * pullStrength * t, ForceMode2D.Force);
            ballInside.Rigidbody.AddForce(tangent * pullStrength * t * 0.6f, ForceMode2D.Force);

            float targetScale = Mathf.Lerp(1f, minScale, t);
            ballInside.transform.localScale = Vector3.Lerp(
                ballInside.transform.localScale,
                originalScale * targetScale,
                Time.fixedDeltaTime * scaleSpeed);
        }
    }
}