using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [CreateAssetMenu(fileName = "SceneObject Settings", menuName = "Scene Creation/Settings/SceneObject Settings")]
    public class SceneObjectSettings : ScriptableObject
    {
        [SerializeField] private SceneObjectTagDatabase _tagDatabase;
        internal SceneObjectTagDatabase TagDatabase => _tagDatabase;

        [SerializeField] private SceneObjectLayerDatabase _layerDatabase;
        internal SceneObjectLayerDatabase LayerDatabase => _layerDatabase;
    }
}
