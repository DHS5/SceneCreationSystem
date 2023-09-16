using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Text;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public abstract class BaseSceneListener : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable, SceneState.ISceneSubscribable, SceneState.ISceneVarDependant
    {
        [SerializeField] protected SceneVariablesSO sceneVariablesSO;
        public SceneVariablesSO SceneVariablesSO => sceneVariablesSO;

        protected SceneObject sceneObject;

        // SceneVar selection
        [SerializeField] protected int varUniqueID;
        public int UID => varUniqueID;
        public SceneVar CurrentSceneVar
        {
            get { return SceneState.GetSceneVar(varUniqueID); }
        }
        protected SceneVar EditorSceneVar
        {
            get => sceneVariablesSO[varUniqueID];
        }

        // Condition
        [SerializeField] protected bool hasCondition;

        [SerializeField] protected List<SceneCondition> conditions;

        [SerializeField] protected bool debug = false;
        [SerializeField] protected float propertyHeight;

        #region Event Subscription
        public void Subscribe()
        {
            SceneEventManager.StartListening(varUniqueID, OnListenerEvent);
        }
        public void Unsubscribe()
        {
            SceneEventManager.StopListening(varUniqueID, OnListenerEvent);
        }
        private void OnListenerEvent(SceneEventParam _param)
        {
            if (VerifyConditions())
            {
                SceneEventParam param = new(_param);
                param.Context.UpRank(sceneObject.name, " listener received ", param.ToString());

                Trigger(param);
                if (debug)
                    DebugSceneListener(param.Context);
            }
        }
        #endregion

        #region Interfaces
        public void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            sceneVariablesSO = _sceneVariablesSO;

            conditions.SetUp(sceneVariablesSO);
        }
        public void BelongTo(SceneObject _sceneObject)
        {
            sceneObject = _sceneObject;
        }
        #endregion

        #region Utility
        private bool VerifyConditions()
        {
            return !hasCondition || conditions.VerifyConditions();
        }
        protected abstract void Trigger(SceneEventParam _param);
        #endregion

        #region Debug
        protected virtual void DebugSceneListener(SceneContext context)
        {
            Debug.LogError(context.Get());
        }
        #endregion

        #region SceneLog
        public override string ToString()
        {
            StringBuilder sb = new();

            sb.Append("Listen to : [");
            sb.Append(varUniqueID);
            sb.Append("] ");
            sb.Append(EditorSceneVar?.ID);
            sb.Append(" (");
            sb.Append(EditorSceneVar.type);
            sb.Append(")");
            sb.Append("\n");

            return sb.ToString();
        }
        public List<string> LogLines(bool detailed = false)
        {
            if (IsEmpty()) return new();

            List<string> lines = new();
            StringBuilder sb = new();

            sb.Append(SceneLogger.ListenerColor);
            sb.Append("|");
            sb.Append(SceneLogger.ColorEnd);
            sb.Append(" Listen to : ");
            sb.Append(EditorSceneVar?.LogString());
            Line();

            if (detailed)
            {
                if (hasCondition && conditions != null && conditions.Count > 0)
                {
                    sb.Append("   * IF : ");
                    Line();

                    for (int i = 0; i < conditions.Count; i++)
                    {
                        sb.Append("          ");
                        sb.Append(conditions[i].ToString());
                        if (i < conditions.Count - 1)
                        {
                            Line();
                            sb.Append("          ");
                            sb.Append(conditions[i].logicOperator);
                        }
                        Line();
                    }
                }

                lines.AddRange(ChildLog(detailed));
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
        protected virtual bool IsEmpty()
        {
            return false;
        }

        protected abstract List<string> ChildLog(bool detailed = false);
        #endregion

        #region Dependencies
        public virtual List<int> Dependencies
        {
            get
            {
                List<int> dependencies = new List<int>() { UID };

                if (hasCondition)
                    dependencies.AddRange(conditions.Dependencies());
                return dependencies;
            }
        }
        public bool DependOn(int UID) { return Dependencies.Contains(UID); }
        public void SetForbiddenUID(int UID) { }
        #endregion
    }
    
    [Serializable]
    public class SceneListener : BaseSceneListener
    {
        #region SceneEventTrigger
        [Serializable]
        public struct SceneEventTrigger
        {
            public string eventID;
            public bool random;
            public bool remove;

            public override string ToString()
            {
                return "Trigger : " + eventID + (random ? " randomly " : "") + (remove ? " and remove" : "");
            }
        }
        #endregion
        
        public UnityEvent<SceneEventParam> events;

        public List<SceneEventTrigger> triggers;

        #region Utility
        protected override void Trigger(SceneEventParam _param)
        {
            events.Invoke(_param);
            sceneObject.Trigger(triggers, _param);
        }
        #endregion

        #region SceneLog
        protected override List<string> ChildLog(bool detailed = false)
        {
            if (!detailed) return new();

            List<string> lines = new();
            StringBuilder sb = new();

            if (events.GetPersistentEventCount() > 0)
            {
                sb.Append("   * UNITY EVENT : ");
                Line();

                for (int i = 0; i < events.GetPersistentEventCount(); i++)
                {
                    sb.Append("      --> ");
                    sb.Append(events.GetPersistentTarget(i).ToString());
                    sb.Append(".");
                    sb.Append(events.GetPersistentMethodName(i));
                    Line();
                }
            }

            if (triggers.Count > 0)
            {
                sb.Append("   * TRIGGERS : ");
                Line();

                foreach (var trigger in triggers)
                {
                    sb.Append("      --> ");
                    sb.Append(trigger.ToString());
                    Line();
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
        protected override bool IsEmpty()
        {
            return events.GetPersistentEventCount() <= 0 && triggers.Count <= 0;
        }
        #endregion
    }
    
    [Serializable]
    public class SceneSpecificListener : BaseSceneListener
    {
        private Action<SceneEventParam> events;

        #region Utility
        protected override void Trigger(SceneEventParam _param)
        {
            events.Invoke(_param);
        }
        public void SetEvents(Action<SceneEventParam> _events)
        {
            events = _events;
        }
        #endregion

        #region SceneLog
        protected override List<string> ChildLog(bool detailed = false)
        {
            if (!detailed) return new();

            List<string> lines = new();
            StringBuilder sb = new();

            if (events != null)
            {
                sb.Append("   * EVENT : ");
                Line();
                sb.Append("      --> ");
                sb.Append(events.Target.ToString());
                sb.Append(".");
                sb.Append(events.Method.Name);
                Line();
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
    
    /*
    [Serializable]
    public class SceneListener : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable, SceneState.ISceneRegisterable, SceneState.ISceneVarDependant
    {
        #region SceneEventTrigger
        [Serializable]
        public struct SceneEventTrigger
        {
            public string eventID;
            public bool random;
            public bool remove;

            public override string ToString()
            {
                return "Trigger : " + eventID + (random ? " randomly " : "") + (remove ? " and remove" : "");
            }
        }
        #endregion

        public SceneVariablesSO sceneVariablesSO;
        private SceneObject sceneObject;

        // SceneVar selection
        [SerializeField] private int varUniqueID;
        public int UID => varUniqueID;
        public SceneVar CurrentSceneVar
        {
            get { return SceneState.GetSceneVar(varUniqueID); }
        }
        private SceneVar EditorSceneVar
        {
            get => sceneVariablesSO[varUniqueID];
        }
        
        // Condition
        public bool hasCondition;

        public List<SceneCondition> conditions;
        
        public UnityEvent<SceneEventParam> events;

        public List<SceneEventTrigger> triggers;

        public bool debug = false;
        public float propertyHeight;

        #region Event Subscription
        public void Register()
        {
            SceneEventManager.StartListening(varUniqueID, OnListenerEvent);
        }
        public void Unregister()
        {
            SceneEventManager.StopListening(varUniqueID, OnListenerEvent);
        }
        private void OnListenerEvent(SceneEventParam _param)
        {
            if (VerifyConditions())
            {
                SceneEventParam param = new(_param);
                param.Context.UpRank(sceneObject.name, " listener received ", param.ToString());

                Trigger(param);
                if (debug)
                    DebugSceneListener(param.Context);
            }
        }
        #endregion

        #region Interfaces
        public void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            sceneVariablesSO = _sceneVariablesSO;

            conditions.SetUp(sceneVariablesSO);
        }
        public void BelongTo(SceneObject _sceneObject)
        {
            sceneObject = _sceneObject;
        }
        #endregion

        #region Utility
        private bool VerifyConditions()
        {
            return !hasCondition || conditions.VerifyConditions();
        }
        private void Trigger(SceneEventParam _param)
        {
            events.Invoke(_param);
            sceneObject.Trigger(triggers, _param);
        }
        #endregion

        #region Debug
        private void DebugSceneListener(SceneContext context)
        {
            Debug.LogError(context.Get());
        }
        #endregion

        #region SceneLog
        public string Log()
        {
            StringBuilder sb = new();

            sb.Append("* Listen to : [");
            sb.Append(varUniqueID);
            sb.Append("] ");
            sb.Append(EditorSceneVar?.ID);
            sb.Append(" (");
            sb.Append(EditorSceneVar.type);
            sb.Append(")");
            sb.Append("\n");

            return sb.ToString();
        }
        public List<string> LogLines(bool detailed = false)
        {
            List<string> lines = new();
            StringBuilder sb = new();

            sb.Append(SceneLogger.ListenerColor);
            sb.Append("|");
            sb.Append(SceneLogger.ColorEnd);
            sb.Append(" Listen to : ");
            sb.Append(EditorSceneVar?.LogString());
            Line();

            if (detailed)
            {
                if (hasCondition && conditions != null && conditions.Count > 0)
                {
                    sb.Append("   * IF : ");
                    Line();

                    for (int i = 0; i < conditions.Count; i++)
                    {
                        sb.Append("          ");
                        sb.Append(conditions[i].ToString());
                        if (i < conditions.Count - 1)
                        {
                            Line();
                            sb.Append("          ");
                            sb.Append(conditions[i].logicOperator);
                        }
                        Line();
                    }
                }                

                sb.Append("   * UNITY EVENT : ");
                Line();

                for (int i = 0; i < events.GetPersistentEventCount(); i++)
                {
                    sb.Append("      --> ");
                    sb.Append(events.GetPersistentTarget(i).ToString());
                    sb.Append(".");
                    sb.Append(events.GetPersistentMethodName(i));
                    Line();
                }

                sb.Append("   * TRIGGERS : ");
                Line();

                foreach (var trigger in triggers)
                {
                    sb.Append("      --> ");
                    sb.Append(trigger.ToString());
                    Line();
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

        #region Dependencies
        public List<int> Dependencies { get => new List<int>() { UID }; }
        public bool DependOn(int UID) { return Dependencies.Contains(UID); }
        public void SetForbiddenUID(int UID) { }
        #endregion
    }
    */
}
