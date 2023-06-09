using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using static UnityEngine.EventSystems.EventTrigger;
using System.Text;

namespace Dhs5.SceneCreation
{
    #region Enums
    [Serializable]
    public enum SceneVarType
    {
        BOOL, INT, FLOAT, STRING, EVENT
    }

    [Serializable]
    public enum BoolOperation
    {
        SET, INVERSE
    }
    [Serializable]
    public enum BoolComparison
    {
        EQUAL, DIFF
    }
    [Serializable]
    public enum IntOperation
    {
        SET, ADD, SUBSTRACT, MULTIPLY, DIVIDE, POWER
    }
    [Serializable]
    public enum IntComparison
    {
        EQUAL, DIFF, SUP, INF, SUP_EQUAL, INF_EQUAL
    }
    [Serializable]
    public enum FloatOperation
    {
        SET, ADD, SUBSTRACT, MULTIPLY, DIVIDE, POWER
    }
    [Serializable]
    public enum FloatComparison
    {
        EQUAL, DIFF, SUP, INF, SUP_EQUAL, INF_EQUAL
    }
    [Serializable]
    public enum StringOperation
    {
        SET, APPEND, REMOVE
    }
    [Serializable]
    public enum StringComparison
    {
        EQUAL, DIFF, CONTAINS, CONTAINED, NULL_EMPTY
    }
    [Serializable]
    public enum LogicOperator
    {
        AND, OR, NAND, NOR, XOR, XNOR
    }
    #endregion

    public static class SceneState
    {
        private static Dictionary<int, SceneVar> SceneVariables = new();
        private static Dictionary<int, ComplexSceneVar> ComplexSceneVariables = new();
        private static Dictionary<int, List<int>> SceneVarLinks = new();
        private static Dictionary<int, object> FormerValues = new();

        #region Private Utility functions
        private static void Clear()
        {
            SceneVariables.Clear();
            ComplexSceneVariables.Clear();
            SceneVarLinks.Clear();
        }
        private static void AddVar(SceneVar variable)
        {
            SceneVariables[variable.uniqueID] = new(variable);
        }
        private static void AddComplexVar(ComplexSceneVar variable)
        {
            SceneVar link = GetSceneVar(variable.uniqueID);
            ComplexSceneVariables[variable.uniqueID] = new(variable);
        }

        private static void SaveFormerValues()
        {
            if (SceneVariables == null) return;

            FormerValues.Clear();
            foreach (var pair in SceneVariables)
            {
                FormerValues[pair.Key] = pair.Value.Value;
            }
        }

        private static void ChangedVar(int varUniqueID, SceneObject sender)
        {
            if (SceneVariables.ContainsKey(varUniqueID))
            {
                SceneEventManager.TriggerEvent(varUniqueID, new(SceneVariables[varUniqueID], FormerValues[varUniqueID], sender));
            }
            if (SceneVarLinks.ContainsKey(varUniqueID))
            {
                foreach (var complexUID in SceneVarLinks[varUniqueID])
                {
                    ChangedComplexVar(complexUID, sender);
                }
            }
        }
        private static void ChangedComplexVar(int complexUID, SceneObject sender)
        {
            if (ComplexSceneVariables.ContainsKey(complexUID))
            {
                SceneEventManager.TriggerEvent(complexUID, new(SceneVariables[complexUID], FormerValues[complexUID], sender));
            }
        }
        #endregion

        #region Public accessors
        public static object GetObjectValue(int varUniqueID)
        {
            if (SceneVariables.ContainsKey(varUniqueID))
                return SceneVariables[varUniqueID].Value;
            IncorrectID(varUniqueID);
            return null;
        }

