using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Events;

namespace Dhs5.SceneCreation
{
    public class SceneClock : SceneObject
    {
        #region Singleton

        public static SceneClock Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            Destroy(gameObject);
        }

        #endregion

        private void Start()
        {
            StartTimeline("Timeline1");
        }

        [Header("Timelines")]
        public List<SceneTimeline> sceneTimelines;

        #region Update SceneVariables
        protected override void UpdateSceneVariables()
        {
            base.UpdateSceneVariables();

            sceneTimelines.SetUp(sceneVariablesSO);
        }
        #endregion

        #region Listener functions
        public void StartTimeline(TimelineEventParam param)
        {
            StartTimeline(param.GetParamTimelineID, param.GetParamTimelineStep);
        }
        [Preserve]
        public void StartTimeline(string timelineID, int step)
        {
            sceneTimelines.Find(t => t.ID == timelineID)?.Start(step);
        }
        public void StartTimeline(string timelineID) { StartTimeline(timelineID, 0); }
        public void StopTimeline(TimelineEventParam param)
        {
            StopTimeline(param.GetParamTimelineID);
        }
        [Preserve]
        public void StopTimeline(string timelineID)
        {
            sceneTimelines.Find(t => t.ID == timelineID)?.Stop();
        }
        public void GoToStep(TimelineEventParam param)
        {
            sceneTimelines.Find(t => t.ID == param.GetParamTimelineID)?.
                StartOrGoTo(param.GetParamTimelineStep, param.interrupt);
        }
        [Preserve]
        public void GoToStep(string timelineID, int step, bool interrupt)
        {
            sceneTimelines.Find(t => t.ID == timelineID)?.StartOrGoTo(step, interrupt);
        }
        #endregion
        
        #region Debug
        public void DebugTest(TimelineEventParam param)
        {
            Debug.LogError(param.SenderTimelineID + " in step " + param.SenderTimelineStep + " sent event : " + param.GetParamTimelineID + ", step : " + param.GetParamTimelineStep + ", time : " + Time.time);
        }
        #endregion
    }

    #region Timeline Event Param
    [Serializable]
    public struct TimelineEventParam
    {
        public TimelineEventParam Send(string ID, int step)
        {
            SenderTimelineID = ID;
            SenderTimelineStep = step;
            return this;
        }
        
        public string SenderTimelineID { get; private set; }
        public int SenderTimelineStep { get; private set; }
        
        [Tooltip("Leave blank to call parent timeline")]
        public string timelineID;
        [Tooltip("Set to -1 to call own step number")]
        public int timelineStep;
        [Tooltip("Whether a GoTo will interrupt the current action or wait")]
        public bool interrupt;

        public string GetParamTimelineID
        {
            get => String.IsNullOrWhiteSpace(timelineID) ? SenderTimelineID : timelineID;
        }

        public int GetParamTimelineStep
        {
            get => timelineStep == -1 ? SenderTimelineStep : timelineStep;
        }
    }
    
    #endregion
}