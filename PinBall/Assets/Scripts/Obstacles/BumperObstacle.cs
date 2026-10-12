using UnityEngine;
using Pinball.Gameplay;

namespace Pinball.Obstacles
{
    public class BumperObstacle : ObstacleBase
    {
        [SerializeField] private float bumpForce = 14f;

        protected override void OnBallHit(Ball ball, Collider2D ballCollider)
        {
            Vector2 dir = (ballCollider.transform.position - transform.position).normalized;
            ball.ApplyImpulse(dir, bumpForce);
        }
    }
}