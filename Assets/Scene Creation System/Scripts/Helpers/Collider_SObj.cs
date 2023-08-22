using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public class Collider_SObj : SceneObject
    {
        #region Variables
        [Header("Collider")]
        [SerializeField] new private Collider collider;
        [Tooltip("Mask deciding which layers to collide with")]
        public LayerMask layerMask;
        [Space(10f)]
        [Tooltip("Number of uses before deactivating the collider\n--> Set to -1 for infinite use\n--> Use Reload() to reset to the original number")]
        [SerializeField] private int useNumber;

        [Header("Collision")]
        public List<SceneEvent<Collision>> onCollisionEnter;
        [Space(15f)]
        public List<SceneEvent<Collision>> onCollisionStay;
        [Space(15f)]
        public List<SceneEvent<Collision>> onCollisionExit;

        [Header("Trigger")]
        public List<SceneEvent<Collider>> onTriggerEnter;
        [Space(15f)]
        public List<SceneEvent<Collider>> onTriggerStay;
        [Space(15f)]
        public List<SceneEvent<Collider>> onTriggerExit;
        #endregion

        #region SceneObject Extension
        protected override void UpdateSceneVariables()
        {
            base.UpdateSceneVariables();

            onCollisionEnter.SetUp(sceneVariablesSO);
            onCollisionStay.SetUp(sceneVariablesSO);
            onCollisionExit.SetUp(sceneVariablesSO);
            onTriggerEnter.SetUp(sceneVariablesSO);
            onTriggerStay.SetUp(sceneVariablesSO);
            onTriggerExit.SetUp(sceneVariablesSO);
        }
        protected override void RegisterElements()
        {
            base.RegisterElements();

            Register(nameof(onCollisionEnter), onCollisionEnter);
            Register(nameof(onCollisionStay), onCollisionStay);
            Register(nameof(onCollisionExit), onCollisionExit);
            Register(nameof(onTriggerEnter), onTriggerEnter);
            Register(nameof(onTriggerStay), onTriggerStay);
            Register(nameof(onTriggerExit), onTriggerExit);
        }
        protected override void Awake_Ext()
        {
            base.Awake_Ext();

            Reload();
        }
        #endregion

        #region Collision
        protected virtual bool CollisionValid(Collision collision)
        {
            return ((1 << collision.gameObject.layer) & layerMask) != 0;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (CollisionValid(collision))
            {
                onCollisionEnter.Trigger(collision);
                Use();
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            if (CollisionValid(collision))
            {
                onCollisionStay.Trigger(collision);
                Use();
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            if (CollisionValid(collision))
            { 
                onCollisionExit.Trigger(collision);
                Use();
            }
        }
        #endregion

        #region Trigger
        protected virtual bool TriggerValid(Collider collider)
        {
            return ((1 << collider.gameObject.layer) & layerMask) != 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (TriggerValid(other))
            { 
                onTriggerEnter.Trigger(other);
                Use();
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (TriggerValid(other))
            { 
                onTriggerStay.Trigger(other);
                Use();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (TriggerValid(other))
            { 
                onTriggerExit.Trigger(other);
                Use();
            }
        }
        #endregion

        #region Collider Management
        protected override void OnValidate_Ext()
        {
            base.OnValidate_Ext();

            if (collider == null)
            {
                collider = GetComponent<Collider>();
            }
            if (collider)
            {
                collider.includeLayers = layerMask;
            }
        }
        #endregion

        #region Use Management
        private int useLeft;

        private void Reload()
        {
            useLeft = useNumber;
            collider.enabled = useLeft != 0;
        }
        private void Use()
        {
            if (useLeft == -1) { return; }
            useLeft--;
            if (useLeft == 0) { collider.enabled = false; }
        }
        #endregion
    }
}
