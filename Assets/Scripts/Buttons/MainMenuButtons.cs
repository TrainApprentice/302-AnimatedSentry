using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void GoToGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
    public void GoToCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
