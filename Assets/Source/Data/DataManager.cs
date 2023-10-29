using UnityEngine;

namespace Source.Data
{
    public class DataManager : MonoBehaviour
    {
        public PlayerData playerData => _playerData;
        public MapData mapData => _mapData;
        public StepData stepData => _stepData;
        public SpawnEnemyData spawnEnemyData => _spawnEnemyData;
        public EnemyData enemyData => _enemyData;
        
        [SerializeField] private PlayerData _playerData;
        [SerializeField] private MapData _mapData;
        [SerializeField] private StepData _stepData;
        [SerializeField] private SpawnEnemyData _spawnEnemyData;
        [SerializeField] private EnemyData _enemyData;
    }
}