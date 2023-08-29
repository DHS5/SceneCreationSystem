using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dhs5.SceneCreation
{
    public class SceneManager : SceneObject
    {
        protected virtual void Start()
        {
            SetSceneVars();
        }

        #region SceneVars & SceneState Setup
        /// <summary>
        /// Index of the balancing sheet to use for this scene.<br/>
        /// <b>0 is the base SceneVariablesSO, 1 is the first BalancingSheet of the list</b>
        /// </summary>
        public static int BalancingIndex { get; set; } = 0;

        /// <summary>
        /// Set the SceneState's SceneVars at the beginning of the Scene
        /// </summary>
        protected void SetSceneVars()
        {
            SceneState.SetSceneVars(sceneVariablesSO, BalancingIndex);
        }
        /// <summary>
        /// Change balancing during a Scene <b>ARLEADY SETUP</b><br/>
        /// and update <b>ONLY the STATIC and RANDOM vars</b><br/>
        /// without triggering any <i>ChangedVar event</i>
        /// </summary>
        /// <remarks>Never use before <see cref="SetSceneVars"/></remarks>
        public void ChangeBalancing()
        {
            SceneState.ActuBalancing(sceneVariablesSO, BalancingIndex);
        }
        /// <summary>
        /// Change balancing during a Scene <b>ARLEADY SETUP</b><br/>
        /// and update <b>ONLY the STATIC and RANDOM vars</b><br/>
        /// without triggering any <i>ChangedVar event</i>
        /// </summary>
        /// <remarks>Never use before <see cref="SetSceneVars"/></remarks>
        /// <param name="balancingIndex">Index of the balancing sheet to apply</param>
        public void ChangeBalancing(int balancingIndex)
        {
            BalancingIndex = balancingIndex;
            SceneState.ActuBalancing(sceneVariablesSO, BalancingIndex);
        }
        #endregion

        #region SceneClock Actions

        /// <summary>
        /// Starts the timeline named <paramref name="timelineID"/> from first step (step 0)
        /// </summary>
        /// <param name="timelineID">ID of the timeline to start</param>
        [Preserve]
        public void StartTimeline(string timelineID)
        {
            SceneClock.Instance.StartTimeline(timelineID);
        }
        /// <summary>
        /// Starts the timeline named <paramref name="timelineID"/> from step <paramref name="step"/>
        /// </summary>
        /// <param name="timelineID">ID of the timeline to start</param>
        /// <param name="step">Index of the step in the <b>TimelineObject</b> list of the <b>SceneTimeline</b></param>
        [Preserve]
        public void StartTimeline(string timelineID, int step)
        {
            SceneClock.Instance.StartTimeline(timelineID, step);
        }

        /// <summary>
        /// Stops the timeline named <paramref name="timelineID"/>
        /// </summary>
        /// <param name="timelineID">ID of the timeline to stop</param>
        [Preserve]
        public void StopTimeline(string timelineID)
        {
            SceneClock.Instance.StopTimeline(timelineID);
        }

        /// <summary>
        /// Makes the timeline <paramref name="timelineID"/> go to step <paramref name="step"/>.<br/>
        /// If <paramref name="interrupt"/> is <b>true</b>, immediatly go to new step.<br/>
        /// Else, wait for the current step to finish its execution.
        /// </summary>
        /// <param name="timelineID">ID of the timeline to change step</param>
        /// <param name="step">Index of the step in the <b>TimelineObject</b> list of the <b>SceneTimeline</b></param>
        /// <param name="interrupt">Whether to interrupt the execution of the current step</param>
        [Preserve]
        public void TimelineGoToStep(string timelineID, int step, bool interrupt)
        {
            SceneClock.Instance.GoToStep(timelineID, step, interrupt);
        }
        #endregion
    }
}
