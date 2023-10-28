using Dhs5.SceneCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagTestScript : MonoBehaviour
{
    public SceneObjectTag soTag1;
    public SceneObjectTag soTag2;
    public SceneObjectTag soTag3;
    public SceneObjectTag soTag4;

    private void Start()
    {
        Debug.Log("soTag1 doesn't contains 'FirstTag' : " + !soTag1.HasTag("FirstTag"));
        Debug.Log("soTag1 contains 'First Tag' : " + soTag1.HasTag("First Tag"));
        Debug.Log("soTag1 contains any of soTag1 : " + soTag1.ContainsAny(soTag1));
        Debug.Log("soTag1 contains any of soTag2 : " + soTag1.ContainsAny(soTag2));
        Debug.Log("soTag1 doesn't contains any of soTag3 : " + !soTag1.ContainsAny(soTag3));
        Debug.Log("soTag2 contains any of soTag3 : " + soTag2.ContainsAny(soTag3));
        Debug.Log("soTag1 doesn't lack any of soTag1 : " + !soTag1.LacksAny(soTag1));
        Debug.Log("soTag1 lacks any of soTag2 : " + soTag1.LacksAny(soTag2));
        Debug.Log("soTag1 lacks any of soTag3 : " + soTag1.LacksAny(soTag3));
        Debug.Log("soTag2 doesn't lack any of soTag3 : " + !soTag2.LacksAny(soTag3));
        Debug.Log("soTag3 has tag by index 1 : " + soTag3.HasTagByIndex(1));
        Debug.Log("soTag3 hasn't tag by index 2 : " + !soTag3.HasTagByIndex(2));

        Debug.Log("soTag1 tags : " + soTag1.NamesAsString);
        Debug.Log("soTag2 tags : " + soTag2.NamesAsString);
        Debug.Log("soTag3 tags : " + soTag3.NamesAsString);
        Debug.Log("soTag1 + soTag2 : " + (soTag1 + soTag2).NamesAsString);
        Debug.Log("soTag1 + soTag3 : " + (soTag1 + soTag3).NamesAsString);
        Debug.Log("soTag2 + soTag3 : " + (soTag2 + soTag3).NamesAsString);
        Debug.Log("soTag1 - soTag2 : " + (soTag1 - soTag2).NamesAsString);
        Debug.Log("soTag2 - soTag1 : " + (soTag2 - soTag1).NamesAsString);
        Debug.Log("soTag1 - soTag3 : " + (soTag1 - soTag3).NamesAsString);
        Debug.Log("soTag2 - soTag3 : " + (soTag2 - soTag3).NamesAsString);

        Debug.Log("soTag4 doesn't contains 'Empty' : " + !soTag4.HasTag("Empty"));
        Debug.Log("soTag4 has tag by index 2 : " + soTag4.HasTagByIndex(2));
        soTag4.Clean();
        Debug.Log("After clean, soTag4 hasn't tag by index 2 : " + !soTag4.HasTagByIndex(2));

        Debug.Log("Names of new SceneObjectTag(-1) : " + (new SceneObjectTag(-1).NamesAsString));
    }
}
