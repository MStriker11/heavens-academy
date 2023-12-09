using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silence : Spell
{
    public override void Activate()
    {
        Debug.Log("Using spell " + abilityInfo.abilityName);
    }
}
