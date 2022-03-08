using UnityEngine;
using UnityEngine.UI;
using Core.Player;
using Networking;

namespace Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        public GameObject[] playerSpawnArr;
        public bool[] spawnsTaken;

        public static GameManager Instance;
        public bool canPlay = false;

        [SerializeField] private GameObject gameFinishUI;

        private void Start()
        {
            if(Instance == null) { Instance = this; }
            gameFinishUI.SetActive(false);
            InitializeSpawn();

            EnablePlayerGameplay();
        }

        private void InitializeSpawn()
        {
            spawnsTaken = new bool[playerSpawnArr.Length];
            for(int i = 0;i < spawnsTaken.Length; i++)
                spawnsTaken[i] = false;
        }

        private void EnablePlayerGameplay()
        {
            canPlay = true;
        }

        public void FinishedGame()
        {
            canPlay = false;

            PlayerHealth[] arr = GameObject.FindObjectsOfType<PlayerHealth>();
            foreach(PlayerHealth p in arr)
            {
                if (!p.dead)
                {
                    string winnerName = p.GetComponent<PlayerObjectController>().playerName;
                    DisplayWinscreen(winnerName);
                    break;
                }
            }
        }

        private void DisplayWinscreen(string winner)
        {
            gameFinishUI.SetActive(false);
            Text winText = gameFinishUI.GetComponentInChildren<Text>();
            winText.text = "The winner is: " + winner.ToUpper();
            Debug.Log("The winner is: " + winner);
        }
    }
}
