using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public class SceneTimeline : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable
    {
        public string ID;
        public bool loop;
        public SceneLoopCondition endLoopCondition;
        public List<TimelineObject> timelineObjects;
        
        public bool IsActive { get; private set; }

        private Coroutine coroutine;
        private Queue<TimelineObject> timelineQueue = new();
        private TimelineObject currentTimelineObject;
        private int currentStep;
        
        public void SetUp(SceneVariablesSO sceneVariablesSO)
        {
            endLoopCondition.SetUp(sceneVariablesSO);
            timelineObjects.SetUp(sceneVariablesSO);
        }
        public void BelongTo(SceneObject _sceneObject)
        {
            timelineObjects.BelongTo(_sceneObject);
        }

        private IEnumerator TimelineRoutine()
        {
            IsActive = true;

            //Reset the end loop condition
            endLoopCondition.Reset();

            do
            {
                //Debug.LogError(ID + " begin at step : " + currentStep + " at : " + Time.time);
                for (;timelineQueue.Count > 0;)
                {
                    currentTimelineObject = timelineQueue.Dequeue();
                    currentStep++;
                    yield return StartCoroutine(currentTimelineObject.Process(this, currentStep));
                }
                SetUpQueue();
            } while (loop && !endLoopCondition.CurrentConditionResult);
            //Debug.LogError(ID + " ended at : " + Time.time);

            IsActive = false;
        }
        
        #region Timeline Actions
        public void Start(int step = 0)
        {
            if (IsActive) return;
            
            SetUpQueue(step);
            coroutine = StartMainCR(TimelineRoutine());
        }
        public void Stop()
        {
            if (!IsActive) return;
            
            StopMainCR();
            currentTimelineObject.StopCoroutine();

            IsActive = false;
        }
        public void GoToStep(int step, bool interrupt)
        {
            Debug.LogError(ID + " GoTo step : " + step);
            SetUpQueue(step);
            currentTimelineObject.StopExecution(interrupt);
        }
        public void StartOrGoTo(int step, bool interrupt)
        {
            if (IsActive)
            {
                GoToStep(step, interrupt);
                return;
            }
            Start(step);
        }
        #endregion
        
        #region Utility
        private void SetUpQueue(int step = 0)
        {
            timelineQueue = new();
            currentStep = step - 1;

            for (int i = step; i < timelineObjects.Count; i++)
            {
                timelineQueue.Enqueue(timelineObjects[i]);
            }
        }
        
        private Coroutine StartMainCR(IEnumerator Coroutine)
        {
            return SceneClock.Instance.StartCoroutine(Coroutine);
        }
        private void StopMainCR()
        {
            SceneClock.Instance.StopCoroutine(coroutine);
        }

        private IEnumerator StartCoroutine(IEnumerator Coroutine)
        {
            yield return SceneClock.Instance.StartCoroutine(Coroutine);
        }
        #endregion
    }
}
