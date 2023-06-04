using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public class SceneBalancingSheetSO : ScriptableObject
    {
        public SceneVariablesSO sceneVariablesSO;

        public List<BalancingVar> balancingVars;

        public List<SceneVar> SceneVars => CreateSceneVarList();

        #region SceneVar List Creation
        private List<SceneVar> CreateSceneVarList()
        {
            List<SceneVar> list = new();

            for (int i = 0; i < sceneVariablesSO.sceneVars.Count; i++)
            {
                if (balancingVars[i].overrideVar)
                {
                    list.Add(new(sceneVariablesSO.sceneVars[i], balancingVars[i]));
                }
                else
                {
                    list.Add(sceneVariablesSO.sceneVars[i]);
                }
            }

            return list;
        }
        #endregion

        #region Template management
        public void ApplyTemplate()
        {
            if (balancingVars == null) balancingVars = new();
            balancingVars = ApplyTemplate(balancingVars, sceneVariablesSO.sceneVars);
        }
        private List<BalancingVar> ApplyTemplate(List<BalancingVar> list, List<SceneVar> vars)
        {
            BalancingVar temp;
            Dictionary<int, int> indexes = new();

            List<BalancingVar> newList = new();
            for (int i = 0; i < vars.Count; i++)
            {
                temp = new(vars[i]);
                newList.Add(temp);
                indexes[vars[i].uniqueID] = i;
            }

            foreach (var var in list)
            {
                if (indexes.ContainsKey(var.uniqueID))
                {
                    newList[indexes[var.uniqueID]] = var;
                }
            }

            return newList;
        }
        #endregion
    }
}
