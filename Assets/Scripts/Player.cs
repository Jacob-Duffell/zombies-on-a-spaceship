using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public float moveSpeed;
    public float cameraMoveSpeed;
    public float jumpPower = 100.0f;
    public float jumpTime;
    public float jumpTimeLeft;
    public bool playerAlive;
    public Camera camera;
    public GameObject bullet;
    public GameObject bulletCharger;

    float fireTimer;

    const float fireTime = 0.3f;

    private Animator playerAnim;

    private AudioSource playerGrunt;
    private AudioSource bgm;
    private AudioSource jetpack;

    private Rigidbody rigidbody;
    
	// Use this for initialization
	void Start ()
    {
        playerAnim = GetComponent<Animator>();

        playerGrunt = GetComponent<AudioSource>();
        bgm = Camera.main.GetComponent<AudioSource>();
        jetpack = bulletCharger.GetComponent<AudioSource>();
        bgm.Play();

        playerAnim.SetTrigger("playerIdle");
        rigidbody = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        fireTimer = 0f;
        jumpTimeLeft = jumpTime;
        playerAlive = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (playerAlive)
        {
            playerAnim.enabled = false;
            if (Input.GetKey(KeyCode.W))        // Set movement for player
            {
                transform.Translate(new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z) * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(new Vector3(-Camera.main.transform.forward.z, 0, Camera.main.transform.forward.x) * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(new Vector3(-Camera.main.transform.forward.x, 0, -Camera.main.transform.forward.z) * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(new Vector3(Camera.main.transform.forward.z, 0, -Camera.main.transform.forward.x) * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.Space))        // Set jump movement for player
            {
                if (jumpTimeLeft > 0)
                {
                    rigidbody.useGravity = false;
                    rigidbody.AddForce(Vector3.up * jumpPower * Time.deltaTime);
                    jumpTimeLeft -= Time.deltaTime;

                    if (!jetpack.isPlaying)
                    {
                        jetpack.Play();
                    }
                }
            }

            if (jumpTimeLeft <= 0 || !Input.GetKey(KeyCode.Space))      // Player falls to ground if jetpack fuel depleted
            {
                rigidbody.useGravity = true;
                jetpack.Stop();
            }


            float h = cameraMoveSpeed * Input.GetAxis("Mouse X");       // Set camera movement
            float v = cameraMoveSpeed * Input.GetAxis("Mouse Y");

            camera.transform.Rotate(-v, h, 0);

            float z = camera.transform.eulerAngles.z;
            camera.transform.Rotate(0, 0, -z);

            if (fireTimer > 0)                                          // Fire Weapon
            {
                fireTimer -= Time.deltaTime;
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    bullet = ObjectPooler.Instance.GetPooledObject("Bullet");
                    bullet.transform.position = bulletCharger.transform.position;
                    bullet.transform.rotation = bulletCharger.transform.rotation;
                    bullet.SetActive(true);
                    fireTimer = fireTime;
                }
            }
        }

        if (rigidbody.IsSleeping())         // Keep rigidbody awake so OnCollisionStay can be called
        {
            rigidbody.WakeUp();
        }

        if (currentHealth <= 0)     // Player is dead
        {
            if (!playerAnim.enabled)
            {
                transform.forward = Camera.main.transform.forward;
            }
            
            playerAnim.enabled = true;
            playerAnim.SetTrigger("playerDead");
            playerAlive = false;
            currentHealth = 0;
            bgm.Stop();
        }
    }   

    private void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "Ground")     // Reset jump time
        {
            if (jumpTimeLeft < jumpTime)
            {
                jumpTimeLeft += Time.deltaTime * 2;
            }
        }

        if (col.gameObject.tag == "Enemy" && currentHealth > 0)     // Player is hurt when colliding with enemy
        {
            currentHealth -= 1;

            if (!playerGrunt.isPlaying)
            {
                playerGrunt.Play();
            }
        }
    }
}
