 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public class SceneVarTween : SceneState.ISceneVarDependantWithProhibition, SceneState.ISceneObjectBelongable
    {
        [SerializeField] private SceneVarType type;
        public SceneVarType Type => type;

        [SerializeField] private SceneVariablesSO sceneVariablesSO;
        private SceneObject sceneObject;
        private SceneContext context;

        [SerializeField] private int sceneVarUniqueID;
        public int UID => sceneVarUniqueID;

        // Static
        [SerializeField] private bool anyVar;

        [SerializeField] private bool canBeStatic;
        [SerializeField] private bool isStatic;

        [SerializeField] private bool boolValue;
        [SerializeField] private int intValue;
        [SerializeField] private float floatValue;
        [SerializeField] private string stringValue;

        [SerializeField] private int forbiddenUID;

        [SerializeField] private float propertyHeight;

        private bool IsStatic => canBeStatic && isStatic;

        private SceneVar SceneVar
        {
            get => SceneState.GetSceneVar(sceneVarUniqueID);
        }
        private SceneVar EditorSceneVar
        {
            get => sceneVariablesSO[sceneVarUniqueID];
        }


        public void SetUp(SceneVariablesSO _sceneVariablesSO, SceneVarType _type, bool _canBeStatic = false, bool _anyVar = false)
        {
            anyVar = _anyVar;
            sceneVariablesSO = _sceneVariablesSO;
            type = _type;
            if (type != SceneVarType.EVENT) canBeStatic = _canBeStatic;
            else canBeStatic = false;
        }
        public void BelongTo(SceneObject _sceneObject)
        {
            sceneObject = _sceneObject;
            context = new(sceneObject.name);
            context.Add("Set var " + sceneVarUniqueID + " via SceneVarTween");
        }

        #region Values
        public object Value
        {
            get
            {
                switch (type)
                {
                    case SceneVarType.BOOL: return BoolValue;
                    case SceneVarType.INT: return IntValue;
                    case SceneVarType.FLOAT: return FloatValue;
                    case SceneVarType.STRING: return StringValue;
                    default: return null;
                }
            }
        }
        public bool BoolValue
        {
            get
            {
                if (IsStatic) return boolValue;
                if (SceneVar.type != SceneVarType.BOOL) IncorrectType(SceneVarType.BOOL);
                return SceneVar.BoolValue;
            }
            set
            {
                if (SceneVar.type != SceneVarType.BOOL)
                {
                    IncorrectType(SceneVarType.BOOL);
                    return;
                }
                if (IsStatic)
                {
                    boolValue = value;
                    return;
                }
                SceneState.ModifyBoolVar(sceneVarUniqueID, BoolOperation.SET, value, sceneObject, context.Add("Set to " + value));
            }
        }
        public int IntValue
        {
            get
            {
                if (IsStatic) return intValue;
                if (SceneVar.type == SceneVarType.FLOAT) return (int)SceneVar.FloatValue;
                if (SceneVar.type != SceneVarType.INT) IncorrectType(SceneVarType.INT);
                return SceneVar.IntValue;
            }
            set
            {
                if (SceneVar.type != SceneVarType.INT)
                {
                    IncorrectType(SceneVarType.INT);
                    return;
                }
                if (IsStatic)
                {
                    intValue = value;
                    return;
                }
                SceneState.ModifyIntVar(sceneVarUniqueID, IntOperation.SET, value, sceneObject, context.Add("Set to " + value));
            }
        }
        public float FloatValue
        {
            get
            {
                if (IsStatic) return floatValue;
                if (SceneVar.type == SceneVarType.INT) return SceneVar.IntValue;
                if (SceneVar.type != SceneVarType.FLOAT) IncorrectType(SceneVarType.FLOAT);
                return SceneVar.FloatValue;
            }
            set
            {
                if (SceneVar.type != SceneVarType.FLOAT)
                {
                    IncorrectType(SceneVarType.FLOAT);
                    return;
                }
                if (IsStatic)
                {
                    floatValue = value;
                    return;
                }
                SceneState.ModifyFloatVar(sceneVarUniqueID, FloatOperation.SET, value, sceneObject, context.Add("Set to " + value));
            }
        }
        public string StringValue
        {
            get
            {
                if (IsStatic) return stringValue;
                if (SceneVar.type != SceneVarType.STRING) IncorrectType(SceneVarType.STRING);
                return SceneVar.StringValue;
            }
            set
            {
                if (SceneVar.type != SceneVarType.STRING)
                {
                    IncorrectType(SceneVarType.STRING);
                    return;
                }
                if (IsStatic)
                {
                    stringValue = value;
                    return;
                }
                SceneState.ModifyStringVar(sceneVarUniqueID, StringOperation.SET, value, sceneObject, context.Add("Set to " + value));
            }
        }
        public void Trigger()
        {
            if (SceneVar.type != SceneVarType.EVENT)
            {
                IncorrectType(SceneVarType.EVENT);
                return;
            }
            SceneState.TriggerEventVar(sceneVarUniqueID, sceneObject, context.Add("Trigger"));
        }
        #endregion

        #region Dependencies

        public List<int> Dependencies
        {
            get
            {
                if (IsStatic) return new();
                SceneVar var = sceneVariablesSO[sceneVarUniqueID];
                if (var.IsLink)
                {
                    return sceneVariablesSO.GetComplexSceneVarWithUID(sceneVarUniqueID).Dependencies;
                }
                return new() { sceneVarUniqueID };
            }
        }
        public bool DependOn(int UID)
        {
            if ((canBeStatic && isStatic) || sceneVariablesSO[sceneVarUniqueID] == null) return false;
            if (sceneVarUniqueID == UID) return true;
            if (sceneVariablesSO[sceneVarUniqueID].IsLink)
            {
                return sceneVariablesSO.GetComplexSceneVarWithUID(sceneVarUniqueID).DependOn(UID);
            }
            return false;
        }
        public void SetForbiddenUID(int UID)
        {
            forbiddenUID = UID;
        }
        public bool IsLink(out int UID)
        {
            UID = 0;
            if (sceneVariablesSO[sceneVarUniqueID].IsLink) 
            {
                UID = sceneVarUniqueID;
                return true;
            }
            return false;
        }
        #endregion

        private void IncorrectType(SceneVarType type)
        {
            Debug.LogError("This SceneVarTween is a " + SceneVar.type + " and not a " + type);
        }

        #region Log
        public string LogString()
        {
            return IsStatic ? Value.ToString() : EditorSceneVar.LogString();
        }
        #endregion
    }
}
