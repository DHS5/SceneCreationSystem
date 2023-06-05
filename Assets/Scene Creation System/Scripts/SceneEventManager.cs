using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Dhs5.SceneCreation
{
    public class SceneEventParam
    {
        public SceneEventParam(SceneVar _var, object _formerValue, SceneObject _sender)
        { 
            Var = new(_var);
            FormerValue = _formerValue;
            Sender = _sender;
        }

        public SceneVar Var { get; private set; }
        public object FormerValue { get; private set; }
        public SceneObject Sender { get; private set; }

        // Getters
        public int UID => Var.uniqueID;
        public SceneVarType Type => Var.type;
        public object Value => Var.Value;
    }

    public static class SceneEventManager
    {
        private static Dictionary<int, Action<SceneEventParam>> eventDico = new();


        public static void StartListening(int keyEvent, Action<SceneEventParam> listener)
        {
            if (eventDico.ContainsKey(keyEvent))
            {
                eventDico[keyEvent] += listener;
            }
            else
            {
                eventDico.Add(keyEvent, listener);
            }
        }

        public static void StopListening(int keyEvent, Action<SceneEventParam> listener)
        {
            if (eventDico.ContainsKey(keyEvent))
            {
                eventDico[keyEvent] -= listener;
            }
        }

        public static void TriggerEvent(int keyEvent, SceneEventParam param)
        {
            if (eventDico.TryGetValue(keyEvent, out Action<SceneEventParam> thisEvent))
            {
                thisEvent.Invoke(param);
            }
        }
    }
}
