using UnityEngine;
using Pinball.Gameplay;

namespace Pinball.Obstacles
{
    public class SpringObstacle : ObstacleBase
    {
        [SerializeField] private float springForce = 16f;

        protected override void OnBallHit(Ball ball, Collider2D ballCollider)
        {
            Vector2 dir = (ballCollider.transform.position - transform.position).normalized;
            ball.ApplyImpulse(dir, springForce);
        }
    }
}