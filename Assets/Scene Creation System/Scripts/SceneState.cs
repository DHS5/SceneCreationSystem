using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using static UnityEngine.EventSystems.EventTrigger;
using System.Text;
using System.Runtime.CompilerServices;
using static Dhs5.SceneCreation.SceneState;

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
        SET, INVERSE, TO_TRUE, TO_FALSE
    }
    [Serializable]
    public enum BoolComparison
    {
        EQUAL, DIFF, IS_TRUE, IS_FALSE
    }
    [Serializable]
    public enum IntOperation
    {
        SET, ADD, SUBSTRACT, MULTIPLY, DIVIDE, POWER, TO_MIN, TO_MAX, TO_NULL, INCREMENT, DECREMENT
    }
    [Serializable]
    public enum IntComparison
    {
        EQUAL, DIFF, SUP, INF, SUP_EQUAL, INF_EQUAL, IS_MIN, IS_MAX, IS_NULL, IS_POSITIVE, IS_NEGATIVE
    }
    [Serializable]
    public enum FloatOperation
    {
        SET, ADD, SUBSTRACT, MULTIPLY, DIVIDE, POWER, TO_MIN, TO_MAX, TO_NULL, INCREMENT, DECREMENT
    }
    [Serializable]
    public enum FloatComparison
    {
        EQUAL, DIFF, SUP, INF, SUP_EQUAL, INF_EQUAL, IS_MIN, IS_MAX, IS_NULL, IS_POSITIVE, IS_NEGATIVE
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
        private static List<SceneObject> sceneObjects = new();

        private static Dictionary<int, SceneVar> SceneVariables = new();
        private static Dictionary<int, ComplexSceneVar> ComplexSceneVariables = new();
        private static Dictionary<int, List<int>> SceneVarLinks = new();
        private static Dictionary<int, object> FormerValues = new();

        #region Scene Object Registration
        public static void Register(SceneObject sceneObject)
        {
            if (sceneObjects.Contains(sceneObject)) return;

            sceneObjects.Add(sceneObject);
        }
        public static void Unregister(SceneObject sceneObject)
        {
            if (!sceneObjects.Contains(sceneObject)) return;

            sceneObjects.Remove(sceneObject);
        }
        public static void StartScene()
        {
            if (sceneObjects.IsValid()) return;

            foreach (SceneObject sceneObject in sceneObjects)
            {
                if (sceneObject != null && sceneObject.enabled && sceneObject.DoStartScene)
                    sceneObject.OnStartScene();
            }
        }
        public static void ChangeScene()
        {
            if (sceneObjects.IsValid()) return;

            foreach (SceneObject sceneObject in sceneObjects)
            {
                if (sceneObject != null && sceneObject.enabled && sceneObject.DoChangeScene)
                    sceneObject.OnChangeScene();
            }
        }
        public static void GameOver()
        {
            if (sceneObjects.IsValid()) return;

            foreach (SceneObject sceneObject in sceneObjects)
            {
                if (sceneObject != null && sceneObject.enabled && sceneObject.DoGameOver)
                    sceneObject.OnGameOver();
            }
        }
        #endregion

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

        private static void ChangedVar(int varUniqueID, SceneObject sender, SceneContext context)
        {
            if (SceneVariables.ContainsKey(varUniqueID))
            {
                SceneEventManager.TriggerEvent(varUniqueID, new(SceneVariables[varUniqueID], FormerValues[varUniqueID], sender, context));
            }
            if (SceneVarLinks.ContainsKey(varUniqueID))
            {
                foreach (var complexUID in SceneVarLinks[varUniqueID])
                {
                    ChangedComplexVar(complexUID, sender, context);
                }
            }
        }
        private static void ChangedComplexVar(int complexUID, SceneObject sender, SceneContext context)
        {
            if (ComplexSceneVariables.ContainsKey(complexUID))
            {
                SceneEventManager.TriggerEvent(complexUID, new(SceneVariables[complexUID], FormerValues[complexUID], sender, context));
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

        public static void ModifyBoolVar(int varUniqueID, BoolOperation op, bool param, SceneObject sender, SceneContext context)
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
                        case BoolOperation.TO_TRUE:
                            SceneVariables[varUniqueID].BoolValue = true;
                            break;
                        case BoolOperation.TO_FALSE:
                            SceneVariables[varUniqueID].BoolValue = false;
                            break;
                        default:
                            SceneVariables[varUniqueID].BoolValue = param;
                            break;
                    }
                    ChangedVar(varUniqueID, sender, context);
                    return;
                }
                IncorrectType(varUniqueID, SceneVarType.BOOL);
                return;
            }
            IncorrectID(varUniqueID);
        }
        public static void ModifyIntVar(int varUniqueID, IntOperation op, int param, SceneObject sender, SceneContext context)
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
                            var.IntValue = param;
                            break;
                        case IntOperation.ADD:
                            var.IntValue += param;
                            break;
                        case IntOperation.SUBSTRACT:
                            var.IntValue -= param;
                            break;
                        case IntOperation.MULTIPLY:
                            var.IntValue *= param;
                            break;
                        case IntOperation.DIVIDE:
                            var.IntValue /= param;
                            break;
                        case IntOperation.POWER:
                            var.IntValue = (int)Mathf.Pow(var.IntValue, param);
                            break;
                        case IntOperation.TO_MIN:
                            if (!var.hasMin) return;
                            var.IntValue = var.minInt;
                            break;
                        case IntOperation.TO_MAX:
                            if (!var.hasMax) return;
                            var.IntValue = var.maxInt;
                            break;
                        case IntOperation.TO_NULL:
                            var.IntValue = 0;
                            break;
                        case IntOperation.INCREMENT:
                            var.IntValue++;
                            break;
                        case IntOperation.DECREMENT:
                            var.IntValue--;
                            break;
                        
                        default:
                            var.IntValue = param;
                            break;
                    }
                    if (var.hasMin || var.hasMax)
                    {
                        var.IntValue = (int)Mathf.Clamp(var.IntValue,
                            var.hasMin ? var.minInt : -Mathf.Infinity,
                            var.hasMax ? var.maxInt : Mathf.Infinity);
                    }

                    ChangedVar(varUniqueID, sender, context);
                    return;
                }
                IncorrectType(varUniqueID, SceneVarType.INT);
                return;
            }
            IncorrectID(varUniqueID);
        }
        public static void ModifyFloatVar(int varUniqueID, FloatOperation op, float param, SceneObject sender, SceneContext context)
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
                            var.FloatValue = param;
                            break;
                        case FloatOperation.ADD:
                            var.FloatValue += param;
                            break;
                        case FloatOperation.SUBSTRACT:
                            var.FloatValue -= param;
                            break;
                        case FloatOperation.MULTIPLY:
                            var.FloatValue *= param;
                            break;
                        case FloatOperation.DIVIDE:
                            var.FloatValue /= param;
                            break;
                        case FloatOperation.POWER:
                            var.FloatValue = Mathf.Pow(SceneVariables[varUniqueID].FloatValue, param);
                            break;
                        case FloatOperation.TO_MIN:
                            if (!var.hasMin) return;
                            var.FloatValue = var.minFloat;
                            break;
                        case FloatOperation.TO_MAX:
                            if (!var.hasMax) return;
                            var.FloatValue = var.maxFloat;
                            break;
                        case FloatOperation.TO_NULL:
                            var.FloatValue = 0;
                            break;
                        case FloatOperation.INCREMENT:
                            var.FloatValue++;
                            break;
                        case FloatOperation.DECREMENT:
                            var.FloatValue--;
                            break;

                        default:
                            var.FloatValue = param;
                            break;
                    }
                    if (var.hasMin || var.hasMax)
                    {
                        var.FloatValue = (int)Mathf.Clamp(var.FloatValue,
                            var.hasMin ? var.minFloat : -Mathf.Infinity,
                            var.hasMax ? var.maxFloat : Mathf.Infinity);
                    }

                    ChangedVar(varUniqueID, sender, context);
                    return;
                }
                IncorrectType(varUniqueID, SceneVarType.FLOAT);
                return;
            }
            IncorrectID(varUniqueID);
        }
        public static void ModifyStringVar(int varUniqueID, StringOperation op, string param, SceneObject sender, SceneContext context)
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
                    ChangedVar(varUniqueID, sender, context);
                    return;
                }
                IncorrectType(varUniqueID, SceneVarType.STRING);
                return;
            }
            IncorrectID(varUniqueID);
        }
        public static void TriggerEventVar(int varUniqueID, SceneObject sender, SceneContext context)
        {
            if (SceneVariables.ContainsKey(varUniqueID))
            {
                SceneVar var = SceneVariables[varUniqueID];
                if (var.type == SceneVarType.EVENT)
                {
                    SaveFormerValues();
                    ChangedVar(varUniqueID, sender, context);
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
        #region Utility
        public static bool IsValid<T>(this List<T> list)
        {
            return list != null && list.Count > 0;
        }
        public static bool IsValid<T>(this T[] array)
        {
            return array != null && array.Length > 0;
        }
        public static bool IsValid<T>(this Stack<T> stack)
        {
            return stack != null && stack.Count > 0;
        }
        public static bool IsValid<T>(this Queue<T> queue)
        {
            return queue != null && queue.Count > 0;
        }
        public static bool IsValid<T, U>(this Dictionary<T, U> dico)
        {
            return dico != null && dico.Count > 0;
        }
        public static bool IsReallyValid<T, U>(this Dictionary<T, List<U>> dico)
        {
            if (dico == null || dico.Count <= 0) return false;
            foreach (var pair in dico)
            {
                if (pair.Value.IsValid()) return true;
            }
            return false;
        }
        #endregion

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
                setupable?.SetUp(sceneVariablesSO);
            }
        }
        public static void SetUp<T>(this List<T> setupables, SceneVariablesSO sceneVariablesSO, SceneVarType type) where T : ISceneVarTypedSetupable
        {
            if (setupables == null || setupables.Count < 1) return;

            foreach (var setupable in setupables)
            {
                setupable?.SetUp(sceneVariablesSO, type);
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

        #region Inits
        public interface IInitializable
        {
            public void Init();
        }
        public static void Init<T>(this List<T> initializables) where T : IInitializable
        {
            if (initializables == null || initializables.Count < 1) return;

            foreach (var initializable in initializables)
            {
                initializable.Init();
            }
        }
        #endregion

        #region Dependencies
        public interface ISceneVarDependant
        {
            public List<int> Dependencies { get; }
            public bool DependOn(int UID);
            public void SetForbiddenUID(int UID);
        }
        public static List<int> Dependencies<T>(this List<T> list) where T : ISceneVarDependant
        {
            if (!list.IsValid())
            {
                return new();
            }
            List<int> dependencies = new();
            foreach (var dependant in list)
            {
                if (dependant.Dependencies.IsValid())
                {
                    foreach (var dep in dependant.Dependencies)
                    {
                        if (!dependencies.Contains(dep))
                            dependencies.Add(dep);
                    }
                }
            }
            return dependencies;
        }
        public static bool DependOn<T>(this List<T> list, int UID) where T : ISceneVarDependant
        {
            foreach (var dependant in list)
            {
                if (!dependant.DependOn(UID))
                    return true;
            }
            return false;
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
        public static bool TriggerRandom<T>(this List<T> sceneEvents, string filter = null, bool removeAfterTrigger = false) where T : BaseSceneEvent
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return false;

            List<T> events = new();

            if (filter != null)
            {
                foreach (var sceneEvent in sceneEvents)
                    if (sceneEvent.eventID.Contains(filter))
                        events.Add(sceneEvent);
            }
            else
            {
                events = new(sceneEvents);
            }

            T ev;
            for (; events.Count > 0;)
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
        /// <summary>
        /// Triggers a random SceneEvent in the list
        /// </summary>
        /// <param name="sceneEvents"></param>
        /// <param name="filter">Trigger a random SceneEvent among ones which eventID contains filter</param>
        /// <returns>Whether an event was triggered</returns>
        public static bool TriggerRandom<T>(this List<SceneEvent<T>> sceneEvents, T value = default, string filter = null, bool removeAfterTrigger = false)
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return false;

            List<SceneEvent<T>> events = new();

            if (filter != null)
            {
                foreach (SceneEvent<T> sceneEvent in sceneEvents)
                    if (sceneEvent.eventID.Contains(filter))
                        events.Add(sceneEvent);
            }
            else
            {
                events = new(sceneEvents);
            }

            SceneEvent<T> ev;
            for (;events.Count > 0;)
            {
                ev = events[UnityEngine.Random.Range(0, events.Count)];
                if (ev.Trigger(value))
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
        public static void Trigger<T>(this List<T> sceneEvents, string ID = null) where T : BaseSceneEvent
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;

            List<T> events = new();

            if (!string.IsNullOrEmpty(ID))
            {
                events = sceneEvents.FindAll(e => e.eventID == ID);
            }
            else
            {
                events = new(sceneEvents);
            }

            if (events == null || events.Count < 1) return;

            foreach (var sceneEvent in events)
            {
                sceneEvent.Trigger();
            }
        }
        /// <summary>
        /// Trigger every SceneEvent which eventID == ID in the list<br></br>
        /// (Trigger all if ID == null)
        /// </summary>
        /// <param name="sceneEvents"></param>
        /// <param name="ID">ID of the SceneEvents to trigger</param>
        public static void Trigger(this List<SceneEvent> sceneEvents, string ID = null)
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;
            
            List<SceneEvent> events = new();

            if (!string.IsNullOrEmpty(ID))
            {
                events = sceneEvents.FindAll(e => e.eventID == ID);
            }
            else
            {
                events = new(sceneEvents);
            }

            if (events == null || events.Count < 1) return;

            foreach (var sceneEvent in events)
            {
                sceneEvent.Trigger();
            }
        }
        /// <summary>
        /// Trigger every SceneEvent which eventID == ID in the list<br></br>
        /// (Trigger all if ID == null)
        /// </summary>
        /// <typeparam name="T">SceneEvent type param</typeparam>
        /// <param name="sceneEvents"></param>
        /// <param name="value">Value to pass to SceneEvents (type T)</param>
        /// <param name="ID">ID of the SceneEvents to trigger</param>
        public static void Trigger<T>(this List<SceneEvent<T>> sceneEvents, T value, string ID = null)
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;
            
            List<SceneEvent<T>> events = new();

            if (!string.IsNullOrEmpty(ID))
            {
                events = sceneEvents.FindAll(e => e.eventID == ID);
            }
            else
            {
                events = new(sceneEvents);
            }

            if (events == null || events.Count < 1) return;

            foreach (var sceneEvent in events)
            {
                sceneEvent.Trigger(value);
            }
        }
        public static void TriggerAndRemove<T>(this List<T> sceneEvents, string ID, int triggerNumber) where T : BaseSceneEvent
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;

            if (string.IsNullOrEmpty(ID)) return;

            List<T> events = new();
            events = sceneEvents.FindAll(e => e.eventID == ID);

            foreach (var sceneEvent in events)
            {
                sceneEvent.Trigger(triggerNumber);
                if (sceneEvent.TriggerNumberLeft == 0)
                    sceneEvents.Remove(sceneEvent);
            }
        }
        /// <summary>
        /// Triggers one (or more) <see cref="SceneEvent"/> with the eventID <paramref name="ID"/><br></br>
        /// and remove from list after triggering <paramref name="triggerNumber"/> times
        /// </summary>
        /// <param name="sceneEvents"></param>
        /// <param name="ID"></param>
        /// <param name="triggerNumber"></param>
        public static void TriggerAndRemove(this List<SceneEvent> sceneEvents, string ID, int triggerNumber)
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;

            if (string.IsNullOrEmpty(ID)) return;

            List<SceneEvent> events = new();
            events = sceneEvents.FindAll(e => e.eventID == ID);

            foreach (var sceneEvent in events)
            {
                sceneEvent.Trigger(triggerNumber);
                if (sceneEvent.TriggerNumberLeft == 0)
                    sceneEvents.Remove(sceneEvent);
            }
        }
        public static void TriggerAndRemove<T>(this List<SceneEvent<T>> sceneEvents, T value, string ID, int triggerNumber)
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;

            if (string.IsNullOrEmpty(ID)) return;

            List<SceneEvent<T>> events = new();
            events = sceneEvents.FindAll(e => e.eventID == ID);

            foreach (var sceneEvent in events)
            {
                sceneEvent.Trigger(value, triggerNumber);
                if (sceneEvent.TriggerNumberLeft == 0)
                    sceneEvents.Remove(sceneEvent);
            }
        }
        public static void TriggerAndRemoveAll<T>(this List<T> sceneEvents, int triggerNumber) where T : BaseSceneEvent
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;

            List<T> events = new(sceneEvents);

            foreach (var sceneEvent in events)
            {
                sceneEvent.Trigger(triggerNumber);
                if (sceneEvent.TriggerNumberLeft == 0)
                    sceneEvents.Remove(sceneEvent);
            }
        }
        /// <summary>
        /// Triggers one (or more) <see cref="SceneEvent"/> with the eventID <paramref name="ID"/><br></br>
        /// and remove from list after triggering <paramref name="triggerNumber"/> times
        /// </summary>
        /// <param name="sceneEvents"></param>
        /// <param name="triggerNumber"></param>
        public static void TriggerAndRemoveAll(this List<SceneEvent> sceneEvents, int triggerNumber)
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;

            List<SceneEvent> events = new(sceneEvents);

            foreach (var sceneEvent in events)
            {
                sceneEvent.Trigger(triggerNumber);
                if (sceneEvent.TriggerNumberLeft == 0)
                    sceneEvents.Remove(sceneEvent);
            }
        }
        public static void TriggerAndRemoveAll<T>(this List<SceneEvent<T>> sceneEvents, T value, int triggerNumber)
        {
            if (sceneEvents == null || sceneEvents.Count < 1) return;

            List<SceneEvent<T>> events = new(sceneEvents);

            foreach (var sceneEvent in events)
            {
                sceneEvent.Trigger(value, triggerNumber);
                if (sceneEvent.TriggerNumberLeft == 0)
                    sceneEvents.Remove(sceneEvent);
            }
        }
        #endregion

        #region Trigger a list of SceneActions or SceneParameteredEvents (Extension Method)
        public static void Trigger(this List<SceneAction> sceneActions, SceneContext context)
        {
            if (sceneActions == null || sceneActions.Count < 1) return;
            
            foreach (var action in sceneActions)
            {
                action.Trigger(context);
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

        #region SceneListeners Registration
        public interface ISceneSubscribable
        {
            public void Subscribe();
            public void Unsubscribe();
        }
        public static void Subscribe<T>(this List<T> subscribables) where T : ISceneSubscribable
        {
            if (subscribables == null || subscribables.Count <= 0) return;

            foreach (var subscribable in subscribables)
            {
                subscribable.Subscribe();
            }
        }
        public static void Unsubscribe<T>(this List<T> subscribables) where T : ISceneSubscribable
        {
            if (subscribables == null || subscribables.Count <= 0) return;

            foreach (var subscribable in subscribables)
            {
                subscribable.Unsubscribe();
            }
        }
        #endregion

        #region SceneSpecificListeners Set Events
        public static void SetEvents(this List<SceneSpecificListener> listeners, Action<SceneEventParam> _events)
        {
            if (listeners == null || listeners.Count <= 0) return;

            foreach (var listener in listeners)
            {
                listener.SetEvents(_events);
            }
        }
        #endregion

        #region SceneObject Trigger
        public static void Trigger(this SceneObject sceneObject, SceneListener.SceneEventTrigger trigger, SceneEventParam param)
        {
            if (!trigger.random)
            {
                sceneObject.TriggerSceneEvent(trigger.eventID, param);
            }
            else
            {
                if (!trigger.remove)
                {
                    sceneObject.TriggerRandom(trigger.eventID, param);
                }
                else
                {
                    sceneObject.TriggerRandomAndRemove(trigger.eventID, param);
                }
            }
        }
        public static void Trigger(this SceneObject sceneObject, List<SceneListener.SceneEventTrigger> triggers, SceneEventParam param)
        {
            if (triggers == null || triggers.Count <= 0) return;

            foreach (var trigger in triggers)
            {
                sceneObject.Trigger(trigger, param);
            }
        }
        #endregion

        #region Profiles Management
        public static void Attach<T>(this List<T> list, SceneObject _sceneObject) where T : SceneProfile
        {
            if (list == null || list.Count <= 0) return;

            foreach (var item in list)
            {
                item.Attach(_sceneObject);
            }
        }
        public static void Detach<T>(this List<T> list) where T : SceneProfile
        {
            if (list == null || list.Count <= 0) return;

            foreach (var item in list)
            {
                item.Detach();
            }
        }
        #endregion

        #region SceneScriptableObjects management
        public static void Link<T>(this List<T> scriptables, SceneObject sceneObject) where T : SceneScriptableObject
        {
            if (scriptables.IsValid())
            {
                foreach (var item in scriptables)
                    item.Link(sceneObject);
            }
        }
        public static void OnSceneObjectEnable<T>(this List<T> scriptables) where T : SceneScriptableObject
        {
            if (scriptables.IsValid())
            {
                foreach (var item in scriptables)
                    item.OnSceneObjectEnable();
            }
        }
        public static void OnSceneObjectDisable<T>(this List<T> scriptables) where T : SceneScriptableObject
        {
            if (scriptables.IsValid())
            {
                foreach (var item in scriptables)
                    item.OnSceneObjectDisable();
            }
        }
        #endregion

        #region Enum Second Parameter
        #region Operations
        public static bool HasSecondParameter(this BoolOperation boolOp)
        {
            return boolOp switch
            {
                BoolOperation.SET => true,
                BoolOperation.INVERSE => false,
                BoolOperation.TO_TRUE => false,
                BoolOperation.TO_FALSE => false,
                _ => false,
            };
        }
        public static bool HasSecondParameter(this IntOperation intOp)
        {
            return intOp switch
            {
                IntOperation.SET => true,
                IntOperation.ADD => true,
                IntOperation.SUBSTRACT => true,
                IntOperation.MULTIPLY => true,
                IntOperation.DIVIDE => true,
                IntOperation.POWER => true,
                IntOperation.TO_MIN => false,
                IntOperation.TO_MAX => false,
                IntOperation.TO_NULL => false,
                IntOperation.INCREMENT => false,
                IntOperation.DECREMENT => false,
                _ => false,
            };
        }
        public static bool HasSecondParameter(this FloatOperation floatOp)
        {
            return floatOp switch
            {
                FloatOperation.SET => true,
                FloatOperation.ADD => true,
                FloatOperation.SUBSTRACT => true,
                FloatOperation.MULTIPLY => true,
                FloatOperation.DIVIDE => true,
                FloatOperation.POWER => true,
                FloatOperation.TO_MIN => false,
                FloatOperation.TO_MAX => false,
                FloatOperation.TO_NULL => false,
                FloatOperation.INCREMENT => false,
                FloatOperation.DECREMENT => false,
                _ => false,
            };
        }
        #endregion

        #region Comparisons
        public static bool HasSecondParameter(this BoolComparison boolComp)
        {
            return boolComp switch
            { 
                BoolComparison.EQUAL => true,
                BoolComparison.DIFF => true,
                BoolComparison.IS_TRUE => false,
                BoolComparison.IS_FALSE => false,
                _ => false,
            };
        }
        public static bool HasSecondParameter(this IntComparison intComp)
        {
            return intComp switch
            { 
                IntComparison.EQUAL => true,
                IntComparison.DIFF => true,
                IntComparison.SUP => true,
                IntComparison.INF => true,
                IntComparison.SUP_EQUAL => true,
                IntComparison.INF_EQUAL => true,
                IntComparison.IS_MIN => false,
                IntComparison.IS_MAX => false,
                IntComparison.IS_NULL => false,
                IntComparison.IS_POSITIVE => false,
                IntComparison.IS_NEGATIVE => false,
                _ => false,
            };
        }
        public static bool HasSecondParameter(this FloatComparison floatComp)
        {
            return floatComp switch
            { 
                FloatComparison.EQUAL => true,
                FloatComparison.DIFF => true,
                FloatComparison.SUP => true,
                FloatComparison.INF => true,
                FloatComparison.SUP_EQUAL => true,
                FloatComparison.INF_EQUAL => true,
                FloatComparison.IS_MIN => false,
                FloatComparison.IS_MAX => false,
                FloatComparison.IS_NULL => false,
                FloatComparison.IS_POSITIVE => false,
                FloatComparison.IS_NEGATIVE => false,
                _ => false,
            };
        }
        public static bool HasSecondParameter(this StringComparison stringComp)
        {
            return stringComp switch
            { 
                StringComparison.EQUAL => true,
                StringComparison.DIFF => true,
                StringComparison.CONTAINS => true,
                StringComparison.CONTAINED => true,
                StringComparison.NULL_EMPTY => false,
                _ => false,
            };
        }
        #endregion
        #endregion

        #region Enum Description
        #region Comparisons
        public static string Description(this BoolComparison comp)
        {
            switch (comp)
            {
                case BoolComparison.EQUAL: return " == ";
                case BoolComparison.DIFF: return " != ";
                case BoolComparison.IS_TRUE: return " == True";
                case BoolComparison.IS_FALSE: return " == False";
                default: return "";
            }
        }
        public static string Description(this IntComparison comp)
        {
            switch (comp)
            {
                case IntComparison.EQUAL: return " == ";
                case IntComparison.DIFF: return " != ";
                case IntComparison.SUP: return " > ";
                case IntComparison.INF: return " < ";
                case IntComparison.SUP_EQUAL: return " >= ";
                case IntComparison.INF_EQUAL: return " <= ";
                case IntComparison.IS_MIN: return " == min";
                case IntComparison.IS_MAX: return " == max";
                case IntComparison.IS_NULL: return " == 0";
                case IntComparison.IS_POSITIVE: return " > 0";
                case IntComparison.IS_NEGATIVE: return " < 0";
                default: return "";
            }
        }
        public static string Description(this FloatComparison comp)
        {
            switch (comp)
            {
                case FloatComparison.EQUAL: return " == ";
                case FloatComparison.DIFF: return " != ";
                case FloatComparison.SUP: return " > ";
                case FloatComparison.INF: return " < ";
                case FloatComparison.SUP_EQUAL: return " >= ";
                case FloatComparison.INF_EQUAL: return " <= ";
                case FloatComparison.IS_MIN: return " == min ";
                case FloatComparison.IS_MAX: return " == max ";
                case FloatComparison.IS_NULL: return " == 0";
                case FloatComparison.IS_POSITIVE: return " > 0";
                case FloatComparison.IS_NEGATIVE: return " < 0";
                default: return "";
            }
        }
        public static string Description(this StringComparison comp)
        {
            switch (comp)
            {
                case StringComparison.EQUAL: return " == ";
                case StringComparison.DIFF: return " != ";
                case StringComparison.CONTAINS: return " Contains : ";
                case StringComparison.CONTAINED: return " Contained in : ";
                case StringComparison.NULL_EMPTY: return " is null or empty. ";
                default: return "";
            }
        }
        #endregion

        #region Operations
        public static string Description(this BoolOperation op)
        {
            switch (op)
            {
                case BoolOperation.SET: return " = ";
                case BoolOperation.INVERSE: return " Inverse.";
                case BoolOperation.TO_TRUE: return " = True";
                case BoolOperation.TO_FALSE: return " = False";
                default: return "";
            }
        }
        public static string Description(this IntOperation op)
        {
            switch (op)
            {
                case IntOperation.SET: return " = ";
                case IntOperation.ADD: return " += ";
                case IntOperation.SUBSTRACT: return " -= ";
                case IntOperation.MULTIPLY: return " *= ";
                case IntOperation.DIVIDE: return " /= ";
                case IntOperation.POWER: return " = power ";
                case IntOperation.TO_MIN: return " = min ";
                case IntOperation.TO_MAX: return " = max ";
                case IntOperation.TO_NULL: return " = 0 ";
                case IntOperation.INCREMENT: return " ++";
                case IntOperation.DECREMENT: return " --";
                default: return "";
            }
        }
        public static string Description(this FloatOperation op)
        {
            switch (op)
            {
                case FloatOperation.SET: return " = ";
                case FloatOperation.ADD: return " += ";
                case FloatOperation.SUBSTRACT: return " -= ";
                case FloatOperation.MULTIPLY: return " *= ";
                case FloatOperation.DIVIDE: return " /= ";
                case FloatOperation.POWER: return " = power ";
                case FloatOperation.TO_MIN: return " = min ";
                case FloatOperation.TO_MAX: return " = max ";
                case FloatOperation.TO_NULL: return " = 0 ";
                case FloatOperation.INCREMENT: return " ++";
                case FloatOperation.DECREMENT: return " --";
                default: return "";
            }
        }
        public static string Description(this StringOperation op)
        {
            switch (op)
            {
                case StringOperation.SET: return " = ";
                case StringOperation.APPEND: return " .Append ";
                case StringOperation.REMOVE: return " .Replace(param,'') ";
                default: return "";
            }
        }
        #endregion
        #endregion
        #endregion
    }
}