        public static SceneVar GetSceneVar(int uniqueID)
        {
            return new SceneVar(SceneVariables[uniqueID]);
        }
        public static object GetComplexSceneVarValue(int uniqueID)
        {
            if (ComplexSceneVariables.ContainsKey(uniqueID))
            {
                return ComplexSceneVariables[uniqueID].Value;
            }
            Debug.LogError("The ComplexSceneVar with UID : " + uniqueID + " doesn't exist");
            return null;
        }
        public static bool TryGetBoolValue(int varUniqueID, out bool value)
        {
            value = false;
            if (SceneVariables.ContainsKey(varUniqueID))
            {
                SceneVar sceneVar = SceneVariables[varUniqueID];
                if (sceneVar.type == SceneVarType.BOOL)
                {
                    value = sceneVar.BoolValue;
                    return true;
                }
                IncorrectType(varUniqueID, SceneVarType.BOOL);
                return false;
            }
            IncorrectID(varUniqueID);
            return false;
        }
        public static bool TryGetIntValue(int varUniqueID, out int value)
        {
            value = 0;
            if (SceneVariables.ContainsKey(varUniqueID))
            {
                SceneVar sceneVar = SceneVariables[varUniqueID];
                if (sceneVar.type == SceneVarType.INT)
                {
                    value = sceneVar.IntValue;
                    return true;
                }
                IncorrectType(varUniqueID, SceneVarType.INT);
                return false;
            }
            IncorrectID(varUniqueID);
            return false;
        }
        public static bool TryGetFloatValue(int varUniqueID, out float value)
        {
            value = 0f;
            if (SceneVariables.ContainsKey(varUniqueID))
            {
                SceneVar sceneVar = SceneVariables[varUniqueID];
                if (sceneVar.type == SceneVarType.FLOAT)
                {
                    value = sceneVar.FloatValue;
                    return true;
                }
                IncorrectType(varUniqueID, SceneVarType.FLOAT);
                return false;
            }
            IncorrectID(varUniqueID);
            return false;
        }
        public static bool TryGetStringValue(int varUniqueID, out string value)
        {
            value = null;
            if (SceneVariables.ContainsKey(varUniqueID))
            {
                SceneVar sceneVar = SceneVariables[varUniqueID];
                if (sceneVar.type == SceneVarType.STRING)
                {
                    value = sceneVar.StringValue;
                    return true;
                }
                IncorrectType(varUniqueID, SceneVarType.STRING);
                return false;
            }
            IncorrectID(varUniqueID);
            return false;
        }
        #endregion

        #region Public setters
        public static void SetSceneVars(SceneVariablesSO sceneVariablesSO, int balancingIndex = 0)
        {
            Clear();
            if (sceneVariablesSO == null) return;
            List<SceneVar> sceneVars = new(sceneVariablesSO.BalancedSceneVars(balancingIndex));
            List<ComplexSceneVar> complexSceneVars = new(sceneVariablesSO.complexSceneVars);
            SetSceneVars(sceneVars);
            SetComplexSceneVars(complexSceneVars);
            SetSceneLinks();
        }
        private static void SetSceneVars(List<SceneVar> sceneVars)
        {
            foreach (SceneVar sceneVar in sceneVars)
                AddVar(sceneVar);
        }
        private static void SetComplexSceneVars(List<ComplexSceneVar> complexSceneVars)
        {
            foreach (ComplexSceneVar var in complexSceneVars)
            {
                AddComplexVar(var);
            }
        }
        private static void SetSceneLinks()
        {
            foreach (var pair in ComplexSceneVariables)
            {
                foreach (var depUID in pair.Value.Dependencies)
                {
                    if (!SceneVarLinks.ContainsKey(depUID))
                    {
                        SceneVarLinks[depUID] = new();
                    }
                    SceneVarLinks[depUID].Add(pair.Key);
                }
            }
        }

        public static void ActuBalancing(SceneVariablesSO sceneVariablesSO, int balancingIndex)
        {
            foreach (var var in new List<SceneVar>(sceneVariablesSO.BalancedSceneVars(balancingIndex)))
            {
                if (var.IsStatic || var.IsRandom)
                {
                    SceneVariables[var.uniqueID] = var;
                }
            }
        }

