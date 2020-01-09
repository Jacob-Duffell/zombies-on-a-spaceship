using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    public GameObject startCanvas;
    public GameObject creditCanvas;
    public int firstLevel;

    public Button startGameButton;
    public Button exitGameButton;
    public Button creditsButton;
    public Button backButton;

	// Use this for initialization
	void Start ()
    {
        startGameButton.onClick.AddListener(StartClick);
        exitGameButton.onClick.AddListener(ExitClick);
        creditsButton.onClick.AddListener(CreditsClick);
        backButton.onClick.AddListener(BackClick);
    }

    /// <summary>
    /// Starts the main game
    /// </summary>
    void StartClick()
    {
        SceneManager.LoadScene(firstLevel, LoadSceneMode.Single);
    }

    /// <summary>
    /// Closes the application
    /// </summary>
    void ExitClick()
    {
        Application.Quit();
    }

    /// <summary>
    /// Displays the credits menu
    /// </summary>
    void CreditsClick()
    {
        startCanvas.SetActive(false);
        creditCanvas.SetActive(true);
    }

    /// <summary>
    /// Returns to the main menu
    /// </summary>
    void BackClick()
    {
        creditCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }
}
