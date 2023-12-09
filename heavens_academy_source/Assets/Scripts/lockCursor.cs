using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lockCursor : MonoBehaviour
{
    bool locked = true;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && locked)
        {
            Cursor.lockState = CursorLockMode.None;
            locked = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            locked = true;
        }
    }
}
