using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Text;
using UnityEditor.PackageManager;
using System.Runtime.Remoting.Contexts;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public abstract class BaseSceneEvent : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable, SceneState.IInitializable, SceneState.ISceneVarDependant
    {
        public string eventID;
        [Space(5)]

        public List<SceneCondition> sceneConditions;

        public List<SceneAction> sceneActions;

        public List<SceneParameteredEvent> sceneParameteredEvents;

        protected SceneObject sceneObject;
        protected SceneContext context;

        protected abstract UnityEventBase GetUnityEvent();

        #region Trigger Count
        public int TriggerNumberLeft { get; private set; } = -1;
        bool triggerCount = false;

        protected void SetTriggerCount(int triggerNumber)
        {
            if (!triggerCount && triggerNumber != -1)
            {
                TriggerNumberLeft = triggerNumber;
                triggerCount = true;
            }
        }
        #endregion

        #region Trigger
        public abstract bool Trigger(int triggerNumber = -1);
        protected bool IsTriggerValid(int triggerNumber = -1)
        {
            if (!sceneConditions.VerifyConditions()) return false;

            SetTriggerCount(triggerNumber);

            if (triggerCount) TriggerNumberLeft--;

            context = new SceneContext(sceneObject.name);
            context.Add("Trigger : " + eventID);

            return true;
        }

        protected void CoreTrigger()
        {
            sceneActions.Trigger(context);
            sceneParameteredEvents.Trigger();
        }
        #endregion

        #region Set Ups
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
            sceneObject = _sceneObject;
            sceneActions.BelongTo(_sceneObject);
            sceneParameteredEvents.BelongTo(_sceneObject);
        }
        #endregion

        #region Debug
        protected abstract bool DebugCondition();
        protected void DebugSceneEvent()
        {
            if (DebugCondition())
                Debug.LogError(context.Get());
        }
        #endregion

        #region SceneLog
        public string Log()
        {
            StringBuilder sb = new();

            sb.Append("* Event ID : ");
            sb.Append(eventID);
            sb.Append("\n");

            return sb.ToString();
        }
        public List<string> LogLines(bool detailed = false, string alinea = null)
        {
            List<string> lines = new();
            StringBuilder sb = new();

            sb.Append(SceneLogger.EventColor);
            if (alinea != null) sb.Append(alinea);
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

                UnityEventBase unityEvent = GetUnityEvent();
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
                if (alinea != null) sb.Append(alinea);
            }
            #endregion
        }
        #endregion

        #region Dependencies
        public List<int> Dependencies 
        { 
            get
            {
                List<int> dependencies = new List<int>();

                dependencies.AddRange(sceneConditions.Dependencies());
                dependencies.AddRange(sceneActions.Dependencies());
                dependencies.AddRange(sceneParameteredEvents.Dependencies());

                return dependencies;
            }
        }
        public bool DependOn(int UID) { return Dependencies.Contains(UID); }
        public void SetForbiddenUID(int UID) { }
        #endregion
    }

    [Serializable]
    public class SceneEvent : BaseSceneEvent
    {
        public UnityEvent unityEvent;

        public bool debug = false;

        public override bool Trigger(int triggerNumber = -1)
        {
            if (IsTriggerValid(triggerNumber))
            {
                CoreTrigger();
                unityEvent?.Invoke();

                DebugSceneEvent();

                return true;
            }
            return false;
        }

        protected override UnityEventBase GetUnityEvent()
        {
            return unityEvent;
        }

        protected override bool DebugCondition()
        {
            return debug;
        }
    }
    [Serializable]
    public class SceneEvent<T> : BaseSceneEvent
    {
        public UnityEvent<T> unityEvent;

        public bool debug = false;

        public override bool Trigger(int triggerNumber = -1)
        {
            if (IsTriggerValid(triggerNumber))
            {
                CoreTrigger();
                unityEvent?.Invoke(default);

                DebugSceneEvent();

                return true;
            }
            return false;
        }

        public bool Trigger(T param, int triggerNumber = -1)
        {
            if (IsTriggerValid(triggerNumber))
            {
                // Context
                if (param is SceneEventParam p)
                {
                    context = p.Context;
                    context.Add(sceneObject.name, " triggers : ", eventID);
                }

                CoreTrigger();
                unityEvent?.Invoke(param);

                DebugSceneEvent();

                return true;
            }
            return false;
        }

        protected override UnityEventBase GetUnityEvent()
        {
            return unityEvent;
        }

        protected override bool DebugCondition()
        {
            return debug;
        }
    }


    [Serializable]
    public class SceneEvent2 : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable
    {
        public string eventID;
        [Space(5)]

        public List<SceneCondition> sceneConditions;

        public List<SceneAction> sceneActions;

        public List<SceneParameteredEvent> sceneParameteredEvents;

        public UnityEvent unityEvent;

        public bool debug = false;

        private SceneObject sceneObject;

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

            SceneContext context = new SceneContext(sceneObject.name);
            context.Add("Trigger : " + eventID);

            if (triggerCount) TriggerNumberLeft--;
            sceneActions.Trigger(context);
            sceneParameteredEvents.Trigger();
            unityEvent?.Invoke();

            if (debug)
                DebugSceneEvent(context);

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
            sceneObject = _sceneObject;
            sceneActions.BelongTo(_sceneObject);
            sceneParameteredEvents.BelongTo(_sceneObject);
        }

        private void DebugSceneEvent(SceneContext context)
        {
            Debug.LogError(context.Get());
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
    public class SceneEvent2<T> : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable
    {
        public string eventID;
        [Space(5)]

        public List<SceneCondition> sceneConditions;

        public List<SceneAction> sceneActions;

        public List<SceneParameteredEvent> sceneParameteredEvents;

        public UnityEvent<T> unityEvent;

        public bool debug = false;

        private SceneObject sceneObject;

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

            SceneContext context = new SceneContext(sceneObject.name);
            context.Add("Trigger : " + eventID);

            if (triggerCount) TriggerNumberLeft--;

            sceneActions.Trigger(context);
            sceneParameteredEvents.Trigger();
            unityEvent?.Invoke(default);

            if (debug)
                DebugSceneEvent(context);

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

            // Context
            SceneEventParam p = param as SceneEventParam;
            SceneContext context;
            if (p != null)
            {
                context = p.Context;
                context.Add(sceneObject.name, " triggers : ", eventID);
            }
            else
            {
                context = new SceneContext(sceneObject.name);
                context.Add("Trigger : " + eventID);
            }

            if (triggerCount) TriggerNumberLeft--;

            sceneActions.Trigger(context);
            sceneParameteredEvents.Trigger();
            unityEvent?.Invoke(param);

            if (debug)
                DebugSceneEvent(context);

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
            sceneObject = _sceneObject;
            sceneActions.BelongTo(_sceneObject);
            sceneParameteredEvents.BelongTo(_sceneObject);
        }

        private void DebugSceneEvent(SceneContext context)
        {
            Debug.LogError(context.Get());
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
