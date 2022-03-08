using Mirror;
using Steamworks;

namespace Networking
{
    public class PlayerObjectController : NetworkBehaviour
    {
        //Player Data
        [SyncVar] public int connectionID;
        [SyncVar] public int playerIdNumber;
        [SyncVar] public ulong playerSteamId;
        [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;
        [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool ready;

        private CustomNetworkManager manager;

        private CustomNetworkManager Manager
        {
            get
            {
                if(manager != null)
                {
                    return manager;
                }

                return manager = CustomNetworkManager.singleton as CustomNetworkManager;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void PlayerReadyUpdate(bool oldValue, bool newValue)
        {
            if (isServer)
            {
                this.ready = newValue;
            }
            if (isClient)
            {
                LobbyController.Instance.UpdatePlayerList();
            }
        }

        [Command]
        private void CmdSetPlayerReady()
        {
            this.PlayerReadyUpdate(this.ready, !this.ready);
        }

        public void ChangeReady()
        {
            if (hasAuthority)
            {
                CmdSetPlayerReady();
            }
        }

        public override void OnStartAuthority()
        {
            CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
            gameObject.name = "LocalGamePlayer";
            LobbyController.Instance.FindLocalPlayer();
            LobbyController.Instance.UpdateLobbyName();
        }

        public override void OnStartClient()
        {
            Manager.gamePlayers.Add(this);
            LobbyController.Instance.UpdateLobbyName();
            LobbyController.Instance.UpdatePlayerList();
        }

        public override void OnStopClient()
        {
            Manager.gamePlayers.Remove(this);
            LobbyController.Instance.UpdatePlayerList();
        }

        [Command]
        private void CmdSetPlayerName(string playerName)
        {
            this.PlayerNameUpdate(this.playerName, playerName);
        }

        public void PlayerNameUpdate(string oldValue, string newValue)
        {
            if (isServer)
            {
                this.playerName = newValue;
            }

            if (isClient)
            {
                LobbyController.Instance.UpdatePlayerList();
            }
        }

        public void CanStartGame(string sceneName)
        {
            if (hasAuthority)
            {
                CmdCanStartGame(sceneName);
            }
        }

        [Command]
        public void CmdCanStartGame(string sceneName)
        {
            manager.StartGame(sceneName);
        }
    }
}
