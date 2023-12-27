//using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private string playerName = "bob";
    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();
        
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Update is called once per frame
    void Update()
    {
        HeartBeat();
        LobbyPollUpdate();
    }

    private async void HeartBeat(){
        if(hostLobby != null){
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer <= 0f){
                float heartbeatTimerMax = 15f;//every 15 seconds
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }
    
    private async void CreateLobby(bool isPrivate){
        try{
            string lobbyName = "MyLobby";
            int maxPlayers = 5;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions{
                IsPrivate = isPrivate,
                Player = getPlayer(),
                };

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName,maxPlayers,createLobbyOptions);
        hostLobby = lobby;
        joinedLobby = hostLobby;
        Debug.Log("created lobby " + lobby.Name + " " + lobby.MaxPlayers); 
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void ListLobbies(){
        try{
            //add filter lobby here

            //--------------------------------

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies: " + queryResponse.Results.Count);
            foreach(Lobby lobby in queryResponse.Results){
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void JoinLobbyByID(int id){
        try{
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[id].Id);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }
    //better to join lobby by code 
    private async void JoinLobbyByCode(string lobbyCode){
        try{
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions{
                Player = getPlayer(),
            };
           joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode,joinLobbyByCodeOptions);

           Debug.Log("join lobby: " + lobbyCode);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private Player getPlayer(){
        return new Player{
                    Data = new Dictionary<string, PlayerDataObject>{
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,playerName)}
                    }
    };
    }
    private async void QuickJoin(){
        try{
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

     private void PrintPlayers(){
        PrintPlayers(joinedLobby);
     }

    private void PrintPlayers(Lobby lobby){
        foreach(Player player in lobby.Players){
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    private async void LobbyPollUpdate(){
        if(joinedLobby != null){
            lobbyUpdateTimer -= Time.deltaTime;
            if(lobbyUpdateTimer <= 0f){
                float lobbyUpdateTimerMax = 1.1f;//need more than 1 sec
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }

    private async void UpdatePlayerName(string newplayerName){
        try{
        playerName = newplayerName;
        await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions{
            Data = new Dictionary<string, PlayerDataObject>{
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,playerName)}
            }
        });
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void LeaveLobby(){
        try{
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void KickPlayer(){//change to by name & needs to update lobby
         try{
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id,joinedLobby.Players[1].Id);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void ChangeHost(){//make able to select which person
        try{
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id,new UpdateLobbyOptions{
                HostId = joinedLobby.Players[1].Id
            });
            joinedLobby = hostLobby;

        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async void DeleteLobby(){
        try{
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }
}
