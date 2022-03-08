using System.Collections.Generic;
using UnityEngine;
using Core.Player;

namespace Core.Gameplay
{
    public class PowerUP : MonoBehaviour
    {
        private PowerUPSpawner spawnerInstance;

        public Material shotGun;
        public Material bounce;
        public Material bazooka;

        public List<Powers> powerUps = new List<Powers>();
        private Powers pow;

        private void Start()
        {
            CheckIfValidPosition();
            GeneratePowerUpStats();
        }

        private void GeneratePowerUpStats()
        {
            int index = Random.Range(0, powerUps.Count);
            pow = powerUps[index];

            if(index == 3)
            {
                gameObject.GetComponent<MeshRenderer>().material = shotGun;
            }
            else if(index == 1)
            {
                gameObject.GetComponent<MeshRenderer>().material = bounce;
            }
            else if(index == 0)
            {
                gameObject.GetComponent<MeshRenderer>().material = bazooka;
            }
        }

        private void CheckIfValidPosition()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
            if(cols.Length > 0)
            {
                Debug.Log("Destroyed gameobject");
                spawnerInstance.GeneratePowerUp();
                Destroy(gameObject);
            }
        }

        public void SetSpawnerInstance(PowerUPSpawner instance)
        {
            spawnerInstance = instance;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == "Player")
            {
                GiveStats(collision.gameObject);
                Debug.Log("Give stats");
                Destroy(gameObject);
            }
        }

        private void GiveStats(GameObject player)
        {
            WeaponStats ws = player.GetComponent<WeaponStats>();
            ws.ReceiveWeaponStats(pow);
        }
    }
}
