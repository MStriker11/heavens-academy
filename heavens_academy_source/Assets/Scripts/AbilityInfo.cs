using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "new ability", menuName = "new ability")]
public class AbilityInfo : ScriptableObject
{
    [Header("Spell Overview")]
    public string abilityName, spellDescription;

    // public Texture2D spellSprite;
    public enum Element { Fire, Water, Air, Earth }
    public Element element;

    [Header("Spell Stats")]
    public float cdTime;
    public float activeTime;

    public float currentCD = 0f, currentActiveCD = 0f;

    //public Image iconBG, icon, litIcon, silence;

    public virtual void Activate() { }

    public void silenceIcon()
    {
        spellManager.instance.silenceIcon(this);
    }

    public void unSilenceIcon()
    {
        spellManager.instance.unSilenceIcon(this);
    }

    public void PutOnCoolDown()
    {
        spellManager.instance.startCD(this);
    }

    public void PutOnActiveCoolDown()
    {
        spellManager.instance.startActiveCD(this);
    }

    public bool isSpellReady()
    {
        return currentCD <= 0;
    }

    public bool isSpellActive()
    {
        return currentActiveCD > 0;
    }

    public string GetCC()
    {
        return element.ToString();
    }
}
