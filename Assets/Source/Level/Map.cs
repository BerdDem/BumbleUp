using System;
using Source.Data;
using UnityEngine;

namespace Source.Level
{
    public class Map : IPositionModifier
    {
        public Action moveStep;

        public Step topStep { get; private set; }
        public Step bottomStep { get; private set; }
        public Step[] steps { get; }
        
        private Vector3 _topStepPosition;
        private readonly DataManager _dataManager;

        public Map(Step[] steps, DataManager dataManager)
        {
            this.steps = steps;
            _dataManager = dataManager;
            
            bottomStep = this.steps[0];
            topStep = this.steps[this.steps.Length - 1];

            _topStepPosition = topStep.gameObject.transform.position;
        }
        
        public void MoveBottomStepToTop()
          {
            Step movingStep = bottomStep;
            bottomStep = steps[bottomStep.upperStepIndex];
            topStep = movingStep;

            _topStepPosition += new Vector3(0, _dataManager.mapData.heightBetweenSteps, _dataManager.stepData.stepSize.z);
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
                _topStepPosition += new Vector3(0, _dataManager.mapData.heightBetweenSteps, _dataManager.stepData.stepSize.z);
            }
            
            bottomStep = steps[0];
            topStep = steps[steps.Length - 1];
            _topStepPosition = topStep.gameObject.transform.position;
        }
    }
}