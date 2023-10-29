using System;
using System.Collections.Generic;
using Source.Data;
using Source.Level;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Source.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        public List<Enemy> enemyPool => _enemyPool;

        [SerializeField] private GameObject _enemyPrefab;
        
        private const int PoolSize = 100;
        
        private DataManager _dataManager;
        private Map _map;

        private readonly List<Enemy> _enemyPool = new List<Enemy>();
        private readonly List<Enemy> _activeEnemies = new List<Enemy>();

        private float _timeToSpawn;
        private float _currentTimeToSpawn;

        public void Initialize(Map map, DataManager dataManager)
        {
            _map = map;
            _dataManager = dataManager;
            
            for (int i = 0; i < PoolSize; i++)
            {
                GameObject enemyObject = Instantiate(_enemyPrefab, transform);
                enemyObject.SetActive(false);
                Enemy enemy = enemyObject.GetComponent<Enemy>();
                _enemyPool.Add(enemy);
                enemy.Initialize(map, dataManager);
            }

            Vector2 timeInterval = dataManager.spawnEnemyData.timeInterval;
            _timeToSpawn = Random.Range(timeInterval.x, timeInterval.y);
        }

        private void Update()
        {
            _currentTimeToSpawn += Time.deltaTime;
            if (_currentTimeToSpawn < _timeToSpawn)
            {
                return;
            }

            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            Vector2Int enemyCountRange = _dataManager.spawnEnemyData.enemyCountRange;
            int enemyCount = Random.Range(enemyCountRange.x, enemyCountRange.y);

            List<int> excludedNumbers = new List<int>();

            for (int i = 0; i < enemyCount; i++)
            {
                Enemy enemy = GetObjectFromPool();
                _activeEnemies.Add(enemy);

                int segment = GetRandomNumberWithExclusions(new Vector2Int(0, _dataManager.stepData.segmentCount),
                    excludedNumbers.ToArray());
                excludedNumbers.Add(segment);

                enemy.Activate(_map.topStep.index, segment);
            }
            
            _currentTimeToSpawn = 0;
            Vector2 timeInterval = _dataManager.spawnEnemyData.timeInterval;
            _timeToSpawn = Random.Range(timeInterval.x, timeInterval.y);   
        }

        private int GetRandomNumberWithExclusions(Vector2Int range, params int[] excludedNumbers)
        {
            int randomValue;
            do
            {
                randomValue = Random.Range(range.x, range.y);
            } 
            while (Array.Exists(excludedNumbers, element => element == randomValue));

            return randomValue;
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