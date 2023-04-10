using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    [SerializeField] Lobby[] lobbys;

    void Awake()
    {
        Instance = this;
    }
    public void OpenLobby(string lobbyName)
    {
        for(int i = 0; i < lobbys.Length; i++)
        {
            if(lobbys[i].lobbyName == lobbyName)
            {
                OpenLobby(lobbys[i]);
            }  
            else if(lobbys[i].open)
            {
                CloseLobby(lobbys[i]);
            }    
        }    
    }   
    public void OpenLobby(Lobby lobby)
    {
        for (int i = 0; i < lobbys.Length; i++)
        {            
            if (lobbys[i].open)
            {
                CloseLobby(lobbys[i]);
            }
        }
        lobby.Open();
    }
    public void CloseLobby(Lobby lobby)
    {
        lobby.Close();
    }
}
