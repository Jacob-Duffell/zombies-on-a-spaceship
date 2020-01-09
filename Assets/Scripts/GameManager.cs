using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public Text healthText;
    public Text fuelText;
    public Text deathMessage;

    private Player playerScript;

	// Use this for initialization
	void Start ()
    {
        Cursor.visible = false;
        playerScript = player.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        healthText.text = ("Health: " + playerScript.currentHealth);        // Display player health

        if (playerScript.jumpTimeLeft >= 0)     // Display fuel on HUD
        {
            fuelText.text = ("Fuel: " + Mathf.Round(playerScript.jumpTimeLeft * 10));
        }
        else
        {
            fuelText.text = ("Fuel: 0");
        }

        if (!playerScript.playerAlive)      // Display death message on HUD
        {
            deathMessage.text = "YOU ARE DEAD\n Press -SPACE- to restart";

            if (Input.GetKey(KeyCode.Space))        // Restart current level
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }
        }
	}
}
