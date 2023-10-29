using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Source
{
    public class EnemySpawner : MonoBehaviour
    {
        public List<Enemy> enemyPool => _enemyPool;

        [SerializeField] private GameObject _enemyPrefab;
        
        private const int PoolSize = 100;
        
        private Map _map;
        
        private readonly List<Enemy> _enemyPool = new List<Enemy>();
        private readonly List<Enemy> _activeEnemies = new List<Enemy>();

        public void Initialize(Map map)
        {
            _map = map;
            
            for (int i = 0; i < PoolSize; i++)
            {
                GameObject enemyObject = Instantiate(_enemyPrefab, transform);
                enemyObject.SetActive(false);
                Enemy enemy = enemyObject.GetComponent<Enemy>();
                _enemyPool.Add(enemy);
                enemy.Initialize(map);
            }
        }

        public void SpawnEnemy(int stepIndex)
        {
            Enemy enemy = GetObjectFromPool();
            _activeEnemies.Add(enemy);
            
            int segment = Random.Range(0, _map.steps[stepIndex].segmentCount);
            enemy.Activate(stepIndex, segment);
        }

        public void DeactivateAllEnemy()
        {
            foreach (Enemy enemy in _activeEnemies)
            {
                enemy.Deactivate();
            }
        }

        private Enemy GetObjectFromPool()
        {
            for (int i = 0; i < _enemyPool.Count; i++)
            {
                if (_enemyPool[i].gameObject.activeSelf)
                {
                    continue;
                }
                
                return _enemyPool[i];
            }

            throw new ArgumentOutOfRangeException($"Limit of objects in the pool has been reached");
        }

    }
}