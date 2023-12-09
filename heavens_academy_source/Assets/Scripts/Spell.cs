using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : Ability
{
    public string keyBind;
    public GameObject spellPrefab;

    public abstract override void Activate();
}
