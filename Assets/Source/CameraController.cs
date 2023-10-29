﻿using Source.Level;
using UnityEngine;

namespace Source
{
    public class CameraController : MonoBehaviour, IPositionModifier
    {
        [SerializeField] private Vector3 _targetOffsetPosition;
        [SerializeField] private float _positionLerpSpeed;

        private Transform _target;
        private Vector3 smoothPosition;
        private Vector3 targetPosition;
        
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