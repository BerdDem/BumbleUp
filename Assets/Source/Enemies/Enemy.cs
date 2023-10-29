using Source.Data;
using Source.Level;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Source.Enemies
{
    public class Enemy : MonoBehaviour, IPositionModifier
    {
        [SerializeField] private Vector3 _offsetPosition = new Vector3(0, 0.5f, 0);
        [SerializeField] private Vector2 _timeRangeToMove = new Vector2(1, 2);
        [SerializeField] private float _height = 1.0f;
        [SerializeField] private float _speed = 5.0f;

        private Map _map;
        private EnemyData _enemyData;
        
        private int _stepIndex;
        private int _segmentIndex;
        
        private float _timeToMove;
        private float _currentTimeToMove;
        private bool _movingProcess;
        
        private Vector3 startPosition;
        private Vector3 endPosition;
        private float journeyTime;
        private float journeyLength;

        public void Initialize(Map map, DataManager dataManager)
        {
            _map = map;
            _enemyData = dataManager.enemyData;
        }

        public void Activate(int stepIndex, int segmentIndex)
        {
            _stepIndex = stepIndex;
            _segmentIndex = segmentIndex;

            _timeToMove = Random.Range(_enemyData.timeToMoveRange.x, _enemyData.timeToMoveRange.y);
            _currentTimeToMove = 0;

            gameObject.transform.position = _map.steps[stepIndex].GetSegmentCenter(segmentIndex) + _offsetPosition;
            gameObject.SetActive(true);

            _map.moveStep += ChangePlayerStep;
        }

        private void Update()
        {
            if (!_movingProcess)
            {
                _currentTimeToMove += Time.deltaTime;
            }
            
            if (_currentTimeToMove >= _timeToMove)
            {
                StartMoving();
            }

            if (_movingProcess)
            {
                Moving();
            }
        }
        
        public void AddToPosition(Vector3 deltaPosition)
        {
            transform.position += deltaPosition;
            startPosition += deltaPosition;
            endPosition += deltaPosition;
        }

        private void StartMoving()
        {
            int lowerStepIndex = _map.steps[_stepIndex].lowerStepIndex;

            if (lowerStepIndex == _map.bottomStep.index)
            {
                Deactivate();
                return;
            }
            
            _timeToMove = Random.Range(_enemyData.timeToMoveRange.x, _enemyData.timeToMoveRange.y);
            _currentTimeToMove = 0;
            
            journeyTime = 0;
            _movingProcess = true;
            startPosition = transform.position;
            
            endPosition = _map.steps[lowerStepIndex].GetSegmentCenter(_segmentIndex) + _offsetPosition;
            journeyLength = Vector3.Distance(startPosition, endPosition);

            _stepIndex = lowerStepIndex;
        }
        
        private void Moving()
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
                _movingProcess = false;
            }
        }
        
        private void ChangePlayerStep()
        {
            if (_stepIndex == _map.bottomStep.index)
            {
                Deactivate();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (player == null)
            {
                return;
            }
            
            player.TakingDamage();
        }

        public void Deactivate()
        {
            _map.moveStep -= ChangePlayerStep;
            _movingProcess = false;
            gameObject.SetActive(false);
        }
    }
}