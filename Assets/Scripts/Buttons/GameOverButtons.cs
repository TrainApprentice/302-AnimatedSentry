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
    private bool winScreenOut = false;
    private bool gameOverScreenOut = false;

    private AudioSource bgm;
    public AudioClip gameMusic, gameOverMusic, winMusic;

    private void Start()
    {
        bgm = GetComponent<AudioSource>();
        RestartGame();
    }
    private void Update()
    {
        if (currPlayer.isDead)
        {
            if (!winScreenOut)
            {
                if (timeToGameOver > 0) timeToGameOver -= Time.deltaTime;
                else
                {
                    if (!gameOverScreen.activeSelf)
                    {
                        bgm.Stop();
                        bgm.clip = gameOverMusic;
                        bgm.Play();
                        gameOverScreen.SetActive(true);
                        gameOverScreenOut = true;
                    }
                }

            }
        }
        if (sentryHealth.isDead)
        {
            if(!gameOverScreenOut)
            {
                if (timeToWinScreen > 0 && !gameOverScreenOut) timeToWinScreen -= Time.deltaTime;
                else
                {
                    if (!winScreen.activeSelf && !gameOverScreenOut)
                    {
                        bgm.Stop();
                        bgm.clip = winMusic;
                        bgm.Play();
                        winScreen.SetActive(true);
                        winScreenOut = true;
                    }
                }
            }
            
        }
    }

    public void RestartGame()
    {
        print("Restart");
        DetachJoint[] currJoints = FindObjectsOfType<DetachJoint>();
        if (currJoints.Length > 0)
        {
            foreach (DetachJoint j in currJoints)
            {
                Destroy(j.gameObject);
            }
        }

        if (currPlayer) Destroy(currPlayer.gameObject);
        if (currSentry) Destroy(currSentry.gameObject);
        GameObject newPlayer = Instantiate(playerBase, playerStart.position, Quaternion.Euler(0, 180, 0));
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
        winScreenOut = false;
        gameOverScreenOut = false;

        GetComponent<EnvironmentManager>().sentryBase = currSentry.GetComponent<SentryAttacks>();
        GetComponent<EnvironmentManager>().ResetWalls();

        bgm.Stop();
        bgm.clip = gameMusic;
        bgm.Play();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}

        
    

