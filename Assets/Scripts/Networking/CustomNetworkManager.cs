using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Steamworks;

namespace Networking
{
    public class CustomNetworkManager : NetworkManager
    {
        [SerializeField] private PlayerObjectController gamePlayerPrefab;
        public List<PlayerObjectController> gamePlayers { get; } = new List<PlayerObjectController>();

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if(SceneManager.GetActiveScene().name == "Lobby")
            {
                PlayerObjectController gamePlayer = Instantiate(gamePlayerPrefab);
                gamePlayer.connectionID = conn.connectionId;
                gamePlayer.playerIdNumber = gamePlayers.Count + 1;
                gamePlayer.playerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.currentLobbyId, gamePlayers.Count);

                NetworkServer.AddPlayerForConnection(conn, gamePlayer.gameObject);
            }
        }

        public void StartGame(string sceneName)
        {
            ServerChangeScene(sceneName);
        }
    }
}
