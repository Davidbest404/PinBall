using UnityEngine;
using Pinball.Core;
using Pinball.Gameplay;

namespace Pinball.Player
{
    public class Plunger : MonoBehaviour
    {
        [Header("Управление")]
        [SerializeField] private KeyCode[] launchKeys = { KeyCode.Space, KeyCode.Return };

        [Header("Параметры")]
        [SerializeField] private float maxPull = 1.5f;
        [SerializeField] private float pullSpeed = 4f;
        [SerializeField] private float minLaunchForce = 3f;
        [SerializeField] private float maxLaunchForce = 22f;

        [Header("Визуал (3D-модель плунжера)")]
        [SerializeField] private Transform plungerVisual;

        private float pullAmount;
        private bool charging;
        private Vector3 visualRestLocalPos;

        private void Start()
        {
            if (plungerVisual != null)
                visualRestLocalPos = plungerVisual.localPosition;
        }

        private void Update()
        {
            // Ввод читаем ВСЕГДА — не зависим от наличия мяча
            if (AnyKeyDown()) { charging = true; pullAmount = 0f; }

            if (charging && AnyKeyHeld())
            {
                pullAmount = Mathf.Min(pullAmount + pullSpeed * Time.deltaTime, maxPull);
                UpdateVisual();
            }

            if (charging && AnyKeyUp())
            {
                TryLaunch();
                charging = false;
                pullAmount = 0f;
                UpdateVisual();
            }
        }

        private void TryLaunch()
        {
            var gm = GameManager.Instance;
            if (gm == null) { Debug.LogWarning("[Plunger] Нет GameManager.Instance"); return; }

            Ball ball = gm.Balls?.CurrentBall;
            if (ball == null) { Debug.LogWarning("[Plunger] Нет текущего мяча"); return; }

            if (!IsBallAtPlunger(ball))
            {
                Debug.Log("[Plunger] Мяч не у плунжера, запуск отменён");
                return;
            }

            float t = maxPull > 0f ? pullAmount / maxPull : 0f;
            float force = Mathf.Lerp(minLaunchForce, maxLaunchForce, t);

            ball.ApplyImpulse(Vector2.up, force);
            gm.Balls.NotifyBallLaunched();
            Debug.Log($"[Plunger] Запуск силой {force}");
        }

        private bool IsBallAtPlunger(Ball ball)
        {
            Vector2 ballPos = ball.Rigidbody.position;
            Vector2 myPos = transform.position;

            return ballPos.y < myPos.y + 0.5f
                && Mathf.Abs(ballPos.x - myPos.x) < 1f
                && ball.Rigidbody.linearVelocity.magnitude < 0.5f;
        }

        private void UpdateVisual()
        {
            if (plungerVisual == null) return;
            Vector3 p = visualRestLocalPos;
            p.y -= pullAmount;
            plungerVisual.localPosition = p;
        }

        private bool AnyKeyDown()
        {
            foreach (var k in launchKeys) if (Input.GetKeyDown(k)) return true;
            return false;
        }
        private bool AnyKeyHeld()
        {
            foreach (var k in launchKeys) if (Input.GetKey(k)) return true;
            return false;
        }
        private bool AnyKeyUp()
        {
            foreach (var k in launchKeys) if (Input.GetKeyUp(k)) return true;
            return false;
        }
    }
}