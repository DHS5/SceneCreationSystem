using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Dhs5.SceneCreation
{
    public static class SceneLogger
    {
        private const string alinea = "_-_";
        private const string back = "\n";

        private static Stack<string> logStack = new();

        public static string GetSceneLog(GameObject go)
        {
            StringBuilder sb = new StringBuilder();

            GameObject[] roots = go.scene.GetRootGameObjects();

            foreach (GameObject root in roots)
            {
                Appends("ROOT:", root.name);
                if (root.TryGetComponent(out SceneObject so))
                {
                    AppendSO(so);
                }
                AppendGO(root, 1); // Not good, need to be on childs
            }

            return UnpackStack(sb);
        }

        private static string UnpackStack(StringBuilder sb)
        {
            foreach (var s in logStack)
            {
                sb.Append(s);
            }
            return sb.ToString();
        }

        private static void Append(string str)
        {
            logStack.Push(str);
        }
        private static void Appends(params string[] str)
        {
            for (int i = str.Length - 1; i >= 0; i--)
            {
                Append(str[i]);
            }
        }
        private static void AppendSO(SceneObject so)
        {
            Appends("SO:", so.ToString());
        }

        private static void Alinea()
        {
            Append("_-_");
        }
        private static void Back()
        {
            Append("\n");
        }

        private static bool AppendGO(GameObject go, int rank)
        {
            void Name()
            {
                Append(go.name);
            }
            void Alineas()
            {
                for (int i = 0; i < rank; i++)
                {
                    Alinea();
                }
            }

            bool result = go.TryGetComponent(out SceneObject so);
            bool childResult = false;

            int childCount = go.transform.childCount;
            if (childCount == 0)
            {
                Back();
                if (result) AppendSO(so);
                Name();
                Alineas();
                return result;
            }
            else
            {
                rank++;
                for (int i = 0; i < childCount; i++)
                {
                    if (AppendGO(go.transform.GetChild(i).gameObject, rank))
                    {
                        childResult = true;
                    }
                }
            }

            if (result)
            {
                AppendSO(so);
            }
            if (result || childResult)
            {
                Name();
                Alineas();
                return true;
            }
            return false;
        }
    }
}
