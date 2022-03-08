using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Mirror;
using System.Linq;
using System.Collections.Generic;

namespace Networking
{
    public class LobbyController : MonoBehaviour
    {
        public static LobbyController Instance;

        //UI Elements
        public Text lobbyNameText;

        //Player Data
        public GameObject playerListViewContent;
        public GameObject playerListItemPrefab;
        public GameObject localPlayerObject;

        //Other Data
        public ulong currentLobbyId;
        public bool playerItemCreated = false;
        private List<PlayerListItem> playerListItem = new List<PlayerListItem>();
        public PlayerObjectController localPlayerController;
        private const string sceneName = "Game";

        //Ready
        public Button startGameButton;
        public Text readyButtonText;

        //Manager
        private CustomNetworkManager manager;

        private CustomNetworkManager Manager
        {
            get
            {
                if (manager != null)
                {
                    return manager;
                }

                return manager = CustomNetworkManager.singleton as CustomNetworkManager;
            }
        }

        private void Awake()
        {
            if (Instance == null) { Instance = this; }
        }

        public void ReadyPlayer()
        {
            localPlayerController.ChangeReady();
        }

        public void UpdateButton()
        {
            if (localPlayerController.ready)
            {
                readyButtonText.text = "Unready";
            }
            else
            {
                readyButtonText.text = "Ready";
            }
        }

        public void CheckIfAllReady()
        {
            bool allReady = false;

            foreach(PlayerObjectController player in Manager.gamePlayers)
            {
                if (player.ready)
                {
                    allReady = true;
                }
                else
                {
                    allReady = false;
                    break;
                }
            }

            if (allReady)
            {
                if (localPlayerController.playerIdNumber == 1)
                {
                    startGameButton.interactable = true;
                }
                else
                {
                    startGameButton.interactable = false;
                }
            }
            else
            {
                startGameButton.interactable = true;
            }
        }

        public void UpdateLobbyName()
        {
            currentLobbyId = Manager.GetComponent<SteamLobby>().currentLobbyId;
            lobbyNameText.text = SteamMatchmaking.GetLobbyData(
                new CSteamID(currentLobbyId),
                "name");
        }

        public void UpdatePlayerList()
        {
            if (!playerItemCreated)
            {
                CreateHostPlayerItem();
            }

            if (playerListItem.Count < Manager.gamePlayers.Count)
            {
                CreateClientPlayerItem();
            }

            if (playerListItem.Count > Manager.gamePlayers.Count)
            {
                RemovePlayerItem();
            }

            if (playerListItem.Count == Manager.gamePlayers.Count)
            {
                UpdatePlayerItem();
            }
        }

        public void FindLocalPlayer()
        {
            localPlayerObject = GameObject.Find("LocalGamePlayer");
            localPlayerController = localPlayerObject.GetComponent<PlayerObjectController>();
        }

        public void CreateHostPlayerItem()
        {
            foreach (PlayerObjectController player in Manager.gamePlayers)
            {
                GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
                PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();
                newPlayerItemScript.playerName = player.playerName;
                newPlayerItemScript.connectionId = player.connectionID;
                newPlayerItemScript.playerSteamId = player.playerSteamId;
                newPlayerItemScript.ready = player.ready;
                newPlayerItemScript.SetPlayerValues();

                newPlayerItem.transform.SetParent(playerListViewContent.transform);
                newPlayerItem.transform.localScale = Vector3.one;

                playerListItem.Add(newPlayerItemScript);
            }

            playerItemCreated = true;
        }

        public void CreateClientPlayerItem()
        {
            foreach (PlayerObjectController player in Manager.gamePlayers)
            {
                if (!playerListItem.Any(b => b.connectionId == player.connectionID))
                {
                    GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
                    PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();
                    newPlayerItemScript.playerName = player.playerName;
                    newPlayerItemScript.connectionId = player.connectionID;
                    newPlayerItemScript.playerSteamId = player.playerSteamId;
                    newPlayerItemScript.ready = player.ready;
                    newPlayerItemScript.SetPlayerValues();

                    newPlayerItem.transform.SetParent(playerListViewContent.transform);
                    newPlayerItem.transform.localScale = Vector3.one;

                    playerListItem.Add(newPlayerItemScript);
                }
            }
        }

        public void RemovePlayerItem()
        {
            List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();
            foreach(PlayerListItem item in playerListItem)
            {
                if (!Manager.gamePlayers.Any(b => b.connectionID == item.connectionId))
                {
                    playerListItemToRemove.Add(item);
                }
            }

            if(playerListItemToRemove.Count > 0)
            {
                foreach(PlayerListItem item in playerListItemToRemove)
                {
                    if(item == null) { continue; }

                    GameObject objectToRemove = item.gameObject;
                    playerListItem.Remove(item);
                    Destroy(objectToRemove);
                    objectToRemove = null;
                }
            }
        }

        public void UpdatePlayerItem()
        {
            foreach (PlayerObjectController player in Manager.gamePlayers)
            {
                foreach(PlayerListItem playerListItemScript in playerListItem)
                {
                    if(playerListItemScript.connectionId == player.connectionID)
                    {
                        playerListItemScript.playerName = player.playerName;
                        playerListItemScript.ready = player.ready;
                        playerListItemScript.SetPlayerValues();

                        if(player == localPlayerController)
                        {
                            UpdateButton();
                        }
                    }
                }
            }

            CheckIfAllReady();
        }

        public void StartGame()
        {
            localPlayerController.CanStartGame(sceneName);
        }
    }
}
