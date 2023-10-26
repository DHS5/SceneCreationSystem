using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public class SceneObjectTag
    {
        #region Constructor

        public SceneObjectTag(int _value)
        {
            value = _value;
        }

        #endregion

        [SerializeField] private int value;

        public List<string> Names => TagToNames(this);


        #region Operation

        public static implicit operator int(SceneObjectTag tag)
        {
            return tag.value;
        }

        public static implicit operator SceneObjectTag(int intVal)
        {
            return new(intVal);
        }

        public static SceneObjectTag operator +(SceneObjectTag tag1, SceneObjectTag tag2)
        {
            return tag1.Union(tag2);
        }
        public static SceneObjectTag operator -(SceneObjectTag tag1, SceneObjectTag tag2)
        {
            return tag1.Remove(tag2);
        }

        internal bool Include(int index)
        {
            return ((1 << index) & this) != 0;
        }
        internal bool Exclude(int index)
        {
            return ((1 << index) & this) == 0;
        }

        /// <returns>True if this contains any tag in common with <paramref name="other"/></returns>
        public bool ContainsAny(SceneObjectTag other) => Intersection(other) != 0;
        /// <returns>True if this lacks any tag that <paramref name="other"/> contains</returns>
        public bool LackAny(SceneObjectTag other) => other.Remove(this) != 0;

        public SceneObjectTag Intersection(SceneObjectTag other)
        {
            return this & other;
        }
        
        public SceneObjectTag Union(SceneObjectTag other)
        {
            return this | other;
        }

        public SceneObjectTag ExclusiveUnion(SceneObjectTag other)
        {
            return this ^ other;
        }

        public SceneObjectTag Inverse()
        {
            return ~value;
        }
        public SceneObjectTag Remove(SceneObjectTag other)
        {
            //value ^= Intersection(other); 2 working versions
            return value & (~other);
        }        

        #endregion

        #region Statics

        public static List<string> Tags => SceneObjectTagDatabase.Instance.Names;

        public static int NameToTag(string tagName) => SceneObjectTagDatabase.Instance.IndexOfName(tagName);

        public static SceneObjectTag NamesToTag(params string[] tagNames)
        {
            if (tagNames == null)
            {
                throw new ArgumentNullException("tagNames");
            }

            int num = 0;
            foreach (string tagName in tagNames)
            {
                int num2 = NameToTag(tagName);
                if (num2 != -1)
                {
                    num |= 1 << num2;
                }
            }

            return num;
        }

        public static List<string> TagToNames(SceneObjectTag tag) => SceneObjectTagDatabase.Instance.NamesOfTag(tag);

        #endregion
    }
}
