using System.Collections.Generic;
using UnityEngine;

namespace Pinball.Bonuses
{
    public class BonusSpawner : MonoBehaviour
    {
        public static BonusSpawner Instance { get; private set; }

        [Header("Префабы")]
        [SerializeField] private BonusBase smallBonusPrefab;
        [SerializeField] private BonusBase bigBonusPrefab;

        [Header("Точки спавна")]
        [SerializeField] private Transform[] spawnPoints;

        [Header("Тайминг")]
        [SerializeField] private float minInterval = 5f;
        [SerializeField] private float maxInterval = 10f;

        [Header("Шанс большого")]
        [Range(0f, 1f)]
        [SerializeField] private float bigChance = 0.25f;

        private readonly Dictionary<Transform, BonusBase> occupied = new();
        private float nextSpawnTime;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start() => ScheduleNext();

        private void Update()
        {
            if (Time.time < nextSpawnTime) return;
            TrySpawn();
            ScheduleNext();
        }

        private void ScheduleNext() =>
            nextSpawnTime = Time.time + Random.Range(minInterval, maxInterval);

        private void TrySpawn()
        {
            if (spawnPoints == null || spawnPoints.Length == 0) return;

            var free = new List<Transform>();
            foreach (var p in spawnPoints)
            {
                if (p == null) continue;
                if (!occupied.TryGetValue(p, out var existing) || existing == null)
                    free.Add(p);
            }
            if (free.Count == 0) return;

            Transform point = free[Random.Range(0, free.Count)];
            bool big = Random.value < bigChance;
            BonusBase prefab = big ? bigBonusPrefab : smallBonusPrefab;
            if (prefab == null) return;

            BonusBase instance = Instantiate(prefab, point.position, Quaternion.identity);
            occupied[point] = instance;
        }

        public void NotifyCollected(BonusBase bonus)
        {
            var keys = new List<Transform>(occupied.Keys);
            foreach (var k in keys)
            {
                if (occupied[k] == null || occupied[k] == bonus)
                    occupied.Remove(k);
            }
        }
    }
}