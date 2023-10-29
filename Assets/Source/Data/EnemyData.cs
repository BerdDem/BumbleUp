using UnityEngine;

namespace Source.Data
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public Vector2 timeToMoveRange;
    }
}