using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;

namespace Networking
{
    public class SteamLobby : MonoBehaviour
    {
        public static SteamLobby Instance;

        //Callbacks
        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> joinRequest;
        protected Callback<LobbyEnter_t> lobbyEntered;

        //Variables
        public ulong currentLobbyId;
        private const string hostAdressKey = "HostAddress";
        private CustomNetworkManager manager;

        private void Start()
        {
            if(!SteamManager.Initialized) { return; }
            if(Instance == null) { Instance = this; }

            manager = GetComponent<CustomNetworkManager>();

            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        public void HostLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if(callback.m_eResult != EResult.k_EResultOK) { return; }

            Debug.Log("Lobby created successfully!");

            manager.StartHost();

            SteamMatchmaking.SetLobbyData(new CSteamID(
                callback.m_ulSteamIDLobby),
                hostAdressKey,
                SteamUser.GetSteamID().ToString());

            SteamMatchmaking.SetLobbyData(new CSteamID(
                callback.m_ulSteamIDLobby),
                "name",
                SteamFriends.GetPersonaName().ToString() + "'s lobby");
        }

        private void OnJoinRequest(GameLobbyJoinRequested_t callback)
        {
            Debug.Log("Request to join lobby!");

            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            //Everyone
            currentLobbyId = callback.m_ulSteamIDLobby;

            //Clients
            if(NetworkServer.active) { return; }

            manager.networkAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                hostAdressKey);
            manager.StartClient();
        }
    }
}
