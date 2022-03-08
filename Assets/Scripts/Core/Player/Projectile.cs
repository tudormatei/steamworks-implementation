using UnityEngine;
using Core.Gameplay;

namespace Core.Player
{
    public class Projectile : MonoBehaviour
    {
        private Rigidbody rb;
        private Collider shooterCollider;
        private Powers pow;
        private int currentNumberOfCollisions = 0;

        public void StartMovement(Vector3 dir, Collider col, Powers pow)
        {
            this.pow = pow;
            shooterCollider = col;

            rb = GetComponent<Rigidbody>();
            transform.LookAt(dir);
            rb.velocity = transform.forward * pow.bulletSpeed;

            Physics.IgnoreCollision(GetComponent<Collider>(), col);
            Destroy(gameObject, 25f);

            if (pow.bounceBullets)
            {
                SetBounciness();
            }
        }

        private void SetBounciness()
        {
            rb.angularDrag = 0f;
            rb.drag = 0f;
            GetComponent<SphereCollider>().material = pow.bouncyMat;
        }

        private void FixedUpdate()
        {
            CheckCollision();
        }

        private void CheckCollision()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, .5f))
            {
                if (hit.collider == shooterCollider) { return; }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Projectile")) { return; }

                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.DealDamage(pow.damage);

                    if (pow.bazooka) { Explode(pow.bazookaExplosionRadius); }

                    Destroy(gameObject);
                }

                if(pow.bounceBullets) { return; }
                if(pow.bazooka) { return; }
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (pow.bounceBullets && currentNumberOfCollisions < pow.maxNumberOfCollisionForBouncyBullets)
            {
                currentNumberOfCollisions++;
            }
            else
            {
                if (pow.bazooka)
                {
                    Explode(pow.bazookaExplosionRadius);
                }

                Destroy(gameObject);
            }
        }

        private void Explode(float radius)
        {
            //TODO: Check radius for player and deal damage

            GameObject go = Instantiate(pow.explosionPrefab, transform.position, Quaternion.identity);
            Destroy(go, 1f);
        }
    }
}
