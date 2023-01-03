using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GestorUIinGame : MonoBehaviour
{

    public static GestorUIinGame guingame;

    [SerializeField]
    private GameObject mobileUI;
    [SerializeField]
    private GameObject pauseUI;
    [SerializeField]
    private GameObject shopUI;
    [SerializeField]
    private GameObject finPartida;

    private bool desktop = true;

    public bool shooping;

    // Cursor
    [SerializeField] private Texture2D cursorSprite;
    private Vector2 cursorHotSpot;

    // Shop display
    [SerializeField]
    private TextMeshProUGUI shopCoins;

    void Start()
    {
        // Cursor
        cursorHotSpot = new Vector2(0, 0);
    }

    public void ShowShop() { 
        shopUI.SetActive(true);
        Time.timeScale = 0.0f;
        shooping = true;


        mobileUI.SetActive(false);

        shopCoins.text = GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame.ToString();

        // Cursor
        //if (desktop)
        //    Cursor.SetCursor(cursorSprite, cursorHotSpot, CursorMode.ForceSoftware);
    }

    public void HideShop() { 
        shopUI.SetActive(false);
        Time.timeScale = 1.0f;
        shooping = false;


        mobileUI.SetActive(true);

        // Cursor
        //if(desktop)
        //    GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().ChangeCursorBack();
    }

    //public void FinishGame(int min, int secs, int bHit, int bMissed, int goldEarned, int defeatedEnemies) {
    //    Time.timeScale = 0.0f;
    //
    //
    //    mobileUI.SetActive(false);
    //
    //
    //    finPartida.SetActive(true);
    //
    //    // Display resume values
    //    if(secs < 10)
    //        totalTime.text = min + " : 0" + secs;
    //    else
    //        totalTime.text = min + " : " + secs;
    //
    //    if (bulletsHit)
    //    {
    //        bulletsHit.text = bHit.ToString();
    //        bulletsMissed.text = bMissed.ToString();
    //        if (bHit + bMissed == 0)
    //        {
    //            accuracy.text = "00.00%";
    //        }
    //        else
    //        {
    //            float acc = ((float)bHit / ((float)bHit + (float)bMissed)) * 100;
    //            accuracy.text = System.Math.Round(acc, 2) + "%";
    //        }
    //        goldEarnedT.text = goldEarned.ToString();
    //        defeatedEnemiesT.text = defeatedEnemies.ToString();
    //    }
    //}

    public void Pause() {
        mobileUI?.SetActive(false);
        pauseUI?.SetActive(true);
        Time.timeScale = 0.0f;
    }
    public void Unpause() { 
        Time.timeScale = 1.0f;
        pauseUI?.SetActive(false);
        mobileUI?.SetActive(true);
    }

    public void Retry() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        Time.timeScale = 1.0f;
    }

    public void GoBackMainMenu() {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }
}
