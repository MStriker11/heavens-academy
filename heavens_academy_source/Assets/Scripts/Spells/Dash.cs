using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Movement
{
    public override void Activate()
    {
        Debug.Log("Using spell " + abilityInfo.abilityName);
    }
}