        public static void ModifyBoolVar(int varUniqueID, BoolOperation op, bool param, SceneObject sender)
        {
            if (SceneVariables.ContainsKey(varUniqueID))
            {
                SceneVar var = SceneVariables[varUniqueID];
                if (var.type == SceneVarType.BOOL && !var.IsStatic && !var.IsLink)
                {
                    SaveFormerValues();
                    switch (op)
                    {
                        case BoolOperation.SET:
                            SceneVariables[varUniqueID].BoolValue = param;
                            break;
                        case BoolOperation.INVERSE:
                            SceneVariables[varUniqueID].BoolValue = !SceneVariables[varUniqueID].BoolValue;
                            break;

                        default:
                            SceneVariables[varUniqueID].BoolValue = param;
                            break;
                    }
                    ChangedVar(varUniqueID, sender);
                    return;
                }
                IncorrectType(varUniqueID, SceneVarType.BOOL);
                return;
            }
            IncorrectID(varUniqueID);
        }
        public static void ModifyIntVar(int varUniqueID, IntOperation op, int param, SceneObject sender)
        {
            if (SceneVariables.ContainsKey(varUniqueID))
            {
                SceneVar var = SceneVariables[varUniqueID];
                if (var.type == SceneVarType.INT && !var.IsStatic && !var.IsLink && !var.IsRandom)
                {
                    SaveFormerValues();
                    switch (op)
                    {
                        case IntOperation.SET:
                            SceneVariables[varUniqueID].IntValue = param;
                            break;
                        case IntOperation.ADD:
                            SceneVariables[varUniqueID].IntValue += param;
                            break;
                        case IntOperation.SUBSTRACT:
                            SceneVariables[varUniqueID].IntValue -= param;
                            break;
                        case IntOperation.MULTIPLY:
                            SceneVariables[varUniqueID].IntValue *= param;
                            break;
                        case IntOperation.DIVIDE:
                            SceneVariables[varUniqueID].IntValue /= param;
                            break;
                        case IntOperation.POWER:
                            SceneVariables[varUniqueID].IntValue = (int)Mathf.Pow(SceneVariables[varUniqueID].IntValue, param);
                            break;
                        
                        default:
                            SceneVariables[varUniqueID].IntValue = param;
                            break;
                    }
                    if (var.hasMin || var.hasMax)
                    {
                        SceneVariables[varUniqueID].IntValue = (int)Mathf.Clamp(SceneVariables[varUniqueID].IntValue,
                            var.hasMin ? var.minInt : -Mathf.Infinity,
                            var.hasMax ? var.maxInt : Mathf.Infinity);
                    }

                    ChangedVar(varUniqueID, sender);
                    return;
                }
                IncorrectType(varUniqueID, SceneVarType.INT);
                return;
            }
            IncorrectID(varUniqueID);
        }
        public static void ModifyFloatVar(int varUniqueID, FloatOperation op, float param, SceneObject sender)
        {
            if (SceneVariables.ContainsKey(varUniqueID))
            {
                SceneVar var = SceneVariables[varUniqueID];
                if (var.type == SceneVarType.FLOAT && !var.IsStatic && !var.IsLink && !var.IsRandom)
                {
                    SaveFormerValues();
                    switch (op)
                    {
                        case FloatOperation.SET:
                            SceneVariables[varUniqueID].FloatValue = param;
                            break;
                        case FloatOperation.ADD:
                            SceneVariables[varUniqueID].FloatValue += param;
                            break;
                        case FloatOperation.SUBSTRACT:
                            SceneVariables[varUniqueID].FloatValue -= param;
                            break;
                        case FloatOperation.MULTIPLY:
                            SceneVariables[varUniqueID].FloatValue *= param;
                            break;
                        case FloatOperation.DIVIDE:
                            SceneVariables[varUniqueID].FloatValue /= param;
                            break;
                        case FloatOperation.POWER:
                            SceneVariables[varUniqueID].FloatValue = Mathf.Pow(SceneVariables[varUniqueID].FloatValue, param);
                            break;
                        
                        default:
                            SceneVariables[varUniqueID].FloatValue = param;
                            break;
                    }
                    if (var.hasMin || var.hasMax)
                    {
                        SceneVariables[varUniqueID].FloatValue = (int)Mathf.Clamp(SceneVariables[varUniqueID].FloatValue,
                            var.hasMin ? var.minFloat : -Mathf.Infinity,
                            var.hasMax ? var.maxFloat : Mathf.Infinity);
                    }

                    ChangedVar(varUniqueID, sender);
                    return;
                }
                IncorrectType(varUniqueID, SceneVarType.FLOAT);
                return;
            }
            IncorrectID(varUniqueID);
        }
        public static void ModifyStringVar(int varUniqueID, StringOperation op, string param, SceneObject sender)
        {
            if (SceneVariables.ContainsKey(varUniqueID))
            {
                SceneVar var = SceneVariables[varUniqueID];
                if (var.type == SceneVarType.STRING && !var.IsStatic && !var.IsLink)
                {
                    SaveFormerValues();
                    switch (op)
                    {
                        case StringOperation.SET:
                            SceneVariables[varUniqueID].StringValue = param;
                            break;
                        case StringOperation.APPEND:
                            SceneVariables[varUniqueID].StringValue += param;
                            break;
                        case StringOperation.REMOVE:
                            SceneVariables[varUniqueID].StringValue.Replace(param, "");
                            break;

                        default:
                            SceneVariables[varUniqueID].StringValue = param;
                            break;
                    }
                    ChangedVar(varUniqueID, sender);
                    return;
                }
                IncorrectType(varUniqueID, SceneVarType.STRING);
                return;
            }
            IncorrectID(varUniqueID);
        }
        public static void TriggerEventVar(int varUniqueID, SceneObject sender)
        {
            if (SceneVariables.ContainsKey(varUniqueID))
            {
                SceneVar var = SceneVariables[varUniqueID];
                if (var.type == SceneVarType.EVENT)
                {
                    SaveFormerValues();
                    ChangedVar(varUniqueID, sender);
                    return;
                }
                IncorrectType(varUniqueID, SceneVarType.EVENT);
                return;
            }
            IncorrectID(varUniqueID);
        }
        #endregion

