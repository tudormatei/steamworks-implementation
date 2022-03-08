using UnityEngine;
using Mirror;
using Core.Managers;

namespace Core.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        public GameObject playerModel;
        public GameObject playerUI;

        //Assignables
        private Rigidbody rb;
        public Transform orientation;

        //Movement
        public float moveSpeed = 4500;
        public float maxSpeed = 20;

        //Directions
        Vector2 moveInput = new Vector2();
        public float friction = 0.175f;
        public Vector3 shootDir = Vector3.zero;

        public bool canPlay = false;
        private RaycastHit hit;
        private Ray ray;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            playerUI.SetActive(false);
            playerModel.SetActive(false);
        }

        private void Update()
        {
            if(GameManager.Instance == null) { return; }
            if(!GameManager.Instance.canPlay) { return; }

            if (playerModel.activeSelf == false)
            {
                SpawnPosition();
                playerModel.SetActive(true);
                playerUI.SetActive(true);
            }

            if (hasAuthority)
            {
                PlayerInput();
            }
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance == null) { return; }
            if (!GameManager.Instance.canPlay) { return; }

            if (hasAuthority)
            {
                Movement();
            }
        }

        private void PlayerInput()
        {
            //WASD input
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();
        }

        public void SpawnPosition()
        {
            int index = 0;
            foreach(bool b in GameManager.Instance.spawnsTaken)
            {
                if (!b)
                {
                    break;
                }
                index++;
            }

            transform.position = GameManager.Instance.playerSpawnArr[index].transform.position;
        }

        public void Movement()
        {
            //Extra gravity
            rb.AddForce(Vector3.down * Time.deltaTime * 10);

            ApplyFriction();

            //Set max speed
            float maxSpeed = this.maxSpeed;

            //Some multipliers
            float multiplier = 1f, multiplierV = 1f;

            //Apply forces to move player
            rb.AddForce(orientation.right * moveInput.x * moveSpeed * Time.deltaTime * multiplier);
            rb.AddForce(orientation.forward * moveInput.y * moveSpeed * Time.deltaTime * multiplier * multiplierV);

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 rot = new Vector3(hit.point.x, 0f, hit.point.z);

                Vector3 targetPostition = new Vector3(rot.x,
                                        playerModel.transform.position.y,
                                        rot.z);
                playerModel.transform.LookAt(targetPostition);

                //Smoother rotation allthough not recommanded preformance wise
                /*Vector3 lookPos = rot - playerModel.transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, rotation, Time.deltaTime * 10f);*/
            }

            //Limit the player to the maxSpeed
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }

        private void ApplyFriction()
        {
            Vector3 inverseVelocity = -transform.InverseTransformDirection(rb.velocity);

            //Check if the player is giving input if not apply friciton
            if (moveInput.x == 0)
            {
                rb.AddForce(inverseVelocity.x * orientation.right * moveSpeed * friction * Time.deltaTime);
            }
            if (moveInput.y == 0)
            {
                rb.AddForce(inverseVelocity.z * orientation.forward * moveSpeed * friction * Time.deltaTime);
            }
        }
    }

}
