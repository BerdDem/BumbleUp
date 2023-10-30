using System.Collections.Generic;
using Source.Data;
using Source.Enemies;
using Source.Level;
using Source.UI;
using UnityEngine;

namespace Source
{
    public class Location : MonoBehaviour
    {
        [SerializeField] private DataManager _dataManager;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private UIStepCount _uiStepCount;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject _stepPrefab;
        [SerializeField] private GameObject _playerPrefab;

        private const int StepsCompleteToTeleportMap = 1000;

        private readonly List<IPositionModifier> _positionModifiers = new List<IPositionModifier>();

        private Map _map;
        private Player _player;

        private Vector3 _nextStepPosition;
        private int _stepsCompleteCount;

        private void Awake()
        {
            _map = new Map(BuildLocation(), _dataManager);
            SpawnPlayer();
            
            _cameraController.SetTarget(_player.transform);
            _enemySpawner.Initialize(_map, _dataManager);
            _uiStepCount.Initialize(_player);
            
            _player.moveNextStep += MoveBottomStepToTop;
            _player.playerDeathNotify += Restart;

            AddPositionModifier();
        }

        private Step[] BuildLocation()
        {
            _nextStepPosition = Vector3.zero;
            Step[] steps = new Step[_dataManager.mapData.stepsCount];
            
            for (int i = 0; i < steps.Length; i++)
            {
                GameObject stepObject = Instantiate(_stepPrefab, _nextStepPosition, Quaternion.identity, transform);
                stepObject.transform.localScale = _dataManager.stepData.stepSize;

                int lowerStepIndex = CycleInt(-1, i, steps.Length);
                int upperStepIndex = CycleInt(1, i, steps.Length);

                Step step = new Step(stepObject, _dataManager, i, lowerStepIndex, upperStepIndex);
                steps[i] = step;
                
                _nextStepPosition += new Vector3(0, _dataManager.mapData.heightBetweenSteps, _dataManager.stepData.stepSize.z);
            }

            return steps;
        }
        
        private void SpawnPlayer()
        {
            Vector3 playerStartPosition = _map.steps[_dataManager.playerData.startStep].GetSegmentCenter(_dataManager.playerData.startSegment);
            GameObject playerObject = Instantiate(_playerPrefab, playerStartPosition, Quaternion.identity, transform);
            
            _player = playerObject.GetComponent<Player>();
            _player.transform.position += _player.offsetPosition;
            _player.Initialize(_map, _dataManager);
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
            _player.Restart();
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
