using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dhs5.SceneCreation
{
    [CreateAssetMenu(fileName = "SceneObject Settings", menuName = "Scene Creation/Settings/SceneObject Settings")]
    public class SceneObjectSettings : ScriptableObject
    {
        [SerializeField] private SceneObjectTagDatabase _tagDatabase;
        internal SceneObjectTagDatabase TagDatabase => _tagDatabase;

        [SerializeField] private SceneObjectLayerDatabase _layerDatabase;
        internal SceneObjectLayerDatabase LayerDatabase => _layerDatabase;


        readonly string TagDatabaseName = "SceneObject Tag Database";
        readonly string LayerDatabaseName = "SceneObject Layer Database";

        internal void SetupProject()
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(this);

            UnityEngine.Object[] os = AssetDatabase.LoadAllAssetsAtPath(path);
            List<UnityEngine.Object> objects = os.ToList();

            foreach (UnityEngine.Object o in os)
            {
                Debug.Log(o);
            }

            _tagDatabase = SetupObject<SceneObjectTagDatabase>(_tagDatabase, TagDatabaseName);

            _layerDatabase = SetupObject<SceneObjectLayerDatabase>(_layerDatabase, LayerDatabaseName);

            AssetDatabase.SaveAssets();

            T SetupObject<T>(UnityEngine.Object obj, string correctName) where T : ScriptableObject
            {
                if (obj == null)
                {
                    UnityEngine.Object foundObj = objects.Find(o => o is T);
                    if (foundObj != null)
                    {
                        obj = foundObj;
                    }
                    else
                    {
                        obj = CreateInstance<T>();
                        AssetDatabase.AddObjectToAsset(obj, path);
                        obj.name = correctName;
                    }
                }
                else if (!objects.Contains(obj))
                {
                    UnityEngine.Object newObj = Instantiate(obj);
                    AssetDatabase.AddObjectToAsset(newObj, path);
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(obj));
                    obj = newObj;
                }

                if (obj != null)
                {
                    obj.hideFlags = HideFlags.HideInHierarchy;
                }

                return obj == null ? null : obj as T;
            }
#endif
        }
    }
}
