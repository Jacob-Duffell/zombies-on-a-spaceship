using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    public int nextLevel;

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")     // When player collides with archway, load the next level
        {
            SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
        }
    }
}
