using UnityEngine;
using Core.Gameplay;

namespace Core.Player
{
    public class WeaponStats : MonoBehaviour
    {
        public Powers pow;

        private void Start()
        {
            //Create copy of scriptable object so it doesn't serialize on end of playmode
            pow = Instantiate(pow);
        }

        public void ReceiveWeaponStats(Powers pow)
        {
            if (pow.bazooka) { this.pow.bazooka = true; this.pow.shotgun = false; this.pow.laser = false; }
            if (pow.shotgun) { this.pow.shotgun = true; this.pow.bazooka = false; this.pow.laser = false; }
            if (pow.laser) { this.pow.laser = true; this.pow.bazooka = false; this.pow.shotgun = false; }
            if (pow.bounceBullets) { this.pow.bounceBullets = true; }

            this.pow.damage += pow.damage;
            this.pow.bulletSpeed += pow.bulletSpeed;
            this.pow.timeBetweenBullets += pow.timeBetweenBullets;
            this.pow.bazookaExplosionRadius += pow.bazookaExplosionRadius;
        }
    }
}
