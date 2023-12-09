using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System;
using Photon.Realtime;
using System.Linq;

// connect to Photon services/master server
public class Launcher : MonoBehaviourPunCallbacks
{
    //make Launcher a singleton
    public static Launcher Instance;

    [SerializeField] TMP_InputField lobbyNameInputField;
    [SerializeField] TMP_Text lobbyNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    //[SerializeField] TMP_Text errorText;

    private int playerNum = 1, maxPlayer = 4;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
        menuManager.Instance.OpenMenu("loading");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        // needed to create rooms
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        menuManager.Instance.OpenMenu("title");
        // menuManager.Instance.OpenMenu("MainMenu");
        //TODO: check if lobby has 4 players
    }

    // player lobby, not PUN lobby
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(lobbyNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(lobbyNameInputField.text);
        //CreateRoom("") generates a random str; use this if you want to hide your room name
        // PhotonNetwork takes a while to connect and create/join room; enable loading screen while this is happening
        menuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        menuManager.Instance.OpenMenu("room");
        lobbyNameText.text = PhotonNetwork.CurrentRoom.Name; //gives name of room

        // add a player when they join the lobby
        Player[] players = PhotonNetwork.PlayerList;

        // clear all players in playerListHolder
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // photon has built in host migration - if host leaves host is assigned to another player
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // error msg
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //errorText.text = "Error creating lobby: " + message;
        //menuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(2);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        menuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        menuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        menuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // clear list every time we update
        foreach  (Transform trans in roomListContent)
        {
            if (trans.gameObject != null)
            {
                Destroy(trans.gameObject);
            }   
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                continue;
            }
            // spawn a button when a new room is created
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<roomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // add new player when another player enters lobby
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
