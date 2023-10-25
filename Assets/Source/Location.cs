using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    public class Location : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject _stepPrefab;
        [SerializeField] private GameObject _playerPrefab;

        [Header("Step params")]
        [SerializeField] private float _heightBetweenSteps = 1;
        [SerializeField] private int _segmentCount = 7;
        [SerializeField] private Vector3 _stepSize = new Vector3(12, 4, 2);

        private const int StepCount = 10;
        private const int StartStep = 0;
        private const int StartSegment = 3;
        
        private Step[] _steps;
        private Player _player;

        private Step _topStep;
        private Step _bottomStep;
        
        private Vector3 _nextStepPosition;
        private int _bottomStepIndex;
        private int _currentStepIndex;

        private void Start()
        {
            BuildLocation();
            SpawnPlayer();
            
            _cameraController.SetTarget(_player.transform);
            
            _player.moveNextStep += MoveBottomStepToTop;
        }

        private void BuildLocation()
        {
            _bottomStepIndex = 0;
            _nextStepPosition = Vector3.zero;
            _steps = new Step[StepCount];
            
            for (int i = 0; i < _steps.Length; i++)
            {
                GameObject stepObject = Instantiate(_stepPrefab, _nextStepPosition, Quaternion.identity, transform);
                stepObject.transform.localScale = _stepSize;

                Step step = new Step(stepObject, _segmentCount, i);
                _steps[i] = step;
                
                _nextStepPosition += new Vector3(0, _heightBetweenSteps, _stepSize.z);

                if (i != _steps.Length - 1)
                {
                    step.nextStepIndex = i + 1;
                }
            }

            _bottomStep = _steps[0];
            _topStep = _steps[_steps.Length - 1];
        }
        
        private void SpawnPlayer()
        {
            Vector3 playerStartPosition = _steps[StartStep].GetSegmentCenter(StartSegment);
            GameObject playerObject = Instantiate(_playerPrefab, playerStartPosition, Quaternion.identity, transform);
            _player = playerObject.GetComponent<Player>();
            _player.SetSteps(_steps, StartStep, StartSegment);
        }
        
        private void MoveBottomStepToTop()
        {
            //TODO: Move all steps every x(100) steps to zero point
            Step movingStep = _bottomStep;
            _bottomStep = _steps[_bottomStep.nextStepIndex];
            _topStep.nextStepIndex = movingStep.index;
            _topStep = movingStep;

            _topStep.gameObject.transform.position = _nextStepPosition;
            _nextStepPosition += new Vector3(0, _heightBetweenSteps, _stepSize.z);
        }

        private void OnDestroy()
        {
            if (_player != null)
            {
                _player.moveNextStep -= MoveBottomStepToTop;
            }
        }
    }
}
