using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour
{
    // make menuManager a singleton so we can access it anywhere
    public static menuManager Instance; // static instance bounds var to class and not unity object

    [SerializeField] menu[] menuList;

    private void Awake()
    {
        Instance = this; //static reference references this script
        // set all menus to inactive
        for (int i = 0; i < menuList.Length; i++)
        {
            menuList[i].gameObject.SetActive(false);
        }
    }

    // for access through scripts instead of direct reference in the inspector
    public void OpenMenu(string mName)
    {
        for (int i = 0; i < menuList.Length; i++)
        {
            // open menu of specified name in string menuName from menu.cs
            if (menuList[i].menuName == mName) //menuList[i].name compares the gameObject name (MainMenu, LoadingScreen) and not the specified string
            {
                menuList[i].Open();
                // OpenMenu(menuName);
            }
            else if (menuList[i].open)
            {
                CloseMenu(menuList[i]);
            }
        }
    }

    // easier to assign references when testing
    public void OpenMenu(menu menu)
    {
        for (int i = 0; i < menuList.Length; i++)
        {
            if (menuList[i].open)
            {
                CloseMenu(menuList[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(menu menu)
    {
        menu.Close();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void playCredits()
    {
        SceneManager.LoadScene(3);
    }

    public void AddPlayer()
    {

    }
}
