using UnityEngine;
using Pinball.Core;
using Pinball.Gameplay;

namespace Pinball.Obstacles
{
    /// <summary>
    /// �������������� �������������: ���� ��� �������������, �������
    /// ����� ��� ������ �� ������ ������. �� ���������� ��� ������ ����.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PusherCylinder : ObstacleBase
    {
        [Header("������������")]
        [Tooltip("���� �������� �� ��� � �������.")]
        [SerializeField] private float pushForce = 60f;

        [Tooltip("������������ ��������, �� ������� ��������� ���.")]
        [SerializeField] private float maxPushSpeed = 12f;

        [Tooltip("����������� ��������, � ������� ��� �������� �� ����.")]
        [SerializeField] private float minExitSpeed = 4f;

        [Header("�������������")]
        [Tooltip("���� ��� �������� � ���� � �� ����� ����� � ��������� �������������� ������������.")]
        [SerializeField] private float tangentialAssist = 0.3f;

        [Header("�������")]
        [SerializeField] private bool debug = false;

        // ������� �������������� ����, ����� ������������ �� � FixedUpdate
        private readonly System.Collections.Generic.HashSet<Ball> touching = new();

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!col.collider.CompareTag("Ball")) return;
            if (!col.collider.TryGetComponent(out Ball ball)) return;

            touching.Add(ball);

            // ���� � ���� ��� ��� �����
            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(scoreValue);

            if (debug)
                Debug.Log($"[Pusher {name}] ��� �����, �����: {touching.Count}");
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            if (!col.collider.TryGetComponent(out Ball ball)) return;
            touching.Remove(ball);

            if (debug)
                Debug.Log($"[Pusher {name}] ��� �����, ��������: {touching.Count}");
        }

        private void FixedUpdate()
        {
            if (touching.Count == 0) return;

            // ��������, ������ ��� ������ ����� ����� ���������� ���������
            foreach (var ball in touching)
            {
                if (ball == null || ball.Rigidbody == null) continue;
                Push(ball);
            }
        }

        private void Push(Ball ball)
        {
            Rigidbody2D rb = ball.Rigidbody;

            // ����������� �� ������ �������� � ���� (� ��������� XY)
            Vector2 toBall = rb.position - (Vector2)transform.position;
            if (toBall.sqrMagnitude < 0.0001f)
                toBall = Random.insideUnitCircle.normalized;

            Vector2 dir = toBall.normalized;

            // ������� �������� ����� ����������� (��������)
            float radialSpeed = Vector2.Dot(rb.linearVelocity, dir);

            // ���� ��� ��� �� �������� � ������ ������� � ������������ ����
            if (radialSpeed < maxPushSpeed)
            {
                rb.AddForce(dir * pushForce, ForceMode2D.Force);
            }

            // ���� ��� ����� ����������� (�������) � ����� ����������� �������� �������
            if (rb.linearVelocity.magnitude < minExitSpeed)
            {
                // ��������� �������������� ������������, ����� ��� ������������
                Vector2 tangent = new Vector2(-dir.y, dir.x);
                float tangentSign = Vector2.Dot(rb.linearVelocity, tangent) >= 0 ? 1f : -1f;

                Vector2 newVelocity = dir * minExitSpeed
                                    + tangent * minExitSpeed * tangentialAssist * tangentSign;

                rb.linearVelocity = newVelocity;
            }

            // �� ��� ���� ����������� ��-�� ���������� � ������
            if (rb.linearVelocity.sqrMagnitude < 0.01f)
            {
                rb.linearVelocity = dir * minExitSpeed;
            }

            if (debug)
                Debug.Log($"[Pusher {name}] push dir={dir} vel={rb.linearVelocity} mag={rb.linearVelocity.magnitude:F2}");
        }

        // �� ������ ������ ������� ������ ��� ����������
        private void OnDisable()
        {
            touching.Clear();
        }
    }
}