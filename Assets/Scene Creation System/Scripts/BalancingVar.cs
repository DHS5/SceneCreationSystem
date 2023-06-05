using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [System.Serializable]
    public class BalancingVar
    {
        public BalancingVar(SceneVar var)
        {
            uniqueID = var.uniqueID;
            ID = var.ID;
            type = var.type;

            var.GetStaticValues(out boolValue, out intValue, out floatValue, out stringValue);

            hasMin = var.hasMin;
            hasMax = var.hasMax;
            minInt = var.minInt;
            maxInt = var.maxInt;
            minFloat = var.minFloat;
            maxFloat = var.maxFloat;

            isStatic = var.IsStatic;
            isRandom = var.IsRandom;
        }

        public bool overrideVar;
        //public SceneVar sceneVar;

        public int uniqueID = 0;

        public string ID;
        public SceneVarType type;

        public bool boolValue;
        public int intValue;
        public float floatValue;
        public string stringValue;

        public bool hasMin;
        public bool hasMax;
        public int minInt;
        public int maxInt;
        public float minFloat;
        public float maxFloat;

        [SerializeField] private bool isStatic = false;
        public bool IsStatic => isStatic;

        [SerializeField] private bool isRandom = false;
        public bool IsRandom => isRandom;

        [SerializeField] private float propertyHeight;
    }
}