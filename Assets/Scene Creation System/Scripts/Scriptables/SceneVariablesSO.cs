using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dhs5.SceneCreation
{
    [CreateAssetMenu(fileName = "SceneVars", menuName = "Scene Creation/Scene Vars")]
    public class SceneVariablesSO : ScriptableObject
    {
        [SerializeField] private SceneVariablesSO intersceneVariablesSO;

        [SerializeField] private List<SceneVar> sceneVars;

        [SerializeField] private List<ComplexSceneVar> complexSceneVars;

        public List<SceneVar> PureSceneVars => sceneVars.Copy();
        public List<ComplexSceneVar> ComplexSceneVars => complexSceneVars.Copy();

        public List<SceneVar> SceneVars => sceneVars;

        public SceneVar this[int uniqueID]
        {
            get
            {
                SceneVar sVar = SceneVars.Find(v => v.uniqueID == uniqueID);
                if (sVar == null) Debug.LogError("Can't find SceneVar from UID " + uniqueID);
                return sVar;
            }
        }

        #region Scene Vars

        public void AddSceneVarOfType(SceneVarType type)
        {
            sceneVars.Add(new(GenerateUniqueID(), type));
        }

        public void TryRemoveSceneVarAtIndex(int index)
        {
            if (sceneVars.IsIndexValid(index))
            {
                if (!sceneVars[index].IsLink)
                {
                    sceneVars.RemoveAt(index);
                }
                else
                {
                    Debug.LogError("Can't remove link");
                }
            }
            else
            {
                Debug.LogError("Invalid index");
            }
        }

        public bool CanRemoveAtIndex(int index)
        {
            return sceneVars.IsIndexValid(index) && !sceneVars[index].IsLink;
        }
        public bool IsLinkAtIndex(int index)
        {
            return sceneVars.IsIndexValid(index) && sceneVars[index].IsLink;
        }

        #endregion

        #region Complex Scene Vars

        public ComplexSceneVar GetComplexSceneVarWithUID(int UID)
        {
            ComplexSceneVar v = complexSceneVars.Find(x => x.uniqueID == UID);
            if (v == null)
            {
                Debug.LogError("No Complex Scene Var with UID " + UID + " found in " + name);
            }
            return v;
        }
        public bool TryGetComplexSceneVarWithUID(int UID, out ComplexSceneVar complexSceneVar)
        {
            complexSceneVar = GetComplexSceneVarWithUID(UID);

            return complexSceneVar != null;
        }

        public void AddComplexSceneVarOfType(ComplexSceneVarType type)
        {
            ComplexSceneVar complexVar = new(GenerateUniqueID(), type);

            complexSceneVars.Add(complexVar);
            sceneVars.Add(complexVar.Link);
        }

        public void TryRemoveComplexSceneVarAtIndex(int index)
        {
            if (complexSceneVars.IsIndexValid(index))
            {
                ComplexSceneVar v = complexSceneVars[index];
                
                if (sceneVars.Remove(v.Link))
                {
                    complexSceneVars.Remove(v);
                }
                else
                {
                    Debug.LogError("Link " + v.Link + " is not in the SceneVar list ? " + !sceneVars.Contains(v.Link));

                    //SceneVar v2 = sceneVars.Find(x => x.uniqueID == v.Link.uniqueID);
                    //if (v2 != null && sceneVars.Remove(v2))
                    //{
                    //    complexSceneVars.Remove(v);
                    //}
                    //else
                    //{
                    //    Debug.LogError("Can't find and remove link of " + v);
                    //}
                }
            }
            else
            {
                Debug.LogError("Invalid index");
            }
        }

        #endregion

        #region IDs
        public int GetUniqueIDByIndex(int index)
        {
            if (index < 0 || index >= SceneVars.Count) return 0;
            return SceneVars[index].uniqueID;
        }
        public int GetIndexByUniqueID(int uniqueID)
        {
            if (uniqueID == 0) return -1;
            return SceneVars.FindIndex(v => v.uniqueID == uniqueID);
        }
        
        private List<int> UniqueIDs
        {
            get
            {
                List<int> list = new();
                foreach (var var in SceneVars)
                {
                    list.Add(var.uniqueID);
                }
                return list;
            }
        }
        public List<string> IDs
        {
            get
            {
                List<string> list = new();
                foreach (var var in SceneVars)
                {
                    if (var.uniqueID != 0)
                        list.Add(var.ID);
                    else
                        list.Add("No unique ID");
                }
                return list;
            }
        }
        public List<string> SceneVarStrings
        {
            get
            {
                List<string> list = new();
                foreach (var var in SceneVars)
                {
                    if (var.uniqueID != 0)
                        list.Add(var.PopupString());
                    else
                        list.Add("No unique ID");
                }
                return list;
            }
        }
        
        public int GenerateUniqueID()
        {
            int uniqueID;
            List<int> uniqueIDs = UniqueIDs;
            do
            {
                uniqueID = Random.Range(1, 10000);
            } while (uniqueIDs.Contains(uniqueID));
            
            return uniqueID;
        }
        #endregion

        private void OnValidate()
        {
            ActuSceneVarLinks();

            complexSceneVars.SetUp(this);
            complexSceneVars.SetForbiddenUID(0);

#if UNITY_EDITOR
            CleanBalancingSheets();

            GetIntersceneVariablesSO();
#endif
        }
#if UNITY_EDITOR
        public void OnEditorEnable()
        {
            CleanBalancingSheets();

            GetIntersceneVariablesSO();
        }
#endif

        private void ActuSceneVarLinks()
        {
            // Browse on Complex Scene Vars
            foreach (var cVar in complexSceneVars)
            {
                // If link is null, find it or create one
                if (cVar.Link == null)
                {
                    SceneVar lostLink = sceneVars.Find(v => v.uniqueID == cVar.uniqueID);

                    if (lostLink == null)
                    {
                        Debug.LogError("No Scene Var Link found for Complex Scene Var" + cVar.uniqueID + " , creating one");
                        cVar.Link = SceneVar.CreateLink(cVar);
                        sceneVars.Add(cVar.Link);
                    }
                    else
                    {
                        cVar.Link = lostLink;
                    }
                }

                // If link exist but is not in the scene vars
                else if (!sceneVars.Contains(cVar.Link))
                {
                    SceneVar lostLink = sceneVars.Find(v => v.uniqueID == cVar.uniqueID);

                    if (lostLink == null)
                    {
                        Debug.LogError("Complex Scene Var link exist but can't find link in the SceneVar list, adding it");
                        sceneVars.Add(cVar.Link);
                    }
                    else if (cVar.Link != lostLink)
                    {
                        cVar.Link = lostLink;
                    }
                }
            }

            // Browse on Scene Vars
            foreach (var v in sceneVars.Copy())
            {
                if (v.IsLink && !TryGetComplexSceneVarWithUID(v.uniqueID, out _))
                {
                    sceneVars.Remove(v);
                }
            }
        }

        #region Interscene Variables

        /// <summary>
        /// Editor only function to get the interscene variables from the settings
        /// </summary>
        private void GetIntersceneVariablesSO()
        {
            intersceneVariablesSO = SceneCreationSettings.instance.SceneVars;
        }

        #endregion

        #region Lists
        public List<string> VarStrings(List<SceneVar> vars)
        {
            List<string> list = new();
            foreach (var var in vars)
            {
                if (var.uniqueID != 0)
                    list.Add(var.PopupString());
                else
                    list.Add("No unique ID");
            }
            return list;
        }
        public int GetUniqueIDByIndex(List<SceneVar> vars, int index)
        {
            if (index < 0 || index >= vars.Count) return 0;
            return vars[index].uniqueID;
        }
        public int GetIndexByUniqueID(List<SceneVar> vars, int uniqueID)
        {
            if (uniqueID == 0) return -1;
            return vars.FindIndex(v => v.uniqueID == uniqueID);
        }
        public List<SceneVar> GetListByType(SceneVarType type, bool precisely = false)
        {
            switch (type)
            {
                case SceneVarType.BOOL: return Booleans;
                case SceneVarType.INT: return precisely ? Integers : Numbers;
                case SceneVarType.FLOAT: return precisely ? Floats : Numbers;
                case SceneVarType.STRING: return Strings;
                case SceneVarType.EVENT: return Events;
                default: return SceneVars;
            }
        }

        public List<SceneVar> Statics
        {
            get => sceneVars != null ? sceneVars.FindAll(v => v.IsStatic) : null;
        }
        public List<SceneVar> Modifyables
        {
            get => sceneVars != null ? sceneVars.FindAll(v => !v.IsStatic && !v.IsRandom) : null;
        }
        public List<SceneVar> Listenables
        {
            get => SceneVars != null ? SceneVars.FindAll(v => !v.IsStatic && !v.IsRandom) : null;
        }
        public List<SceneVar> Conditionable
        {
            get => SceneVars != null ? SceneVars.FindAll(v => !v.IsStatic && v.type != SceneVarType.EVENT) : null;
        }
        public List<SceneVar> Booleans
        {
            get => SceneVars != null ? SceneVars.FindAll(v => v.type == SceneVarType.BOOL) : null;
        }
        public List<SceneVar> Numbers
        {
            get => SceneVars != null ? SceneVars.FindAll(v => v.type == SceneVarType.INT || v.type == SceneVarType.FLOAT) : null;
        }
        public List<SceneVar> Integers
        {
            get => SceneVars != null ? SceneVars.FindAll(v => v.type == SceneVarType.INT) : null;
        }
        public List<SceneVar> Floats
        {
            get => SceneVars != null ? SceneVars.FindAll(v => v.type == SceneVarType.FLOAT) : null;
        }
        public List<SceneVar> Strings
        {
            get => SceneVars != null ? SceneVars.FindAll(v => v.type == SceneVarType.STRING) : null;
        }
        public List<SceneVar> Events
        {
            get => sceneVars != null ? sceneVars.FindAll(v => v.type == SceneVarType.EVENT) : null;
        }
        public List<SceneVar> NonEvents
        {
            get => SceneVars != null ? SceneVars.FindAll(v => v.type != SceneVarType.EVENT) : null;
        }
        #endregion

        #region Dependency
        public List<SceneVar> CleanListOfCycleDependencies(List<SceneVar> list, int UID)
        {
            List<SceneVar> sceneVars = new();
            foreach (SceneVar v in list)
            {
                if (!v.IsLink || !complexSceneVars.Find(x => x.uniqueID == v.uniqueID).DependOn(UID))
                {
                    sceneVars.Add(v);
                }
            }
            return sceneVars;
        }
        #endregion

        #region Balancing Sheets
        public List<SceneBalancingSheetSO> sceneBalancingSheets;

        readonly string baseName = "BalancingSheet";

#if UNITY_EDITOR
        public void CreateNewBalancingSheet()
        {
            string path = AssetDatabase.GetAssetPath(this);
            path = path.Substring(0, path.LastIndexOf('/') + 1) + this.name + " BalancingSheets/";

            SceneBalancingSheetSO balancingSheet = ScriptableObject.CreateInstance<SceneBalancingSheetSO>();
            balancingSheet.sceneVariablesSO = this;
            sceneBalancingSheets.Add(balancingSheet);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            AssetDatabase.CreateAsset(balancingSheet, path + GetUniqueName(path) + ".asset");
        }

        private void CleanBalancingSheets()
        {
            if (sceneBalancingSheets == null || sceneBalancingSheets.Count == 0) return;
            foreach (var sheet in new List<SceneBalancingSheetSO>(sceneBalancingSheets))
            {
                if (sheet == null)
                {
                    sceneBalancingSheets.Remove(sheet);
                }
                if (sheet.sceneVariablesSO != this) sheet.sceneVariablesSO = this;
            }
        }

        private string GetUniqueName(string path)
        {
            List<string> names = new();
            string current;

            foreach (var name in Directory.GetFiles(path))
            {
                current = name.Remove(0, name.LastIndexOf('/') + 1);
                current = current.Substring(0, current.LastIndexOf('.'));
                names.Add(current);
            }

            if (names.Count < 1)
            {
                return baseName + "1";
            }

            int index = 1;

            while (names.Contains(baseName + index))
            {
                index++;
            }
            return baseName + index;
        }
#endif
        public List<SceneVar> BalancedSceneVars(int balancingIndex)
        {
            List<SceneVar> vars;

            if (!sceneBalancingSheets.IsValid()
                || balancingIndex <= 0
                || balancingIndex > sceneBalancingSheets.Count)
            {
                vars = PureSceneVars;
            }
            else
            {
                vars = sceneBalancingSheets[balancingIndex - 1].SceneVars;
            }

            return vars;

            //return sceneVarLinks != null ? vars.Concat(sceneVarLinks.Copy()).ToList() : vars;
        }
        #endregion
    }
}
