using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class roomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    RoomInfo info;

    // set txt to room name and join room when button is clicked
    public void SetUp(RoomInfo _info)
    {
        info = _info;
        text.text = _info.Name;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
