using UnityEngine;

namespace Source.Data
{
    [CreateAssetMenu(fileName = "SpawnEnemyData", menuName = "ScriptableObjects/SpawnEnemyData")]
    public class SpawnEnemyData : ScriptableObject
    {
        public Vector2Int enemyCountRange;
        public Vector2 timeInterval;
    }
}