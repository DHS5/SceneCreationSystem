using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public class SceneSpawner : SceneObject
    {
        [Header("Spawner")]
        [SerializeField] private List<SpawnTemplate> templates;

        #region Spawn Behaviour
        #region Exposed Versions
        /// <summary>
        /// Spawn the <see cref="SpawnTemplate"/> <paramref name="templateID"/> if it exists
        /// </summary>
        /// <param name="templateID">ID of the template to spawn</param>
        public void Spawn(string templateID)
        {
            Spawn(templateID, null);
        }
        /// <summary>
        /// Spawn the <see cref="SpawnTemplate"/> <paramref name="templateID"/> if it exists and removes it from <see cref="templates"/>
        /// </summary>
        /// <param name="templateID">ID of the template to spawn</param>
        public void SpawnAndRemove(string templateID)
        {
            SpawnAndRemove(templateID, null);
        }
        #endregion

        /// <summary>
        /// Spawn the <see cref="SpawnTemplate"/> <paramref name="templateID"/> if it exists <br></br>
        /// Override the parent Transform with <paramref name="parent"/>
        /// </summary>
        /// <param name="templateID">ID of the template to spawn</param>
        /// <param name="parent">Override parent Transform</param>
        /// <returns>The spawned <see cref="SceneObject"/></returns>
        public SceneObject Spawn(string templateID, Transform parent = null)
        {
            if (templates == null || templates.Count <= 0) return null;

            return templates.Find(t => t.ID == templateID)?.Spawn(sceneVariablesSO, parent);
        }
        /// <summary>
        /// Spawn the <see cref="SpawnTemplate"/> <paramref name="templateID"/> if it exists and removes it from <see cref="templates"/><br></br>
        /// Override the parent Transform with <paramref name="parent"/>
        /// </summary>
        /// <param name="templateID">ID of the template to spawn</param>
        /// <param name="parent">Override parent Transform</param>
        /// <returns>The spawned <see cref="SceneObject"/></returns>
        public SceneObject SpawnAndRemove(string templateID, Transform parent = null)
        {
            if (templates == null || templates.Count <= 0) return null;

            SpawnTemplate template = templates.Find(t => t.ID == templateID);

            if (template == null) return null;
            templates.Remove(template);

            return template.Spawn(sceneVariablesSO, parent);
        }
        #endregion

        #region Post Spawn Behaviour
        /// <summary>
        /// Apply the profiles of the <see cref="SpawnTemplate"/> <paramref name="templateID"/> on <paramref name="preSpawnedObject"/> if the template exists
        /// </summary>
        /// <param name="preSpawnedObject">SceneObject's GameObject that just got instantiated</param>
        /// <param name="templateID">ID of the template to post spawn</param>
        /// <returns>The post spawned <see cref="SceneObject"/></returns>
        public SceneObject PostSpawn(GameObject preSpawnedObject, string templateID)
        {
            if (templates == null || templates.Count <= 0) return null;

            return templates.Find(t => t.ID == templateID)?.PostSpawn(sceneVariablesSO, preSpawnedObject);
        }
        /// <summary>
        /// Apply the profiles of the <see cref="SpawnTemplate"/> <paramref name="templateID"/> on <paramref name="preSpawnedObject"/> and remove the template if it exists
        /// </summary>
        /// <param name="preSpawnedObject">SceneObject's GameObject that just got instantiated</param>
        /// <param name="templateID">ID of the template to post spawn</param>
        /// <returns>The post spawned <see cref="SceneObject"/></returns>
        public SceneObject PostSpawnAndRemove(GameObject preSpawnedObject, string templateID)
        {
            if (templates == null || templates.Count <= 0) return null;

            SpawnTemplate template = templates.Find(t => t.ID == templateID);

            if (template == null) return null;
            templates.Remove(template);

            return template.PostSpawn(sceneVariablesSO, preSpawnedObject);
        }
        #endregion

        #region Interfaces
        protected override void Init()
        {
            base.Init();

            templates.Init();
        }
        protected override void UpdateSceneVariables()
        {
            base.UpdateSceneVariables();

            templates.SetUp(sceneVariablesSO);
        }
        protected override void UpdateBelongings()
        {
            base.UpdateBelongings();

            templates.BelongTo(this);
        }
        #endregion

        [Serializable]
        public class SpawnTemplate : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable, SceneState.IInitializable
        {
            [Tooltip("ID of the template, must be unique in this list")]
            [SerializeField] private string templateID;
            [SerializeField] private GameObject prefab;
            [Tooltip("Instantiation parent of the SceneObject, can be overrided in code")]
            [SerializeField] private Transform parent;
            [Tooltip("Avoid adding several profile of the same type as only the first will be used")]
            [SerializeReference, SubclassPicker] private List<SceneProfile> profiles;

            public string ID => templateID;
            public SceneSpawner Spawner { get; private set; }

            #region Behaviour
            public SceneObject Spawn(SceneVariablesSO sceneVariablesSO, Transform overrideParent)
            {
                SceneObject sceneObject = Instantiate(prefab, overrideParent ?? parent).GetComponent<SceneObject>();
                sceneObject.name = templateID;
                sceneObject.ApplyProfiles(sceneVariablesSO, profiles);
                return sceneObject;
            }
            public SceneObject PostSpawn(SceneVariablesSO sceneVariablesSO, GameObject preSpawnedObject)
            {
                SceneObject sceneObject = preSpawnedObject.GetComponent<SceneObject>();
                sceneObject.name = templateID;
                sceneObject.ApplyProfiles(sceneVariablesSO, profiles);
                return sceneObject;
            }
            #endregion

            #region Interfaces
            public void Init()
            {
                profiles.Init();
            }
            public void SetUp(SceneVariablesSO sceneVariablesSO)
            {
                profiles.SetUp(sceneVariablesSO);
            }
            public void BelongTo(SceneObject _sceneObject)
            {
                Spawner = (SceneSpawner)_sceneObject;
            }
            #endregion
        }
    }
}
