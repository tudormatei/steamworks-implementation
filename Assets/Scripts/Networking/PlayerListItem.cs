using UnityEngine;
using UnityEngine.UI;
using Steamworks;

namespace Networking
{
    public class PlayerListItem : MonoBehaviour
    {
        public string playerName;
        public int connectionId;
        public ulong playerSteamId;
        private bool avatarReceived;

        public Text playerNameText;
        public RawImage playerIcon;
        public Text playerReadyText;
        public bool ready;

        protected Callback<AvatarImageLoaded_t> imageLoaded;

        private void Start()
        {
            imageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
        }

        public void ChangeReadyStatus()
        {
            if (ready)
            {
                playerReadyText.text = "Ready";
                playerReadyText.color = Color.green;
            }
            else
            {
                playerReadyText.text = "Unready";
                playerReadyText.color = Color.red;
            }
        }

        public void SetPlayerValues()
        {
            playerNameText.text = playerName;
            ChangeReadyStatus();
            if (!avatarReceived) { GetPlayerIcon(); }
        }

        private void GetPlayerIcon()
        {
            int imageId = SteamFriends.GetLargeFriendAvatar((CSteamID)playerSteamId);
            if (imageId == -1) { return; }
            playerIcon.texture = GetSteamImageAsTexture(imageId);
        }

        private void OnImageLoaded(AvatarImageLoaded_t callback)
        {
            if (callback.m_steamID.m_SteamID == playerSteamId)
            {
                playerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
            }
            else
            {
                return;
            }
        }

        private Texture2D GetSteamImageAsTexture(int iImage)
        {
            Texture2D texture = null;

            bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
            if (isValid)
            {
                byte[] image = new byte[width * height * 4];

                isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

                if (isValid)
                {
                    texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                    texture.LoadRawTextureData(image);
                    texture.Apply();
                }
            }
            avatarReceived = true;
            return texture;
        }
    }
}
