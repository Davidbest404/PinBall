using UnityEngine;

namespace Pinball.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Flipper : MonoBehaviour
    {
        public enum FlipDirection { CounterClockwise, Clockwise }

        [Header("”правление")]
        [SerializeField] private KeyCode activateKey = KeyCode.Z;

        [Header("”глы")]
        [Tooltip("”гол поко€ в градусах (0 = плашка горизонтальна).")]
        [SerializeField] private float restAngle = 30f;

        [Tooltip("”гол при нажатии.")]
        [SerializeField] private float activeAngle = -30f;

        [Header("—корости")]
        [Tooltip("—корость вращени€ при нажатии, град/сек.")]
        [SerializeField] private float activeSpeed = 1500f;

        [Tooltip("—корость возврата, град/сек.")]
        [SerializeField] private float restSpeed = 700f;

        [Header("ќтладка")]
        [SerializeField] private bool debug = false;

        private Rigidbody2D rb;
        private float currentAngle;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.useFullKinematicContacts = true; // важно: чтобы Kinematic толкал Dynamic-м€ч

            currentAngle = restAngle;
            ApplyRotation(currentAngle);
        }

        private void FixedUpdate()
        {
            bool pressed = Input.GetKey(activateKey);

            float targetAngle = pressed ? activeAngle : restAngle;
            float speed = pressed ? activeSpeed : restSpeed;

            currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, speed * Time.fixedDeltaTime);
            ApplyRotation(currentAngle);

            if (debug)
                Debug.Log($"[{name}] pressed={pressed} angle={currentAngle:F1} target={targetAngle}");
        }

        private void ApplyRotation(float angleDegrees)
        {
            rb.MoveRotation(angleDegrees);
        }

        // ”добно дл€ ручной настройки в редакторе
        private void OnValidate()
        {
            if (!Application.isPlaying && TryGetComponent(out Rigidbody2D r))
            {
                r.bodyType = RigidbodyType2D.Kinematic;
                r.rotation = restAngle;
            }
        }
    }
}