using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public class SceneListenerProfile : SceneProfile
    {
        [Header("Listeners")]
        public List<SceneListener> sceneListeners;

        #region Overrides
        public override void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            base.SetUp(_sceneVariablesSO);

            sceneListeners.SetUp(sceneVariablesSO);
        }
        public override void Attach(SceneObject _sceneObject)
        {
            base.Attach(_sceneObject);

            sceneListeners.BelongTo(sceneObject);
            sceneListeners.Register();
        }
        public override void Detach()
        {
            base.Detach();

            sceneListeners.Unregister();
            sceneListeners.BelongTo(null);
        }
        #endregion

        #region Scene Events
        public override void RegisterSceneEventsLists() { }
        #endregion
    }
}
