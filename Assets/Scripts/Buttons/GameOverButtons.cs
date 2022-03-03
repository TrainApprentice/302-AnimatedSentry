using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverButtons : MonoBehaviour
{
    public GameObject playerBase, sentryBase;
    public Transform playerStart, sentryStart;
    public GameObject gameOverScreen, winScreen;
    public CameraController cam;

    private PlayerMovement currPlayer;
    private GameObject currSentry;
    private TargetableObject sentryHealth;

    private float timeToGameOver = 3f;
    private float timeToWinScreen = 3f;

    

    private void Start()
    {
        currSentry = FindObjectOfType<SentryAttacks>().gameObject;
        currPlayer = FindObjectOfType<PlayerMovement>();
        sentryHealth = currSentry.GetComponent<TargetableObject>();
    }
    private void Update()
    {
        if(currPlayer.isDead)
        {
            if (timeToGameOver > 0) timeToGameOver -= Time.deltaTime;
            else
            {
                if (!gameOverScreen.activeSelf) gameOverScreen.SetActive(true);
            }
        }

        if(sentryHealth.isDead)
        {
            if (timeToWinScreen > 0) timeToGameOver -= Time.deltaTime;
            else
            {
                if (!winScreen.activeSelf) winScreen.SetActive(true);
            }
        }
    }
    public void RestartGame()
    {
        DetachJoint[] currJoints = FindObjectsOfType<DetachJoint>();
        foreach (DetachJoint j in currJoints)
        {
            Destroy(j.gameObject);
        }
        Destroy(currPlayer.gameObject);
        Destroy(currSentry.gameObject);
        GameObject newPlayer = Instantiate(playerBase, playerStart.position, Quaternion.identity);
        GameObject newSentry = Instantiate(sentryBase, sentryStart.position, Quaternion.identity);

        currPlayer = newPlayer.GetComponent<PlayerMovement>();
        currSentry = newSentry;
        sentryHealth = currSentry.GetComponent<TargetableObject>();

        cam.player = currPlayer.gameObject.GetComponent<PlayerTargeting>();
        cam.playerController = currPlayer;

        winScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        timeToGameOver = 3f;
        timeToWinScreen = 3f;

        GetComponent<EnvironmentManager>().sentryBase = currSentry.GetComponent<SentryAttacks>();
        GetComponent<EnvironmentManager>().ResetWalls();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
