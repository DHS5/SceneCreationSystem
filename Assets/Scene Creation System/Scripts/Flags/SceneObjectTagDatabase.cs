using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [CreateAssetMenu(fileName = "SceneObject Tag Database", menuName = "Scene Creation/Database/Tags")]
    public class SceneObjectTagDatabase : FlagDatabase
    {
        public static SceneObjectTagDatabase Instance
        {
            get
            {
#if UNITY_EDITOR
                return SceneCreationSettings.instance.SceneObjectSettings.TagDatabase;
#else
                if (Application.isPlaying)
                {
                    return SceneManager.Settings.SceneObjectTagDatabase;
                }

                return null;
#endif
            }
        }
    }
}
