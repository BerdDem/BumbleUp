using System;
using UnityEngine;

namespace Source
{
    public class Map : IPositionModifier
    {
        public Action moveStep;

        public Step topStep { get; private set; }
        public Step bottomStep { get; private set; }
        public Step[] steps { get; }
        
        private const int HeightBetweenSteps = 1;
        
        private Vector3 _topStepPosition;
        private readonly Vector3 _stepSize;
        
        public Map(Step[] steps, Vector3 stepSize)
        {
            this.steps = steps;
            _stepSize = stepSize;
            
            bottomStep = this.steps[0];
            topStep = this.steps[this.steps.Length - 1];

            _topStepPosition = topStep.gameObject.transform.position;
        }
        
        public void MoveBottomStepToTop()
        {
            Step movingStep = bottomStep;
            bottomStep = steps[bottomStep.upperStepIndex];
            topStep = movingStep;

            _topStepPosition += new Vector3(0, HeightBetweenSteps, _stepSize.z);
            topStep.gameObject.transform.position = _topStepPosition;
            
            moveStep?.Invoke();
        }

        public void AddToPosition(Vector3 deltaPosition)
        {
            foreach (Step step in steps)
            {
                step.gameObject.transform.position += deltaPosition;
            }

            _topStepPosition += deltaPosition;
        }

        public void Restart()
        {
            _topStepPosition = Vector3.zero;
            
            foreach (Step step in steps)
            {
                step.gameObject.transform.position = _topStepPosition;
                _topStepPosition += new Vector3(0, HeightBetweenSteps, _stepSize.z);
            }
            
            bottomStep = steps[0];
            topStep = steps[steps.Length - 1];
        }
    }
}