using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Events;
using System.Text;

namespace Dhs5.SceneCreation
{
    public class SceneClock : SceneObject
    {
        #region Singleton

        public static SceneClock Instance;

        protected override void Awake_Ext()
        {
            base.Awake_Ext();

            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        #endregion

        [Header("Timelines")]
        public List<SceneTimeline> sceneTimelines;

        #region Update SceneVariables
        protected override void Init()
        {
            base.Init();

            sceneTimelines.Init();
        }
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

        #region Log
        protected override void ChildLog(List<string> lines, StringBuilder sb, bool detailed)
        {
            string passToLine = "Line()";

            base.ChildLog(lines, sb, detailed);

            AppendColor(SceneLogger.TimelineColor, "Timelines :");
            Line();

            if (sceneTimelines != null && sceneTimelines.Count > 0)
            {
                if (detailed)
                {
                    AppendColor(SceneLogger.TimelineColor, "----------------------------------------");
                    Line();
                }

                foreach (var timeline in sceneTimelines)
                {
                    lines.AddRange(timeline.LogLines(detailed));
                }

                if (detailed)
                {
                    AppendColor(SceneLogger.TimelineColor, "----------------------------------------");
                    Line();
                }
            }
            

            #region Local
            void Line()
            {
                sb.Append('\n');
                lines.Add(sb.ToString());
                sb.Clear();
            }
            void AppendColor(string color, params string[] strings)
            {
                sb.Append(color);
                foreach (var s in strings)
                {
                    if (s == passToLine) Line();
                    else sb.Append(s);
                }
                sb.Append(SceneLogger.ColorEnd);
            }
            #endregion
        }
        #endregion
    }
}
