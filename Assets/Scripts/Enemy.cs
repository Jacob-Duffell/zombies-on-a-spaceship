using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public bool playerSpotted;
    public bool alive;
    public GameObject blood;

    Animator enemyAnim;

    private AudioSource enemyNoise;
    private float makeEnemyNoiseTime;

	// Use this for initialization
	void Start ()
    {
        makeEnemyNoiseTime = 1;
        enemyNoise = GetComponent<AudioSource>();
        alive = true;
        playerSpotted = false;
        enemyAnim = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (health <= 0 && alive)       // Enemy is killed
        {
            enemyAnim.SetTrigger("zombieKilled");
            enemyNoise.Play();
            blood.SetActive(true);
            alive = false;
        }

        if (playerSpotted && alive)     // Enemy chases player
        {
            if (!enemyNoise.isPlaying)
            {
                makeEnemyNoiseTime -= Time.deltaTime;

                if (makeEnemyNoiseTime <= 0)
                {
                    enemyNoise.Play();

                    makeEnemyNoiseTime = 1;
                }
                
            }
        }
	}

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Bullet")     // Enemy is hit by bullet
        {
            health -= 1;
            col.gameObject.SetActive(false);
        }
    }
}
