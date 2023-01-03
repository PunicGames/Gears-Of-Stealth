using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private GameObject mobileUI;
    [SerializeField] private GameObject finUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            mobileUI.SetActive(false);
            finUI.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }
}
