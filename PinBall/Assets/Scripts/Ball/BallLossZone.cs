using UnityEngine;
using Pinball.Core;

namespace Pinball.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class BallLossZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Ball ball)) return;

            // ����������, ��� ��� ������������� ����, � �� �������� ���� �� ��������
            if (ball.Rigidbody != null && ball.Rigidbody.linearVelocity.magnitude > 4f) return;

            GameManager.Instance?.Balls.NotifyBallLost();
        }
    }
}