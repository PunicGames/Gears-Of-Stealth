using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    private bool desktop = true;
   
    [SerializeField] private GameObject MobileControls;

    private void Awake()
    {
        if (Application.isMobilePlatform)
        {
            desktop = false;

        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            desktop = true;

        }

        //desktop = false;
    }

    public void ActivateControlsScreen()
    {
       
            MobileControls.SetActive(true);
        
    }

    public void DeactivateControlsScreen()
    {
       
            MobileControls.SetActive(false);
        
    }

}
