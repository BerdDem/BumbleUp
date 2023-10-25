using UnityEngine;

namespace Source
{
    public class Step
    {
        public int nextStepIndex;
        public int index { get; }
        public GameObject gameObject { get; }
        
        private readonly Vector3[] _segmentLocalMiddlePositions;

        public Step(GameObject gameObject, int segmentCount, int index)
        { 
            this.gameObject = gameObject;
            _segmentLocalMiddlePositions = new Vector3[segmentCount];
            this.index = index;
            
            SetupSegmentPositions(segmentCount);
        }

        private void SetupSegmentPositions(int segmentCount)
        {
            Vector3 localScale = gameObject.transform.localScale;
            Quaternion rotation = gameObject.transform.rotation;
            
            Vector3 leftStepPosition = rotation * new Vector3(-localScale.x / 2, localScale.y / 2, 0);
            Vector3 rightStepPosition = rotation * new Vector3(localScale.x / 2, localScale.y / 2, 0);;

            float stepDistance = Vector3.Distance(leftStepPosition, rightStepPosition);
            float segmentDistanceBetweenEdge = stepDistance / segmentCount;
            Vector3 middlePosition = leftStepPosition + rotation * new Vector3(segmentDistanceBetweenEdge / 2, 0, 0);

            for (int i = 0; i < _segmentLocalMiddlePositions.Length; i++)
            {
                _segmentLocalMiddlePositions[i] = middlePosition;
                middlePosition +=  gameObject.transform.rotation * new Vector3(segmentDistanceBetweenEdge, 0, 0);
            }
        }
        
        public Vector3 GetSegmentCenter(int index)
        {
            return gameObject.transform.position + _segmentLocalMiddlePositions[index];
        }
    }
}