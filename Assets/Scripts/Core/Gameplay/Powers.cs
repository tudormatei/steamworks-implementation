using UnityEngine;

namespace Core.Gameplay
{
    [CreateAssetMenu(fileName = "New Power Up", menuName = "Gameplay/PowerUp")]
    public class Powers : ScriptableObject
    {
        public PhysicMaterial bouncyMat;
        public GameObject projectilePrefab;
        public GameObject bazookaProjectilePrefab;
        public GameObject explosionPrefab;
        public GameObject laserProjectilePrefab;

        public bool shotgun = false;
        public bool bazooka = false;
        public bool bounceBullets = false;
        public bool laser = false;

        public float damage;
        public float bulletSpeed;
        public float timeBetweenBullets;
        public int maxNumberOfCollisionForBouncyBullets;
        public float bazookaExplosionRadius;
    }
}