        #region Log
        private static void IncorrectID(int ID)
        {
            Debug.LogError("Variable ID : '" + ID + "' doesn't exist in the current scene.");
        }
        private static void IncorrectType(int ID, SceneVarType type)
        {
            Debug.LogError("Variable ID : '" + ID + "' is not of type : '" + type.ToString() + "'.");
        }
        #endregion

        #region Casts
        #region Cast To Bool
        public static bool CastToBool(SceneVar var)
        {
            switch (var.type)
            {
                case SceneVarType.BOOL:
                    return var.BoolValue;
                case SceneVarType.INT:
                    return var.IntValue != 0;
                case SceneVarType.FLOAT:
                    return var.FloatValue != 0;
                case SceneVarType.STRING:
                    return (var.StringValue.ToLower() == "true");
                default:
                    return false;
            }
        }
        #endregion

        #region Cast To Int
        public static int CastToInt(SceneVar var)
        {
            int i;
            switch (var.type)
            {
                case SceneVarType.INT:
                    return var.IntValue;
                case SceneVarType.FLOAT:
                    return (int)var.FloatValue;
                case SceneVarType.BOOL:
                    return var.BoolValue ? 1 : 0;
                case SceneVarType.STRING:
                    int.TryParse(var.StringValue, out i);
                    return i;
                default:
                    return 0;
            }
        }
        #endregion

        #region Cast To Float
        public static float CastToFloat(SceneVar var)
        {
            float f;
            switch (var.type)
            {
                case SceneVarType.FLOAT:
                    return var.FloatValue;
                case SceneVarType.INT:
                    return var.IntValue;
                case SceneVarType.BOOL:
                    return var.BoolValue ? 1f : 0f;
                case SceneVarType.STRING:
                    float.TryParse(var.StringValue, out f);
                    return f;
                default:
                    return 0f;
            }
        }
        #endregion

        #region Cast To String
        public static string CastToString(SceneVar var)
        {
            switch (var.type)
            {
                case SceneVarType.STRING:
                    return var.StringValue;
                case SceneVarType.BOOL:
                    return var.BoolValue.ToString();
                case SceneVarType.INT:
                    return var.IntValue.ToString();
                case SceneVarType.FLOAT:
                    return var.FloatValue.ToString();
                default:
                    return "";
            }
        }
        #endregion
        #endregion

        #region Extension Methods
        #region Set Ups
        public interface ISceneVarSetupable
        {
            public void SetUp(SceneVariablesSO sceneVariablesSO);
        }
        public interface ISceneVarTypedSetupable
        {
            public void SetUp(SceneVariablesSO sceneVariablesSO, SceneVarType type);
        }
        
