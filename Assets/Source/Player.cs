using System;
using Source.Data;
using Source.Level;
using UnityEngine;

namespace Source
{
    public class Player : MonoBehaviour, IPositionModifier
    {
        public Action moveNextStep;
        public Action playerDeathNotify;
        public Vector3 offsetPosition => _offsetPosition;

        [SerializeField] private float _height = 1.0f;
        [SerializeField] private float _speed = 5.0f;
        [SerializeField] private Vector3 _offsetPosition = new Vector3(0, 0.5f, 0);

        private DataManager _dataManager;
        private Map _map;
        private int _stepIndex;
        private int _stepSegment;
        
        private Vector3 startPosition;
        private Vector3 endPosition;
        private float journeyTime;
        private float journeyLength;
        private bool _jumpProcess;

        public void Initialize(Map map, DataManager dataManager)
        {
            _map = map;
            _dataManager = dataManager;

            _stepIndex = dataManager.playerData.startStep;
            _stepSegment = dataManager.playerData.startSegment;
        }

        private void Update()
        {
            //TODO: Change Input, add limit for moving on segments
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _stepIndex = _map.steps[_stepIndex].upperStepIndex;
                StartJump(_map.steps[_stepIndex].GetSegmentCenter(_stepSegment));
                moveNextStep?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _stepSegment++;
                StartJump(_map.steps[_stepIndex].GetSegmentCenter(_stepSegment));
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _stepSegment--;
                StartJump(_map.steps[_stepIndex].GetSegmentCenter(_stepSegment));
            }

            if (_jumpProcess)
            {
                Jumping();
            }
        }

        public void TakingDamage()
        {
            playerDeathNotify?.Invoke();
        }
        
        public void Restart()
        {
            _jumpProcess = false;
            _stepIndex = _dataManager.playerData.startStep;
            _stepSegment = _dataManager.playerData.startSegment;
            transform.position = _map.steps[_stepIndex].GetSegmentCenter(_stepSegment) + _offsetPosition;
        }
        
        public void AddToPosition(Vector3 deltaPosition)
        {
            transform.position += deltaPosition;
            startPosition += deltaPosition;
            endPosition += deltaPosition;
        }
        
        private void StartJump(Vector3 targetPosition)
        {
            journeyTime = 0;
            _jumpProcess = true;
            startPosition = transform.position;
            endPosition = targetPosition + _offsetPosition;
            journeyLength = Vector3.Distance(startPosition, endPosition);
        }

        private void Jumping()
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