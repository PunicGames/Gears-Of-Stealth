using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    static GameManager instance;
    [SerializeField] private PostProcessingControler pP_controller;

    private bool isMuted = false;

    private void Start()
    {
        if (instance != null) { 
            Destroy(gameObject);
        }
        else { 
            instance = this;
            DontDestroyOnLoad(gameObject);
            //OptionsInitilizer_DefaultValues();
        }

        //Sound control
        if (isMuted)
        {
            AudioListener.pause = true;
        }
        else
        {
            AudioListener.pause = false;
        }
    }

   
    public void OptionsInitilizer_DefaultValues()
    {
        PlayerPrefs.SetInt("bloomEffect", 1);
        PlayerPrefs.SetInt("colorGrading", 1);
        //PlayerPrefs.SetInt("antialiasing", 1);
        
        // Activate from default both post processing options
        pP_controller?.UpdateBloom(true);
        pP_controller?.UpdateColorGrading(true);

        
        //SetAntialiassing(PlayerPrefs.GetInt("antialiasing"));
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public bool SetSound()
    {
        if (isMuted)
        {
            AudioListener.pause = false;
            isMuted = false;
            return true;
        }
        else
        {
            AudioListener.pause = true;
            isMuted = true;
            return false;

        }


    }

}