        public static void SetUp<T>(this List<T> setupables, SceneVariablesSO sceneVariablesSO) where T : ISceneVarSetupable
        {
            if (setupables == null || setupables.Count < 1) return;

            foreach (var setupable in setupables)
            {
                setupable.SetUp(sceneVariablesSO);
            }
        }
        public static void SetUp<T>(this List<T> setupables, SceneVariablesSO sceneVariablesSO, SceneVarType type) where T : ISceneVarTypedSetupable
        {
            if (setupables == null || setupables.Count < 1) return;

            foreach (var setupable in setupables)
            {
                setupable.SetUp(sceneVariablesSO, type);
            }
        }
        #endregion
        
        #region Belongs
        public interface ISceneObjectBelongable
        {
            public void BelongTo(SceneObject _sceneObject);
        }
        
        public static void BelongTo<T>(this List<T> belongables, SceneObject sceneObject) where T : ISceneObjectBelongable
        {
            if (belongables == null || belongables.Count < 1) return;

            foreach (var belongable in belongables)
            {
                belongable.BelongTo(sceneObject);
            }
        }
        #endregion

        #region Dependencies
        public interface ISceneVarDependant
        {
            public List<int> Dependencies { get; }
            public bool CanDependOn(int UID);
            public void SetForbiddenUID(int UID);
        }
        public static List<int> Dependencies<T>(this List<T> list) where T : ISceneVarDependant
        {
            if (list == null || list.Count < 1)
            {
                return new();
            }
            List<int> dependencies = new();
            List<int> temp;
            foreach (var dependant in list)
            {
                temp = dependant.Dependencies;
                if (temp != null && temp.Count > 0)
                {
                    foreach (var dep in temp)
                    {
                        dependencies.Add(dep);
                    }
                }
            }
            return dependencies;
        }
        public static bool CanDependOn<T>(this List<T> list, int UID) where T : ISceneVarDependant
        {
            foreach (var dependant in list)
            {
                if (!dependant.CanDependOn(UID))
                    return false;
            }
            return true;
        }
        public static void SetForbiddenUID<T>(this List<T> list, int UID) where T : ISceneVarDependant
        {
            foreach (var dependant in list)
            {
                dependant.SetForbiddenUID(UID);
            }
        }
        #endregion

        #region Scene Condition list verification (Extension Method)
        public static bool VerifyConditions(this List<SceneCondition> conditions)
        {
            if (conditions == null || conditions.Count < 1) return true;
        
            bool result = conditions[0].VerifyCondition();
        
            for (int i = 1; i < conditions.Count; i++)
            {
                result = ApplyLogicOperator(result, conditions[i - 1].logicOperator, conditions[i].VerifyCondition());
            }
        
            return result;
        }
        private static bool ApplyLogicOperator(bool bool1, LogicOperator op, bool bool2)
        {
            switch (op)
            {
                case LogicOperator.AND: return bool1 & bool2;
                case LogicOperator.OR: return bool1 | bool2;
                case LogicOperator.NAND: return !(bool1 & bool2);
                case LogicOperator.NOR: return !(bool1 | bool2);
                case LogicOperator.XOR: return bool1 ^ bool2;
                case LogicOperator.XNOR: return !(bool1 ^ bool2);
                default: return true;
            }
        }
        #endregion
        
