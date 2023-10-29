using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    public class Location : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private EnemySpawner _enemySpawner;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject _stepPrefab;
        [SerializeField] private GameObject _playerPrefab;

        [Header("Step params")]
        [SerializeField] private float _heightBetweenSteps = 1;
        [SerializeField] private int _segmentCount = 7;
        [SerializeField] private Vector3 _stepSize = new Vector3(12, 4, 2);

        private const int StepCount = 20;
        private const int StartStep = 4;
        private const int StartSegment = 3;

        private const int StepsCompleteToTeleportMap = 10;

        private List<IPositionModifier> _positionModifiers = new List<IPositionModifier>();

        private Map _map;
        private Player _player;

        private Vector3 _nextStepPosition;
        private int _stepsCompleteCount;

        private void Awake()
        {
            _map = new Map(BuildLocation(), _stepSize);
            SpawnPlayer();
            
            _cameraController.SetTarget(_player.transform);
            _enemySpawner.Initialize(_map);
            
            _player.moveNextStep += MoveBottomStepToTop;
            _player.playerDeathNotify += Restart;

            AddPositionModifier();
        }

        private Step[] BuildLocation()
        {
            _nextStepPosition = Vector3.zero;
            Step[] steps = new Step[StepCount];
            
            for (int i = 0; i < steps.Length; i++)
            {
                GameObject stepObject = Instantiate(_stepPrefab, _nextStepPosition, Quaternion.identity, transform);
                stepObject.transform.localScale = _stepSize;

                int lowerStepIndex = CycleInt(-1, i, steps.Length);
                int upperStepIndex = CycleInt(1, i, steps.Length);

                Step step = new Step(stepObject, _segmentCount, i, lowerStepIndex, upperStepIndex);
                steps[i] = step;
                
                _nextStepPosition += new Vector3(0, _heightBetweenSteps, _stepSize.z);
            }

            return steps;
        }
        
        private void SpawnPlayer()
        {
            Vector3 playerStartPosition = _map.steps[StartStep].GetSegmentCenter(StartSegment);
            GameObject playerObject = Instantiate(_playerPrefab, playerStartPosition, Quaternion.identity, transform);
            
            _player = playerObject.GetComponent<Player>();
            _player.transform.position += _player.offsetPosition;
            _player.Initialize(_map, StartStep, StartSegment);
        }
        
        private void AddPositionModifier()
        {
            foreach (Enemy enemy in _enemySpawner.enemyPool)
            {
                _positionModifiers.Add(enemy);
            }
            
            _positionModifiers.Add(_cameraController);
            _positionModifiers.Add(_player);
            _positionModifiers.Add(_map);
        }

        private void MoveBottomStepToTop()
        {
            _stepsCompleteCount++;

            _map.MoveBottomStepToTop();
            _enemySpawner.SpawnEnemy(_map.topStep.index);

            if (_stepsCompleteCount >= StepsCompleteToTeleportMap)
            {
                MoveMapToZeroPoint();
                _stepsCompleteCount = 0;
            }
        }

        private void MoveMapToZeroPoint()
        {
            Vector3 delta = -_map.bottomStep.gameObject.transform.position;
            foreach (IPositionModifier positionModifier in _positionModifiers)
            {
                positionModifier.AddToPosition(delta);
            }
        }

        private static int CycleInt(int increment, int index, int count)
        {
            int value = (index + increment) % count;
            
            if (value < 0)
            {
                value += count;
            }

            return value;
        }
        
        private void Restart()
        {
            _map.Restart();
            _enemySpawner.DeactivateAllEnemy();
            _player.Restart(StartStep, StartSegment);
            _cameraController.ResetPositionToPlayer();
        }

        private void OnDestroy()
        {
            if (_player != null)
            {
                _player.moveNextStep -= MoveBottomStepToTop;
                _player.playerDeathNotify -= Restart;
            }
        }
    }
}
