using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

[Serializable]
public struct spellIconList
{
    public AbilityInfo spell;
    public Image[] imgList;
}

//TODO: MAKE SERVER CONTROL CDS, NOT CLIENTS
public class spellManager : MonoBehaviourPunCallbacks
{
    // make singleton
    public static spellManager instance;

    public List<AbilityInfo> spellsOnCD = new List<AbilityInfo> ();
    public List<AbilityInfo> activeSpellCD = new List<AbilityInfo>();
    // order: bg, icon, litIcon, silence
    public spellIconList[] icons;
    Dictionary<AbilityInfo, Image[]> iconList = new Dictionary<AbilityInfo, Image[]>();

    //[Header("Spell 1")]
    //public Image iconBG1, icon1, litIcon1, silence1;

    //[Header("Spell 2")]
    //public Image iconBG2, icon2, litIco2n, silence2;

    private void Awake()
    {
        // singleton stuff
        if (instance == null)
        {
            instance = this;
        }
        else if ( instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);

        // insert array into dictionary
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].imgList[3].enabled = false;
            iconList.Add(icons[i].spell, icons[i].imgList);
            // DontDestroyOnLoad(icons[i].imgList);
        }

        for (int i = 0; i < icons.Length; i++)
        {
            DontDestroyOnLoad(icons[i].spell);
            for (int j = 0; j < 4; j++)
            {
                DontDestroyOnLoad(icons[i].imgList[j]);
            }
        }
    }

    private void Update()
    {
        onCD();
        onActiveCD();
    }

    #region icons
    void putIconOnCD(AbilityInfo spell)
    {
        //iconList[spell][0].enabled = true;
        //iconList[spell][1].enabled = true;
        if (iconList[spell][2] != null)
        {
            iconList[spell][2].enabled = false;
        }
    }

    void putIconOffCD(AbilityInfo spell)
    {
        //iconList[spell][0].enabled = false;
        //iconList[spell][1].enabled = false;
        if (iconList[spell][2] != null)
        {
            iconList[spell][2].enabled = true;
        }
    }

    public void silenceIcon(AbilityInfo spell)
    {
        if (iconList[spell][3] != null)
        {
            iconList[spell][3].enabled = true;
        }
    }

    public void unSilenceIcon(AbilityInfo spell)
    {
        if (iconList[spell][3] != null)
        {
            iconList[spell][3].enabled = false;
        }
    }

    #endregion icons

    #region cooldowns
    public void startCD(AbilityInfo spell)
    {
        if (!spellsOnCD.Contains(spell))
        {
            spell.currentCD = spell.cdTime;
            spellsOnCD.Add(spell); // add to list
            putIconOnCD(spell);
        }
    }

    public void startActiveCD(AbilityInfo spell)
    {
        if (!activeSpellCD.Contains(spell))
        {
            spell.currentActiveCD = spell.activeTime;
            activeSpellCD.Add(spell);
        }
    }

    void onCD()
    {
        for (int i = spellsOnCD.Count - 1; i >= 0; i--)
        {
            var currSpell = spellsOnCD[i];
            currSpell.currentCD -= Time.deltaTime;
            iconList[currSpell][0].fillAmount = (currSpell.cdTime - currSpell.currentCD) / currSpell.cdTime;
            iconList[currSpell][1].fillAmount = (currSpell.cdTime - currSpell.currentCD) / currSpell.cdTime;

            if (currSpell.currentCD <= 0)
            {
                currSpell.currentCD = 0;
                spellsOnCD.Remove(currSpell);
                putIconOffCD(currSpell);
            }

        }
    }

    void onActiveCD()
    {
        for (int i = activeSpellCD.Count - 1; i >= 0; i--)
        {
            activeSpellCD[i].currentActiveCD -= Time.deltaTime;

            if (activeSpellCD[i].currentActiveCD <= 0)
            {
                activeSpellCD[i].currentActiveCD = 0;
                activeSpellCD.Remove(activeSpellCD[i]);
            }
        }
    }

    #endregion cooldowns

}
