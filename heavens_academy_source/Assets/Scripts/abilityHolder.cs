using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class abilityHolder : MonoBehaviour
{
    public AbilityInfo ability;
    private float cdTime;
    float activeTime;

    enum abilityState
    {
        ready,
        active,
        onCD
    }

    abilityState state = abilityState.ready;

    public KeyCode keyBinding;

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case abilityState.ready:
                if (Input.GetKeyDown(keyBinding))
                {
                    ability.Activate();
                    state = abilityState.active;
                    activeTime = ability.activeTime;
                }
            break;
            case abilityState.active:
                if (activeTime > 0)
                {
                    // run timer for ability duration
                    activeTime -= Time.deltaTime;
                }
                else
                {
                    state = abilityState.onCD;
                    cdTime = ability.cdTime;
                }
            break;
            case abilityState.onCD:
                if (cdTime > 0)
                {
                    // run timer for ability duration
                    cdTime -= Time.deltaTime;
                }
                else
                {
                    state = abilityState.ready;
                }
                break;
        }
    }
}
