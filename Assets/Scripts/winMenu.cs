using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class winMenu : MonoBehaviour
{
    public int mainMenu;
    public Button resetGameButton;

    // Use this for initialization
    void Start ()
    {
        Cursor.visible = true;
        resetGameButton.onClick.AddListener(ResetClick);
    }

    /// <summary>
    /// Returns the player to the main menu
    /// </summary>
    void ResetClick()
    {
        SceneManager.LoadScene(mainMenu, LoadSceneMode.Single);
    }
}