        #region Scene Total list evaluation (Extension Method)
        public static int EvaluateIntTotal(this List<SceneTotal> totals)
        {
            if (totals == null || totals.Count < 1) return 0;
        
            int result = (int)totals[0].Value;
        
            for (int i = 1; i < totals.Count; i++)
            {
                result = ApplyMathOperator(result, totals[i - 1].Op, (int)totals[i].Value);
            }
        
            return result;
        }
        public static float EvaluateFloatTotal(this List<SceneTotal> totals)
        {
            if (totals == null || totals.Count < 1) return 0f;
        
            float result = (float)totals[0].Value;
        
            for (int i = 1; i < totals.Count; i++)
            {
                result = ApplyMathOperator(result, totals[i - 1].Op, (float)totals[i].Value);
            }
        
            return result;
        }
        public static string EvaluateSentence(this List<SceneTotal> totals)
        {
            if (totals == null || totals.Count < 1) return "";

            StringBuilder sb = new StringBuilder();
            foreach (SceneTotal total in totals)
            {
                sb.Append(total.Value);
            }
            return sb.ToString();
        }
        private static int ApplyMathOperator(int int1, SceneTotal.Operator op, int int2)
        {
            switch (op)
            {
                case SceneTotal.Operator.ADD: return int1 + int2;
                case SceneTotal.Operator.SUBTRACT: return int1 - int2;
                case SceneTotal.Operator.MULTIPLY: return int1 * int2;
                case SceneTotal.Operator.DIVIDE: return int1 / int2;
                case SceneTotal.Operator.POWER: return (int)Mathf.Pow(int1, int2);
                default: return 0;
            }
        }
        private static float ApplyMathOperator(float float1, SceneTotal.Operator op, float float2)
        {
            switch (op)
            {
                case SceneTotal.Operator.ADD: return float1 + float2;
                case SceneTotal.Operator.SUBTRACT: return float1 - float2;
                case SceneTotal.Operator.MULTIPLY: return float1 * float2;
                case SceneTotal.Operator.DIVIDE: return float1 / float2;
                case SceneTotal.Operator.POWER: return Mathf.Pow(float1, float2);
                default: return 0;
            }
        }
        #endregion

        #region Random Scene Event triggering (Extension Method)
        /// <summary>
        /// Triggers a random SceneEvent in the list
        /// </summary>
        /// <param name="sceneEvents"></param>
        /// <param name="filter">Trigger a random SceneEvent among ones which eventID contains filter</param>
        /// <returns>Whether an event was triggered</returns>
        public static bool TriggerRandom(this List<SceneEvent> sceneEvents, string filter = null, bool removeAfterTrigger = false)
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return false;

            List<SceneEvent> events = new();

            if (filter != null)
            {
                foreach (SceneEvent sceneEvent in sceneEvents)
                    if (sceneEvent.eventID.Contains(filter))
                        events.Add(sceneEvent);
            }
            else
            {
                events = new(sceneEvents);
            }

            SceneEvent ev;
            for (;events.Count > 0;)
            {
                ev = events[UnityEngine.Random.Range(0, events.Count)];
                if (ev.Trigger())
                {
                    if (removeAfterTrigger) sceneEvents.Remove(ev);
                    return true;
                }
                events.Remove(ev);
            }

            return false;
        }
        #endregion
        
        #region Trigger a list of SceneEvents (Extension Method)
        /// <summary>
        /// Trigger every SceneEvent which eventID == ID in the list
        /// (Trigger all if ID == null)
        /// </summary>
        /// <param name="sceneEvents"></param>
        /// <param name="ID">ID of the SceneEvents to trigger</param>
        public static void Trigger(this List<SceneEvent> sceneEvents, string ID = null)
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;
            
            List<SceneEvent> events = new();

            if (ID != null)
            {
                events = sceneEvents.FindAll(e => e.eventID == ID);
            }
            else
            {
                events = new(sceneEvents);
            }

            foreach (var sceneEvent in events)
            {
                sceneEvent.Trigger();
            }
        }
        #endregion
        
        #region Trigger a list of SceneActions or SceneParameteredEvents (Extension Method)
        public static void Trigger(this List<SceneAction> sceneActions)
        {
            if (sceneActions == null || sceneActions.Count < 1) return;
            
            foreach (var action in sceneActions)
            {
                action.Trigger();
            }
        }
        public static void Trigger(this List<SceneParameteredEvent> sceneEvents)
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;
            
            foreach (var action in sceneEvents)
            {
                action.Trigger();
            }
        }
        #endregion
        
        #region Init a list of SceneParameteredEvents (Extension Method)
        public static void Init(this List<SceneParameteredEvent> sceneEvents)
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;
            
            foreach (var action in sceneEvents)
            {
                action.Init();
            }
        }
        public static void Init(this List<SceneEvent> sceneEvents)
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;
            
            foreach (var action in sceneEvents)
            {
                action.Init();
            }
        }
        #endregion
        #endregion
    }
}
