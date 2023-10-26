using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [CreateAssetMenu(fileName = "SceneObject Settings", menuName = "Scene Creation/Settings/SceneObject Settings")]
    public class SceneObjectSettings : ScriptableObject
    {
        [SerializeField] private FlagDatabase _flagDatabase;
        internal FlagDatabase FlagDatabase => _flagDatabase;
    }
}
