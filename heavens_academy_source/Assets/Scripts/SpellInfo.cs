using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "new spell")]
public class SpellInfo : AbilityInfo
{
    // public GameObject spellPrefab;

    [Header("Spell Stats")]
    public float damage;
    //[Header("cc Type")]
    //public enum ccType { None, Silence, Freeze, Stun }
    //public ccType cc;

    //public string GetCC()
    //{
    //    return cc.ToString();
    //}
}
