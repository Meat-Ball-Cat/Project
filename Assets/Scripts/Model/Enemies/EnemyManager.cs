using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Scripts.Model.Levels;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Model.Enemies
{
    public sealed class EnemyManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemies;

        private Dictionary<Layer, HashSet<Enemy>> _layers = new();
        private Dictionary<Type, int> _enemyCounter = new();
        
        private Cooldown _enemyCooldown = new(12000); // TODO переделать в SerializeField
        private Random _random = new();

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
                for (var _ = 0; _ < enemyData.MaxNumberOnMap - GetEnemyCount(enemyData); _++)
                {
                    if (enemyData is null)
                        throw new InvalidOperationException(
                            $"Enemy {enemyPrefab} doesn't have component derived from 'Enemy'" +
                            $" and cannot be managed by EnemyManager.");
                
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
            var enemy = Instantiate(enemyPrefab);
            enemy.transform.position = location;
            
            LayerManager.Instance.AddObject(enemy);
            _layers[LayerManager.Instance.CurrentLayer].Add(enemy.GetComponent<Enemy>());

            var enemyComponent = enemyPrefab.GetComponent<Enemy>();
            
            _enemyCounter.TryAdd(enemyComponent.GetType(), 0);
            _enemyCounter[enemyComponent.GetType()]++;
            
            Debug.Log($"Spawned '{enemyComponent.GetType()} at {location}'");
        }

        private void DespawnEnemy(Enemy enemy)
        {
            Debug.Log($"Despawned {enemy}");
            _layers[LayerManager.Instance.CurrentLayer].Remove(enemy);
            LayerManager.Instance.CurrentLayer.RemoveObject(enemy.gameObject);
            Destroy(enemy.gameObject);
            _enemyCounter[enemy.GetType()]--;
        }

        private int GetEnemyCount(Enemy enemy)
        {
            var enemyType = enemy.GetType();
            
            _enemyCounter.TryAdd(enemyType, 0);
            return _enemyCounter[enemyType];
        }
    }
}