using UnityEngine;
using Pinball.Core;
using Pinball.Gameplay;

namespace Pinball.Player
{
    public class Plunger : MonoBehaviour
    {
        [Header("����������")]
        [SerializeField] private KeyCode[] launchKeys = { KeyCode.Space, KeyCode.Return };

        [Header("���������")]
        [SerializeField] private float maxPull = 1.5f;
        [SerializeField] private float pullSpeed = 4f;
        [SerializeField] private float minLaunchForce = 3f;
        [SerializeField] private float maxLaunchForce = 22f;

        [Header("������")]
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
            Ball ball = GameManager.Instance?.Balls.CurrentBall;
            if (ball == null) return;

            if (!IsBallAtPlunger(ball))
            {
                charging = false;
                pullAmount = 0f;
                UpdateVisual();
                return;
            }

            if (AnyKeyDown()) { charging = true; pullAmount = 0f; }

            if (charging && AnyKeyHeld())
            {
                pullAmount = Mathf.Min(pullAmount + pullSpeed * Time.deltaTime, maxPull);
                UpdateVisual();
            }

            if (charging && AnyKeyUp())
            {
                Launch(ball);
                charging = false;
                pullAmount = 0f;
                UpdateVisual();
            }
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

        private void Launch(Ball ball)
        {
            float t = pullAmount / maxPull;
            float force = Mathf.Lerp(minLaunchForce, maxLaunchForce, t);

            ball.ApplyImpulse(Vector2.up, force);
            GameManager.Instance.Balls.NotifyBallLaunched();
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