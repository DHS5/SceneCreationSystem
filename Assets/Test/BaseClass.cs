using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseClass { }

[Serializable]
public class ChildClass1 : BaseClass
{
    public string childClass1String;
}

[Serializable]
public class ChildClass2 : BaseClass
{
    public int childClass2Int;
}

[Serializable]
public class ChildClass3 : BaseClass
{
    public float childClass3Float;
}
