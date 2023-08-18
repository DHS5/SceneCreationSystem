using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Text;
using UnityEditorInternal;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public class SceneListener : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable
    {
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
        private void OnListenerEvent(SceneEventParam param)
        {
            if (VerifyConditions())
            {
                events.Invoke(param);
                sceneObject.Trigger(triggers, param);
                if (debug)
                    Debug.LogError("Received event : " + CurrentSceneVar.ToString());
            }
        }
        #endregion


        public void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            sceneVariablesSO = _sceneVariablesSO;

            conditions.SetUp(sceneVariablesSO);
        }
        public void BelongTo(SceneObject _sceneObject)
        {
            sceneObject = _sceneObject;
        }

        public bool VerifyConditions()
        {
            return !hasCondition || conditions.VerifyConditions();
        }

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
    }
}
