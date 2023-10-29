using UnityEngine;

namespace Source.Data
{
    [CreateAssetMenu(fileName = "StepData", menuName = "ScriptableObjects/StepData")]
    public class StepData : ScriptableObject
    {
        public Vector3 stepSize;
        public int segmentCount;
    }
}