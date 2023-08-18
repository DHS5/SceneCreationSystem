using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Dhs5.SceneCreation
{
    [Serializable]
    public class SceneAction : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable
    {
        public SceneVariablesSO sceneVariablesSO;
        private SceneObject sceneObject;

        [SerializeField] private int var1UniqueID;
        public SceneVar SceneVar1 { get => SceneState.GetSceneVar(var1UniqueID); }
        private SceneVar EditorSceneVar1 { get => sceneVariablesSO[var1UniqueID]; }

        [SerializeField] private SceneVarTween SceneVar2;
        [SerializeField] private SceneVarType var2Type;

        // Operations        
        public BoolOperation boolOP;
        
        public IntOperation intOP;
        
        public FloatOperation floatOP;
        
        public StringOperation stringOP;

        public void SetUp(SceneVariablesSO sceneVariablesSO)
        {
            this.sceneVariablesSO = sceneVariablesSO;

            SceneVar2.SetUp(sceneVariablesSO, var2Type, true);
        }
        public void BelongTo(SceneObject _sceneObject)
        {
            sceneObject = _sceneObject;
        }
        
        public void Trigger()
        {
            if (SceneVar1 == null)
            {
                Debug.LogError("Trigger doesn't have a SceneVar");
                return;
            }

            switch (SceneVar1.type)
            {
                case SceneVarType.BOOL:
                    SceneState.ModifyBoolVar(var1UniqueID, boolOP, SceneVar2.BoolValue, sceneObject);
                    break;
                case SceneVarType.INT:
                    SceneState.ModifyIntVar(var1UniqueID, intOP, SceneVar2.IntValue, sceneObject);
                    break;
                case SceneVarType.FLOAT:
                    SceneState.ModifyFloatVar(var1UniqueID, floatOP, SceneVar2.FloatValue, sceneObject);
                    break;
                case SceneVarType.STRING:
                    SceneState.ModifyStringVar(var1UniqueID, stringOP, SceneVar2.StringValue, sceneObject);
                    break;
                case SceneVarType.EVENT:
                    SceneState.TriggerEventVar(var1UniqueID, sceneObject);
                    break;

                default:
                    break;
            }
        }

        #region Operation Description
        public static string BoolOpDescription(BoolOperation op)
        {
            switch (op)
            {
                case BoolOperation.SET: return " = ";
                case BoolOperation.INVERSE: return " Inverse.";
                default: return "";
            }
        }
        public static string IntOpDescription(IntOperation op)
        {
            switch (op)
            {
                case IntOperation.SET: return " = ";
                case IntOperation.ADD: return " += ";
                case IntOperation.SUBSTRACT: return " -= ";
                case IntOperation.MULTIPLY: return " *= ";
                case IntOperation.DIVIDE: return " /= ";
                case IntOperation.POWER: return " = power ";
                default: return "";
            }
        }
        public static string FloatOpDescription(FloatOperation op)
        {
            switch (op)
            {
                case FloatOperation.SET: return " = ";
                case FloatOperation.ADD: return " += ";
                case FloatOperation.SUBSTRACT: return " -= ";
                case FloatOperation.MULTIPLY: return " *= ";
                case FloatOperation.DIVIDE: return " /= ";
                case FloatOperation.POWER: return " = power ";
                default: return "";
            }
        }
        public static string StringOpDescription(StringOperation op)
        {
            switch (op)
            {
                case StringOperation.SET: return " = ";
                case StringOperation.APPEND: return " .Append ";
                case StringOperation.REMOVE: return " .Replace(param,'') ";
                default: return "";
            }
        }
        private string GetOpDescription()
        {
            switch (EditorSceneVar1.type)
            {
                case SceneVarType.BOOL: return BoolOpDescription(boolOP);
                case SceneVarType.INT: return IntOpDescription(intOP);
                case SceneVarType.FLOAT: return FloatOpDescription(floatOP);
                case SceneVarType.STRING: return StringOpDescription(stringOP);
                case SceneVarType.EVENT: return "Trigger";
                default: return null;
            }
        }
        #endregion

        #region Log
        public override string ToString()
        {
            StringBuilder sb = new();

            sb.Append(EditorSceneVar1.LogString());
            sb.Append(" ");
            sb.Append(GetOpDescription());
            sb.Append(" ");
            sb.Append(SceneVar2.LogString());

            return sb.ToString();
        }
        #endregion
    }
}
