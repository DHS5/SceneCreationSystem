using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public static class IntersceneState
    {
        #region Global SceneVars

        public static bool IsEmpty { get; private set; }

        private static Dictionary<int, SceneVar> IntersceneVariables = new();

        private static Dictionary<int, object> FormerValues = new();

        public static void SetGlobalVars(IntersceneVariablesSO intersceneVariablesSO)
        {
            if (!IsEmpty || intersceneVariablesSO == null) return;

            IsEmpty = false;

            SetIntersceneVars(intersceneVariablesSO.SceneVars);
        }

        private static void SetIntersceneVars(List<SceneVar> globalVars)
        {
            if (globalVars.IsValid())
                foreach (var var in globalVars)
                    AddGlobalVar(var);
        }

        private static void AddGlobalVar(SceneVar variable)
        {
            IntersceneVariables[variable.uniqueID] = new(variable);
        }

        #endregion

        private static void SaveFormerValues()
        {
            if (!IntersceneVariables.IsValid()) return;

            FormerValues.Clear();
            foreach (var pair in IntersceneVariables)
            {
                FormerValues[pair.Key] = pair.Value.Value;
            }
        }

        #region Global Vars Actions

        private static void ChangedVar(int varUniqueID, SceneObject sender, SceneContext context)
        {
            if (IntersceneVariables.ContainsKey(varUniqueID))
            {
                SceneEventManager.TriggerEvent(varUniqueID, new(IntersceneVariables[varUniqueID], FormerValues[varUniqueID], sender, context));
            }
            SceneState.CheckChangedLink(varUniqueID, sender, context);
        }

        public static void ModifyBoolVar(int varUniqueID, BoolOperation op, bool param, SceneObject sender, SceneContext context)
        {
            if (IntersceneVariables.ContainsKey(varUniqueID))
            {
                SceneVar var = IntersceneVariables[varUniqueID];
                if (var.type == SceneVarType.BOOL && !var.IsStatic && !var.IsLink)
                {
                    SaveFormerValues();
                    switch (op)
                    {
                        case BoolOperation.SET:
                            IntersceneVariables[varUniqueID].BoolValue = param;
                            break;
                        case BoolOperation.INVERSE:
                            IntersceneVariables[varUniqueID].BoolValue = !IntersceneVariables[varUniqueID].BoolValue;
                            break;
                        case BoolOperation.TO_TRUE:
                            IntersceneVariables[varUniqueID].BoolValue = true;
                            break;
                        case BoolOperation.TO_FALSE:
                            IntersceneVariables[varUniqueID].BoolValue = false;
                            break;
                        default:
                            IntersceneVariables[varUniqueID].BoolValue = param;
                            break;
                    }
                    ChangedVar(varUniqueID, sender, context);
                    return;
                }
                return;
            }
        }

        #endregion

        #region Main Variables

        /// <summary>
        /// Current level of balancing of the game
        /// </summary>
        public static int BalancingLevel { get; private set; }

        #endregion
    }
}
