using UnityEngine;

namespace Pinball.Obstacles
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RotatingObstacle : ObstacleBase
    {
        [SerializeField] private float degreesPerSecond = 90f;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        private void FixedUpdate()
        {
            rb.MoveRotation(rb.rotation + degreesPerSecond * Time.fixedDeltaTime);
        }
    }
}