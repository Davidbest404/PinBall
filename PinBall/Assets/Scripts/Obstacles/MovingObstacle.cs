using UnityEngine;

namespace Pinball.Obstacles
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovingObstacle : ObstacleBase
    {
        [SerializeField] private Vector2 direction = Vector2.right;
        [SerializeField] private float distance = 2f;
        [SerializeField] private float speed = 1.5f;

        private Rigidbody2D rb;
        private Vector2 startPos;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            startPos = transform.position;
        }

        private void FixedUpdate()
        {
            float offset = Mathf.Sin(Time.time * speed) * distance;
            Vector2 target = startPos + direction.normalized * offset;
            rb.MovePosition(target);
        }
    }
}