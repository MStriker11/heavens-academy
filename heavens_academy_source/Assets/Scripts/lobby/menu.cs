using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// control which menu ui buttons to disable/enable
public class menu : MonoBehaviour
{
    public string menuName;
    public bool open;
    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }
}
