using UnityEngine;
using Pinball.Gameplay;

namespace Pinball.Obstacles
{
    public class BumperObstacle : ObstacleBase
    {
        [SerializeField] private float bumpForce = 20f;
        [SerializeField] private bool debug = false;

        protected override void OnBallHit(Ball ball, Collider2D ballCollider)
        {
            if (ball == null || ball.Rigidbody == null) return;

            // ����������� �� ������ ������� � ������ ����
            Vector2 toBall = (Vector2)ballCollider.transform.position - (Vector2)transform.position;

            // ������ �� ������� �� ����
            if (toBall.sqrMagnitude < 0.0001f)
                toBall = Random.insideUnitCircle.normalized;

            Vector2 dir = toBall.normalized;

            // ��� ���� ������ �������� �������� (�������, ��� AddForce)
            Rigidbody2D rb = ball.Rigidbody;
            rb.linearVelocity = dir * bumpForce;

            if (debug)
                Debug.Log($"[Bumper {name}] ������� {dir} * {bumpForce}, ����� �������� {rb.linearVelocity.magnitude:F2}");
        }
    }
}