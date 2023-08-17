using Codice.Client.BaseCommands;
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

        #region Behaviour
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

            return templates.Find(t => t.ID == templateID)?.Spawn(parent);
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

            return template.Spawn(parent);
        }
        #endregion

        #region Interfaces
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
        public class SpawnTemplate : SceneState.ISceneVarSetupable, SceneState.ISceneObjectBelongable
        {
            [Tooltip("ID of the template, must be unique in this list")]
            [SerializeField] private string templateID;
            [SerializeField] private GameObject prefab;
            [Tooltip("Instantiation parent of the SceneObject, can be overrided in code")]
            [SerializeField] private Transform parent;
            [SerializeReference, SubclassPicker] private List<SceneProfile> profiles;

            public string ID => templateID;
            public SceneSpawner Spawner { get; private set; }

            #region Behaviour
            public SceneObject Spawn(Transform overrideParent)
            {
                SceneObject sceneObject = Instantiate(prefab, overrideParent ?? parent).GetComponent<SceneObject>();
                sceneObject.name = templateID;
                sceneObject.ApplyProfiles(profiles);
                return sceneObject;
            }
            #endregion

            #region Interfaces
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
