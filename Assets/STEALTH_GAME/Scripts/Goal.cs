using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    [SerializeField] private GameObject mobileUI;
    [SerializeField] private GameObject finUI;

    [SerializeField] private CoinSystem cS;
    [SerializeField] private GameObject[] coinImages;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            mobileUI.SetActive(false);
            finUI.SetActive(true);
            Time.timeScale = 0.0f;

            for (int i = 0; i < cS.totalCoinsInGame; i++) {
                coinImages[i].SetActive(true);
            }

        }
    }
}
