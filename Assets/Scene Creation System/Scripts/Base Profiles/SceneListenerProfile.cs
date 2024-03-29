using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public class SceneListenerProfile : SceneProfile
    {
        public List<SceneListener> sceneListeners;

        #region Overrides
        public override void SetUp(SceneVariablesSO _sceneVariablesSO)
        {
            base.SetUp(_sceneVariablesSO);

            sceneListeners.SetUp(sceneVariablesSO);
        }
        public override void BelongTo(SceneObject _sceneObject)
        {
            base.BelongTo(_sceneObject);

            sceneListeners.BelongTo(sceneObject);
        }
        public override void Attach(SceneObject _sceneObject)
        {
            base.Attach(_sceneObject);

            sceneListeners.Subscribe();

            _sceneObject.OverrideListeners(this, sceneListeners);
        }
        public override void Detach()
        {
            base.Detach();

            sceneListeners.Unsubscribe();
        }

        public override bool CanOverrideListeners => true;
        public override bool CanOverrideEvents => false;
        #endregion

        #region Scene Events
        protected override void RegisterSceneEventsLists() { }
        protected override void RegisterTweens() { }
        #endregion
    }
}
