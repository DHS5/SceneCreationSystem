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

        [Header("Timelines")]
        public List<SceneTimeline> sceneTimelines;

        #region Update SceneVariables
        protected override void UpdateSceneVariables()
        {
            base.UpdateSceneVariables();

            sceneTimelines.SetUp(sceneVariablesSO);
        }
        protected override void UpdateBelongings()
        {
            base.UpdateBelongings();

            sceneTimelines.BelongTo(this);
        }
        #endregion

        #region Listener functions
        [Preserve]
        public void StartTimeline(string timelineID, int step)
        {
            sceneTimelines.Find(t => t.ID == timelineID)?.Start(step);
        }
        public void StartTimeline(string timelineID) { StartTimeline(timelineID, 0); }
        [Preserve]
        public void StopTimeline(string timelineID)
        {
            sceneTimelines.Find(t => t.ID == timelineID)?.Stop();
        }
        [Preserve]
        public void GoToStep(string timelineID, int step, bool interrupt)
        {
            sceneTimelines.Find(t => t.ID == timelineID)?.StartOrGoTo(step, interrupt);
        }
        #endregion
    }
}
