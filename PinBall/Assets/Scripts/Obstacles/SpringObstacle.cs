using UnityEngine;
using Pinball.Gameplay;

namespace Pinball.Obstacles
{
    public class SpringObstacle : ObstacleBase
    {
        [SerializeField] private float springForce = 25f;
        [SerializeField] private bool debug = false;

        protected override void OnBallHit(Ball ball, Collider2D ballCollider)
        {
            if (ball == null || ball.Rigidbody == null) return;

            Vector2 toBall = (Vector2)ballCollider.transform.position - (Vector2)transform.position;
            if (toBall.sqrMagnitude < 0.0001f)
                toBall = Random.insideUnitCircle.normalized;

            Vector2 dir = toBall.normalized;
            ball.Rigidbody.linearVelocity = dir * springForce;

            if (debug)
                Debug.Log($"[Spring {name}] �������� {ball.Rigidbody.linearVelocity.magnitude:F2}");
        }
    }
}