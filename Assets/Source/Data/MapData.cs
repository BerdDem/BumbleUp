using UnityEngine;

namespace Source.Data
{
    [CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/MapData")]
    public class MapData : ScriptableObject
    {
        public int stepsCount;
        public int heightBetweenSteps;
    }
}