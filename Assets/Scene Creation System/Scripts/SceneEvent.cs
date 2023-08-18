using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Text;
using UnityEditor.PackageManager;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public class SceneEvent : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable
    {
        public string eventID;
        [Space(5)]

        public List<SceneCondition> sceneConditions;

        public List<SceneAction> sceneActions;

        public List<SceneParameteredEvent> sceneParameteredEvents;

        public UnityEvent unityEvent;

        public bool debug = false;

        public int TriggerNumberLeft { get; private set; } = -1;
        bool triggerCount = false;

        public bool Trigger(int triggerNumber = -1)
        {
            if (!triggerCount && triggerNumber != -1)
            {
                TriggerNumberLeft = triggerNumber;
                triggerCount = true;
            }

            if (!sceneConditions.VerifyConditions()) return false;

            if (triggerCount) TriggerNumberLeft--;
            sceneActions.Trigger();
            sceneParameteredEvents.Trigger();
            unityEvent?.Invoke();

            if (debug)
                DebugSceneEvent();

            return true;
        }

        
        public void Init()
        {
            sceneParameteredEvents.Init();
        }
        public void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            sceneConditions.SetUp(_sceneVariablesSO);
            sceneActions.SetUp(_sceneVariablesSO);
            sceneParameteredEvents.SetUp(_sceneVariablesSO);
        }
        public void BelongTo(SceneObject _sceneObject)
        {
            sceneActions.BelongTo(_sceneObject);
            sceneParameteredEvents.BelongTo(_sceneObject);
        }

        private void DebugSceneEvent()
        {
            Debug.LogError("Triggered Scene Event : " + eventID);
        }

        #region SceneLog
        public string Log()
        {
            StringBuilder sb = new();

            return sb.ToString();
        }
        #endregion
    }

    [Serializable]
    public class SceneEvent<T> : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable
    {
        public string eventID;
        [Space(5)]

        public List<SceneCondition> sceneConditions;

        public List<SceneAction> sceneActions;

        public List<SceneParameteredEvent> sceneParameteredEvents;

        public UnityEvent<T> unityEvent;

        public bool debug = false;

        public int TriggerNumberLeft { get; private set; } = -1;
        bool triggerCount = false;

        public bool Trigger(int triggerNumber = -1)
        {
            if (!triggerCount && triggerNumber != -1)
            {
                TriggerNumberLeft = triggerNumber;
                triggerCount = true;
            }

            if (!sceneConditions.VerifyConditions()) return false;

            if (triggerCount) TriggerNumberLeft--;

            sceneActions.Trigger();
            sceneParameteredEvents.Trigger();
            unityEvent?.Invoke(default);

            if (debug)
                DebugSceneEvent();

            return true;
        }
        public bool Trigger(T param, int triggerNumber = -1)
        {
            if (!triggerCount && triggerNumber != -1)
            {
                TriggerNumberLeft = triggerNumber;
                triggerCount = true;
            }

            if (!sceneConditions.VerifyConditions()) return false;

            if (triggerCount) TriggerNumberLeft--;

            sceneActions.Trigger();
            sceneParameteredEvents.Trigger();
            unityEvent?.Invoke(param);

            if (debug)
                DebugSceneEvent();

            return true;
        }

        
        public void Init()
        {
            sceneParameteredEvents.Init();
        }
        public void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            sceneConditions.SetUp(_sceneVariablesSO);
            sceneActions.SetUp(_sceneVariablesSO);
            sceneParameteredEvents.SetUp(_sceneVariablesSO);
        }
        public void BelongTo(SceneObject _sceneObject)
        {
            sceneActions.BelongTo(_sceneObject);
            sceneParameteredEvents.BelongTo(_sceneObject);
        }

        private void DebugSceneEvent()
        {
            Debug.LogError("Triggered Scene Event : " + eventID);
        }

        #region SceneLog
        public string Log()
        {
            StringBuilder sb = new();

            sb.Append("* Event ID : ");
            sb.Append(eventID);
            sb.Append("\n");

            return sb.ToString();
        }
        public List<string> LogLines(bool detailed = false)
        {
            List<string> lines = new();
            StringBuilder sb = new();

            sb.Append(SceneLogger.EventColor);
            sb.Append("|");
            sb.Append(SceneLogger.ColorEnd);
            sb.Append(" Event ID : ");
            sb.Append(eventID);
            Line();

            if (detailed)
            {
                if (sceneConditions != null && sceneConditions.Count > 0)
                {
                    sb.Append("   * IF : ");
                    Line();

                    for (int i = 0; i < sceneConditions.Count; i++)
                    {
                        sb.Append("          ");
                        sb.Append(sceneConditions[i].ToString());
                        if (i < sceneConditions.Count - 1)
                        {
                            Line();
                            sb.Append("          ");
                            sb.Append(sceneConditions[i].logicOperator);
                        }
                        Line();
                    }
                }

                if (sceneActions != null && sceneActions.Count > 0)
                {
                    sb.Append("   * ACTION : ");
                    Line();

                    foreach (var a in sceneActions)
                    {
                        sb.Append("      --> ");
                        sb.Append(a.ToString());
                        Line();
                    }
                }

                if (sceneParameteredEvents != null && sceneParameteredEvents.Count > 0)
                {
                    sb.Append("   * PARAMETERED EVENT : ");
                    Line();

                    foreach (var e in sceneParameteredEvents)
                    {
                        sb.Append("      --> ");
                        sb.Append(e.ToString());
                        Line();
                    }
                }

                int uEventCount = unityEvent.GetPersistentEventCount();
                if (uEventCount > 0)
                {
                    sb.Append("   * UNITY EVENT : ");
                    Line();

                    for (int i = 0; i < uEventCount; i++)
                    {
                        sb.Append("      --> ");
                        sb.Append(unityEvent.GetPersistentTarget(i).ToString());
                        sb.Append(".");
                        sb.Append(unityEvent.GetPersistentMethodName(i));
                        Line();
                    }
                }
            }

            return lines;

            #region Local
            void Line()
            {
                sb.Append('\n');
                lines.Add(sb.ToString());
                sb.Clear();
            }
            #endregion
        }
        #endregion
    }
}
