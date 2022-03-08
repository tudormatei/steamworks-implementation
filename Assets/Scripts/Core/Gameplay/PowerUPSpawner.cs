using UnityEngine;

namespace Core.Gameplay
{
    public class PowerUPSpawner : MonoBehaviour
    {
        public int initialPowerUps = 10;

        public Vector2 xSpawnRange;
        public Vector2 ySpawnRange;

        [SerializeField] private GameObject powerUpPrefab;

        private void Start()
        {
            for (int i = 0; i < initialPowerUps; i++)
            {
                GeneratePowerUp();
            }

            InvokeRepeating("GeneratePowerUp", 5f, 5f);
        }

        public void GeneratePowerUp()
        {
            GameObject go = Instantiate(powerUpPrefab) as GameObject;
            go.GetComponent<PowerUP>().SetSpawnerInstance(this);
            go.transform.position = new Vector3(Random.Range(xSpawnRange.x, xSpawnRange.y), 0.5f, Random.Range(ySpawnRange.x, ySpawnRange.y));
        }
    }
}
