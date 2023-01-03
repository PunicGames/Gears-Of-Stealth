using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinSystem : MonoBehaviour
{
    public int totalCoinsInGame;
    private readonly int MAX_COINS = 3;

    // Display coin
    private GestorUIinGame UIDisplay;

    // Player Stats

    void Start()
    {
        totalCoinsInGame = 0;
        UIDisplay = FindObjectOfType<GestorUIinGame>();
    }

    public void AddCoin()
    {
        if (totalCoinsInGame < MAX_COINS)
            totalCoinsInGame++;
        UIDisplay.UpdateCoinNumber(totalCoinsInGame);
    }


}
