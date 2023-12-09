using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attach to basic ability holder (empty game object)
public abstract class Ability : MonoBehaviour
{
    public AbilityInfo abilityInfo;
    public abstract void Activate();
}
