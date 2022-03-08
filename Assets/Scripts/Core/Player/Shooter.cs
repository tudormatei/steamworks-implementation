using UnityEngine;
using Mirror;
using Core.Managers;

namespace Core.Player
{
    public class Shooter : NetworkBehaviour
    {
        private WeaponStats weaponStats;
        private float timeSinceLastShot = Mathf.Infinity;

        private void Start()
        {
            weaponStats = GetComponent<WeaponStats>();
        }

        private void Update()
        {
            if (GameManager.Instance == null) { return; }
            if (!GameManager.Instance.canPlay) { return; }

            if (hasAuthority)
            {
                timeSinceLastShot += Time.deltaTime;
                if (Input.GetMouseButtonDown(0) && timeSinceLastShot >= weaponStats.pow.timeBetweenBullets)
                {
                    timeSinceLastShot = 0f;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        Shoot(hit);
                    }
                }
            }
        }

        private void Shoot(RaycastHit hit)
        {
            if (weaponStats.pow.shotgun) { Shotgun(hit); }
            else if (weaponStats.pow.bazooka) { Bazooka(hit); }
            else if(weaponStats.pow.laser) { Laser(hit); }
            else { DefaultBullet(hit); }
        }

        private void Laser(RaycastHit hit)
        {
            GameObject go = Instantiate(weaponStats.pow.laserProjectilePrefab, transform.position, Quaternion.identity) as GameObject;
            Vector3 target = new Vector3(hit.point.x, go.transform.position.y, hit.point.z);
            Vector3 dir = target - transform.position;
            RaycastHit nHit;
            if (Physics.Raycast(transform.position, dir, out nHit))
            {
                go.transform.position = nHit.point;
            }
        }

        private void Bazooka(RaycastHit hit)
        {
            GameObject go = Instantiate(weaponStats.pow.bazookaProjectilePrefab, transform.position, Quaternion.identity) as GameObject;
            Vector3 target = new Vector3(hit.point.x, go.transform.position.y, hit.point.z);
            /*Vector3 dir = target - transform.position;
            dir = transform.position + dir.normalized * 1.25f;
            go.transform.position = dir;*/
            Projectile projectile = go.GetComponent<Projectile>();
            Collider col = GetComponent<Collider>();

            projectile.StartMovement(target, col, weaponStats.pow);
        }

        private void Shotgun(RaycastHit hit)
        {
            GameObject go = Instantiate(weaponStats.pow.projectilePrefab, transform.position, Quaternion.identity) as GameObject;
            Vector3 target = new Vector3(hit.point.x, go.transform.position.y, hit.point.z);
            /*Vector3 dir = target - transform.position;
            dir = transform.position + dir.normalized * 1.25f;
            go.transform.position = dir;*/
            Projectile projectile = go.GetComponent<Projectile>();
            Collider col = GetComponent<Collider>();

            projectile.StartMovement(target, col, weaponStats.pow);

            go = Instantiate(weaponStats.pow.projectilePrefab, transform.position, Quaternion.identity) as GameObject;
            Vector3 newTarget = Quaternion.AngleAxis(25, go.transform.up) * target;
            /*dir = newTarget - transform.position;
            dir = transform.position + dir.normalized * 1.25f;
            go.transform.position = dir;*/
            projectile = go.GetComponent<Projectile>();
            col = GetComponent<Collider>();

            projectile.StartMovement(newTarget, col, weaponStats.pow);

            go = Instantiate(weaponStats.pow.projectilePrefab, transform.position, Quaternion.identity) as GameObject;
            newTarget = Quaternion.AngleAxis(-25, go.transform.up) * target;
            /*dir = newTarget - transform.position;
            dir = transform.position + dir.normalized * 1.25f;
            go.transform.position = dir;*/
            projectile = go.GetComponent<Projectile>();
            col = GetComponent<Collider>();

            projectile.StartMovement(newTarget, col, weaponStats.pow);
        }

        private void DefaultBullet(RaycastHit hit)
        {
            GameObject go = Instantiate(weaponStats.pow.projectilePrefab, transform.position, Quaternion.identity) as GameObject;
            Vector3 target = new Vector3(hit.point.x, go.transform.position.y, hit.point.z);
            /*Vector3 dir = target - transform.position;
            dir = transform.position + dir.normalized * 1.25f;
            go.transform.position = dir;*/
            Projectile projectile = go.GetComponent<Projectile>();
            Collider col = GetComponent<Collider>();

            projectile.StartMovement(target, col, weaponStats.pow);
        }
    }
}
