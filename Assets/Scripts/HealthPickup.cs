using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPickup : MonoBehaviour
{
    public int addHealthOnPickup;
    public GameObject player;
    public GameObject cross;
    public GameObject healthText;
    public ParticleSystem healthParticles;

    Player playerScript;
    float showText = 3;

    private AudioSource itemCollected;

    private void Start()
    {
        itemCollected = GetComponent<AudioSource>();
        playerScript = player.GetComponent<Player>();
    }

    private void Update()
    {
        if (healthText.activeSelf)      // If the player has collected the pickup, display text for a few seconds
        {
            showText -= Time.deltaTime;
            GetComponent<Collider>().enabled = false;

            if (showText <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")     // Increases the player's health when the player collides with it
        {
            itemCollected.Play();

            if (playerScript.currentHealth < playerScript.maxHealth - 10)
            {
                playerScript.currentHealth += addHealthOnPickup;
            }
            else
            {
                playerScript.currentHealth = playerScript.maxHealth;
            }

            cross.SetActive(false);
            healthParticles.Stop();
            healthText.SetActive(true);
        }
    }
}
