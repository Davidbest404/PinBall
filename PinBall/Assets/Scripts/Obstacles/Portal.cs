using UnityEngine;
using Pinball.Core;
using Pinball.Gameplay;

namespace Pinball.Obstacles
{
    [RequireComponent(typeof(Collider2D))]
    public class Portal : MonoBehaviour
    {
        [Header("Пара")]
        [Tooltip("Портал, в который телепортируется мяч.")]
        [SerializeField] private Portal linkedPortal;

        [Header("Притяжение")]
        [SerializeField] private float pullRadius = 3f;
        [SerializeField] private float pullStrength = 40f;
        [SerializeField] private float maxPullSpeed = 8f;

        [Header("Телепорт")]
        [Tooltip("Радиус, при котором срабатывает телепорт.")]
        [SerializeField] private float teleportRadius = 0.3f;

        [Tooltip("Порог скорости: если мяч быстрее — телепорт не срабатывает.")]
        [SerializeField] private float teleportSpeedThreshold = 6f;

        [Tooltip("Кулдаун после телепорта (сек). Защита от петель.")]
        [SerializeField] private float teleportCooldown = 0.5f;

        [Tooltip("Смещение точки выхода от центра парного портала (в сторону вылета).")]
        [SerializeField] private float exitOffset = 0.5f;

        [Header("Эффекты")]
        [SerializeField] private float exitRotation = 15f;
        [SerializeField] private float exitAngleJitter = 10f;

        [Header("Очки")]
        [SerializeField] private int scoreValue = 200;

        [Header("Отладка")]
        [SerializeField] private bool debug = false;

        private Ball ballInside;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Ball")) return;
            if (!other.TryGetComponent(out Ball ball)) return;

            ballInside = ball;
            GameManager.Instance?.AddScore(scoreValue);

            if (debug)
                Debug.Log($"[Portal {name}] Мяч вошёл. Cooldown до {ball.TeleportCooldownUntil:F2}, now {Time.time:F2}");
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Ball ball)) return;
            if (ballInside != ball) return;

            ballInside = null;
        }

        private void FixedUpdate()
        {
            if (ballInside == null) return;

            Rigidbody2D rb = ballInside.Rigidbody;
            if (rb == null) return;

            Vector2 myPos = transform.position;
            Vector2 ballPos = rb.position;
            Vector2 toCenter = myPos - ballPos;
            float dist = toCenter.magnitude;

            // 1. Притяжение
            if (dist <= pullRadius && dist > 0.001f)
            {
                Vector2 dir = toCenter / dist;
                float falloff = 1f - Mathf.Clamp01(dist / pullRadius);
                Vector2 desiredVelocity = dir * maxPullSpeed * falloff;

                rb.linearVelocity = Vector2.Lerp(
                    rb.linearVelocity,
                    desiredVelocity,
                    pullStrength * Time.fixedDeltaTime / 10f);
            }

            // 2. Телепорт — только если кулдаун прошёл
            if (Time.time < ballInside.TeleportCooldownUntil)
            {
                if (debug && dist <= teleportRadius)
                    Debug.Log($"[Portal {name}] Телепорт на кулдауне ({ballInside.TeleportCooldownUntil - Time.time:F2}s)");
                return;
            }

            if (dist <= teleportRadius && rb.linearVelocity.magnitude <= teleportSpeedThreshold)
            {
                Teleport(ballInside);
            }
        }

        private void Teleport(Ball ball)
        {
            if (linkedPortal == null)
            {
                Debug.LogWarning($"[Portal {name}] Нет парного портала!");
                return;
            }

            Rigidbody2D rb = ball.Rigidbody;

            // Направление «вылета» из парного портала — от его центра к текущему порталу.
            // Так мяч вылетит не в центр, а «наружу», сразу покидая зону триггера.
            Vector2 linkedPos = linkedPortal.transform.position;
            Vector2 myPos = transform.position;
            Vector2 exitDir = (linkedPos - myPos).normalized;
            if (exitDir.sqrMagnitude < 0.0001f)
                exitDir = Random.insideUnitCircle.normalized;

            // Позиция выхода — со смещением
            Vector2 exitPos = linkedPos + exitDir * exitOffset;

            // Скорость: поворачиваем на exitRotation + jitter, сохраняем магнитуду
            float speed = rb.linearVelocity.magnitude;
            if (speed < 1f) speed = 3f; // если мяч стоял — даём минимальную скорость

            // Направление вылета — вдоль exitDir, плюс поворот
            float angle = exitRotation + Random.Range(-exitAngleJitter, exitAngleJitter);
            Vector2 exitVelocity = Quaternion.Euler(0, 0, angle) * exitDir * speed;

            rb.position = exitPos;
            rb.linearVelocity = exitVelocity;

            // Кулдаун: ни этот, ни парный портал не сработают ближайшие N секунд
            ball.TeleportCooldownUntil = Time.time + teleportCooldown;

            // Очищаем состояние
            ballInside = null;
            linkedPortal.ballInside = null;

            if (debug)
                Debug.Log($"[Portal {name}] Телепорт → {linkedPortal.name}, pos={exitPos}, vel={exitVelocity}, mag={exitVelocity.magnitude:F2}");
        }
    }
}