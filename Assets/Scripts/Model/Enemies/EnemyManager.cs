using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model.Levels;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Model.Enemies
{
    public sealed class EnemyManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemies;

        private readonly Dictionary<Layer, HashSet<Enemy>> _layers = new();
        private readonly Dictionary<Type, Dictionary<Layer, int>> _enemyCounter = new();

        [FormerlySerializedAs("enemySpawnCooldown")] [FormerlySerializedAs("enemyCooldownMs")] [SerializeField]
        private int enemySpawnCooldownMs = 12000;

        [SerializeField] private float incrementEnemiesByPercentEachLayer = 25;

        private Cooldown _enemyCooldown;
        private readonly Random _random = new();

        public static EnemyManager Instance 
        {
            get
            {
                var enemyManager = GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>();
                if (enemyManager is null)
                    throw new Exception("No EnemyManager present on scene");

                return enemyManager;
            }
        }

        private void Awake()
        {
            _enemyCooldown = new(enemySpawnCooldownMs);
        }

        private void Update()
        {
            if (_enemyCooldown.CoolingDown)
                return;
            
            HandlePossibleLayerChange();
            DespawnFarEnemies();
            SpawnEnemies();
        }

        private void HandlePossibleLayerChange()
        {
            if (!_layers.ContainsKey(LayerManager.Instance.CurrentLayer))
                _layers[LayerManager.Instance.CurrentLayer] = new HashSet<Enemy>();
        }

        private void DespawnFarEnemies()
        {
            var despawnQueue = new Queue<Enemy>();
            foreach (var enemy in from enemy in _layers[LayerManager.Instance.CurrentLayer]
                                  let distanceToTarget = enemy.GetDistanceToTarget()
                                  where distanceToTarget is not null
                                  where distanceToTarget > enemy.DistanceToDespawn
                                  select enemy)
                despawnQueue.Enqueue(enemy);

            foreach (var enemy in despawnQueue)
                DespawnEnemy(enemy);
            
            _enemyCooldown.Start();
        }

        private void SpawnEnemies()
        {
            foreach (var enemyPrefab in enemies)
            {
                var enemyData = enemyPrefab.GetComponent<Enemy>();
                for (var _ = 0;
                     _ < enemyData.MaxNumberOnMap
                     * (1 + incrementEnemiesByPercentEachLayer * LayerManager.Instance.CurrentLayer.LayerId) 
                     - GetEnemyCount(enemyData);
                     _++)
                {
                    if (enemyData is null)
                        throw new InvalidOperationException(
                            $"Enemy {enemyPrefab} doesn't have component derived from 'Enemy'" +
                            " and cannot be managed by EnemyManager.");
                
                    if (GetEnemyCount(enemyData) > enemyData.MaxNumberOnMap)
                        continue;

                    var distanceToPlayer = _random.Next((int)Math.Truncate(enemyData.MinDistanceFromPlayerToSpawn),
                        (int)Math.Floor(enemyData.DistanceToDespawn * 0.8));

                    var location = new Vector3((float)_random.NextDouble() * distanceToPlayer,
                        (float)_random.NextDouble() * distanceToPlayer,
                        LayerManager.Instance.GetLayerDepth(LayerManager.Instance.CurrentLayer.LayerId));
                
                    SpawnEnemy(enemyPrefab, location);
                }
            }
        }

        private void SpawnEnemy(GameObject enemyPrefab, Vector3 location)
        {
            var currentLayer = LayerManager.Instance.CurrentLayer;
            var enemy = Instantiate(enemyPrefab);
            enemy.transform.position = location;
            
            LayerManager.Instance.AddObject(enemy);
            _layers[currentLayer].Add(enemy.GetComponent<Enemy>());

            var enemyComponent = enemyPrefab.GetComponent<Enemy>();
            
            _enemyCounter.TryAdd(enemyComponent.GetType(), new Dictionary<Layer, int> { { currentLayer, 0 } });
            _enemyCounter[enemyComponent.GetType()][currentLayer]++;
            
            Debug.Log($"Spawned '{enemyComponent.GetType()} at {location}'");
        }

        public void DespawnEnemy(Enemy enemy)
        {
            Debug.Log($"Despawned {enemy}");

            var layer = LayerManager.Instance.FindObjectLayer(enemy.gameObject);
            
            _layers[layer].Remove(enemy);
            LayerManager.Instance.RemoveObject(enemy.gameObject);

            Destroy(enemy.gameObject);
            _enemyCounter[enemy.GetType()][layer]--;
        }

        private int GetEnemyCount(Enemy enemy)
        {
            var enemyType = enemy.GetType();
            var currentLayer = LayerManager.Instance.CurrentLayer;
            
            _enemyCounter.TryAdd(enemyType, new Dictionary<Layer, int> { { currentLayer, 0 } });
            return _enemyCounter[enemyType][currentLayer];
        }
    }
}