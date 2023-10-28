using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public abstract class FlagDatabase : ScriptableObject
    {
        [SerializeField] protected string tag0;
        [SerializeField] protected string tag1;
        [SerializeField] protected string tag2;
        [SerializeField] protected string tag3;
        [SerializeField] protected string tag4;
        [SerializeField] protected string tag5;
        [SerializeField] protected string tag6;
        [SerializeField] protected string tag7;
        [SerializeField] protected string tag8;
        [SerializeField] protected string tag9;
        [SerializeField] protected string tag10;
        [SerializeField] protected string tag11;
        [SerializeField] protected string tag12;
        [SerializeField] protected string tag13;
        [SerializeField] protected string tag14;
        [SerializeField] protected string tag15;
        [SerializeField] protected string tag16;
        [SerializeField] protected string tag17;
        [SerializeField] protected string tag18;
        [SerializeField] protected string tag19;
        [SerializeField] protected string tag20;
        [SerializeField] protected string tag21;
        [SerializeField] protected string tag22;
        [SerializeField] protected string tag23;
        [SerializeField] protected string tag24;
        [SerializeField] protected string tag25;
        [SerializeField] protected string tag26;
        [SerializeField] protected string tag27;
        [SerializeField] protected string tag28;
        [SerializeField] protected string tag29;
        [SerializeField] protected string tag30;
        [SerializeField] protected string tag31;

        #region Core Behaviour

        private void OnValidate()
        {
            Names = GetAllNames();
        }

        #endregion

        #region Helpers

        private string GetFlagAtIndex(int index)
        {
            if (index < 0 || index >= 32)
            {
                Debug.LogError("Flag index out of range");
                return null;
            }

            return index switch
            {
                0 => tag0,
                1 => tag1,
                2 => tag2,
                3 => tag3,
                4 => tag4,
                5 => tag5,
                6 => tag6,
                7 => tag7,
                8 => tag8,
                9 => tag9,
                10 => tag10,
                11 => tag11,
                12 => tag12,
                13 => tag13,
                14 => tag14,
                15 => tag15,
                16 => tag16,
                17 => tag17,
                18 => tag18,
                19 => tag19,
                20 => tag20,
                21 => tag21,
                22 => tag22,
                23 => tag23,
                24 => tag24,
                25 => tag25,
                26 => tag26,
                27 => tag27,
                28 => tag28,
                29 => tag29,
                30 => tag30,
                31 => tag31,
                _ => null
            };
        }
        
        private int GetIndexOfFlag(string flag)
        {
            if (string.IsNullOrWhiteSpace(flag))
            {
                Debug.LogError("Flag is null or white spaces");
                return -1;
            }

            for (int i = 0; i < 32; i++)
            {
                if (flag == GetFlagAtIndex(i)) return i;
            }

            return -1;
        }

        private bool IsValid(int index) => !string.IsNullOrWhiteSpace(GetFlagAtIndex(index));

        private List<string> GetAllNames()
        {
            int lastValid = -1;

            for (int i = 31; i >= 0; i--)
            {
                if (IsValid(i))
                {
                    lastValid = i;
                    break;
                }
            }

            if (lastValid == -1) return null;

            List<string> names = new();

            string n;

            for (int i = 0; i <= lastValid; i++)
            {
                n = i + ": ";
                if (!IsValid(i))
                {
                    n += "--- Empty ---";
                }
                else
                {
                    n += GetFlagAtIndex(i);
                }
                names.Add(n);
            }

            return names;
        }

        #endregion

        #region Public Accessors

        internal List<string> Names { get => _names; private set => _names = value; }
        [SerializeField, HideInInspector] private List<string> _names;

        internal string NameAtIndex(int index) => GetFlagAtIndex(index);
        internal List<string> NamesAtIndexes(ICollection<int> indexes)
        {
            List<string> names = new();
            foreach (int index in indexes)
            {
                names.Add(GetFlagAtIndex(index));
            }

            return names;
        }
        internal List<string> NamesOfTag(SceneObjectTag tag)
        {
            List<string> names = new();
            for (int i = 0; i < 32; i++)
            {
                if (tag.Include(i) && IsValid(i)) names.Add(GetFlagAtIndex(i));
            }

            return names;
        }

        internal int IndexOfName(string name) => GetIndexOfFlag(name);
        internal List<int> IndexesOfNames(ICollection<string> names)
        {
            List<int> indexes = new();
            foreach (string name in names)
            {
                indexes.Add(GetIndexOfFlag(name));
            }

            return indexes;
        }

        internal int CleanTag(SceneObjectTag tag)
        {
            int cleaner = 0;
            for (int i = 0; i < 32; i++)
            {
                if (tag.Include(i) && !IsValid(i)) cleaner |= 1 << i;
            }

            return tag & (~cleaner);
        }
        #endregion
    }
}
