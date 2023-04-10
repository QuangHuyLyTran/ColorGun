using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Laucher : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
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
    public override void OnLeftRoom()
    {
        LobbyManager.Instance.OpenLobby("lobby");
    }
}
