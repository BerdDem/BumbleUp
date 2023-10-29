using UnityEngine;

namespace Source.Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public int startStep;
        public int startSegment;
    }
}