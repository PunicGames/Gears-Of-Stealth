using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [SerializeField]
    GameManager gm;

    [SerializeField]
    GameObject muteSymbol;
    [SerializeField]
    GameObject soundSymbol;

   

    public void ActivateLevelSelector() { }
    public void PlayLevel1() { SceneManager.LoadScene("Level1"); }
    public void PlayLevel2() { SceneManager.LoadScene("Level2"); }
    public void PlayLevel3() { SceneManager.LoadScene("Level3"); }
    public void ActivateMainMenu() { }

    public void ActivateTutorialMenu() { }

    public void ActivateOptionsMenu() { }

    public void CloseApp() {
        Application.Quit();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetSound()
    {
        if (gm.SetSound())
        {
            muteSymbol.SetActive(false);
            soundSymbol.SetActive(true);
        }
        else
        {
            muteSymbol.SetActive(true);
            soundSymbol.SetActive(false);
        }



    }
}
