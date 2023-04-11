using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Laucher : MonoBehaviourPunCallbacks
{
    public static Laucher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListPrefab;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        LobbyManager.Instance.OpenLobby("lobby");
        Debug.Log("Joined Lobby");
    }
    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        LobbyManager.Instance.OpenLobby("loading");
    }    

    public override void OnJoinedRoom()
    {
        LobbyManager.Instance.OpenLobby("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Create Room Fail: " + message;
        LobbyManager.Instance.OpenLobby("error");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        LobbyManager.Instance.OpenLobby("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        LobbyManager.Instance.OpenLobby("");
    }    
    public override void OnLeftRoom()
    {
        LobbyManager.Instance.OpenLobby("lobby");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            Instantiate(roomListPrefab, roomListContent).GetComponent<RoomItem>().SetUp(roomList[i]);
        }    
    }
}
