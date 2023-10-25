using System;
using UnityEngine;

namespace Source
{
    public class Player : MonoBehaviour
    {
        public Action moveNextStep;

        [SerializeField] private float _height = 1.0f;
        [SerializeField] private float _speed = 5.0f;
        
        private Step[] _stepObjects;
        private int _stepIndex;
        private int _stepSegment;

        public void SetSteps(Step[] stepObjects, int stepIndex, int stepSegment)
        {
            _stepObjects = stepObjects;
            _stepIndex = stepIndex;
            _stepSegment = stepSegment;
        }

        private void Update()
        {
            //TODO: Change Input, add limit for moving on segments
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _stepIndex = _stepObjects[_stepIndex].nextStepIndex;
                PlayerStartJump(_stepObjects[_stepIndex].GetSegmentCenter(_stepSegment));
                moveNextStep?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _stepSegment++;
                PlayerStartJump(_stepObjects[_stepIndex].GetSegmentCenter(_stepSegment));
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _stepSegment--;
                PlayerStartJump(_stepObjects[_stepIndex].GetSegmentCenter(_stepSegment));
            }

            if (_jumpProcess)
            {
                PlayerJumping();
            }
        }
        
        //TODO: refactoring shit code 
        private Vector3 startPosition;
        private Vector3 endPosition;
        private float journeyTime;
        private float journeyLength;
        private bool _jumpProcess;
        private void PlayerStartJump(Vector3 targetPosition)
        {
            journeyTime = 0;
            _jumpProcess = true;
            startPosition = transform.position;
             endPosition = targetPosition;
            journeyLength = Vector3.Distance(startPosition, endPosition);
        }
        
        private void PlayerJumping()
        {
            journeyTime += Time.deltaTime;
            
            float distanceCovered = journeyTime * _speed;
            float fractionOfJourney = distanceCovered / journeyLength;

            fractionOfJourney = Mathf.Clamp01(fractionOfJourney);

            float yOffset = _height * (4 * fractionOfJourney - 4 * fractionOfJourney * fractionOfJourney);

            Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);

            currentPos.y += yOffset;
            transform.position = currentPos;

            if (Vector3.Distance(transform.position, endPosition) < 0.05f)
            {
                transform.position = endPosition;
                _jumpProcess = false;
            }
        }
    }
}