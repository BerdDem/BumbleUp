using Source.Level;
using UnityEngine;

namespace Source
{
    public class CameraController : MonoBehaviour, IPositionModifier
    {
        [SerializeField] private Vector3 _targetOffsetPosition;
        [SerializeField] private float _positionLerpSpeed;
        [SerializeField] private float _rotationLerpSpeed = 5;

        private Transform _target;
        private Vector3 smoothPosition;
        private Vector3 targetPosition;

        private Quaternion smoothRotation;
        private Quaternion targetRotation;
        
        public void SetTarget(Transform target)
        {
            _target = target;
            smoothPosition = _target.position + _targetOffsetPosition;
            targetPosition = _target.position + _targetOffsetPosition;
        }

        private void LateUpdate()
        {
            targetPosition = _target.position + _targetOffsetPosition;
            smoothPosition = Vector3.Lerp(smoothPosition, targetPosition, Time.deltaTime * _positionLerpSpeed);
            transform.position = smoothPosition;

            targetRotation = Quaternion.LookRotation(_target.position - transform.position, Vector3.up);
            smoothRotation = Quaternion.Lerp(smoothRotation, targetRotation, Time.deltaTime * _rotationLerpSpeed);
            transform.rotation = smoothRotation;
        }

        public void AddToPosition(Vector3 deltaPosition)
        {
            smoothPosition += deltaPosition;
            targetPosition += deltaPosition;
        }
        
        public void ResetPositionToPlayer()
        {
            targetPosition = _target.position + _targetOffsetPosition;
            smoothPosition = targetPosition;
            transform.position = targetPosition;
        }
    }
}