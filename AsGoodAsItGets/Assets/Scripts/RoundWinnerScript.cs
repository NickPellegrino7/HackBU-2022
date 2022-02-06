using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class RoundWinnerScript : MonoBehaviour
{
    public GameObject WinnerText;
    public GameObject butBackPrefab;
    public GameObject getBackPrefab;

    public CardsPile WinningGetCardDeck;
    public CardsPile WinningButCardDeck;

    // Start is called before the first frame update
    void Start()
    {
      TextMeshProUGUI winnerText = WinnerText.GetComponent<TextMeshProUGUI>();
      string winner = PlayerPrefs.GetString("RoundWinner", "No One");
      winnerText.text = winner + " Wins This Round!";

      int getWinner = PlayerPrefs.GetInt("RoundGetWinner", 1);
      int butWinner = PlayerPrefs.GetInt("RoundButWinner", 1);

      Card butCard = Instantiate(butBackPrefab).GetComponent<Card>();
      butCard.Initialize(butWinner);
      WinningButCardDeck.Add(butCard, false);

      Card getCard = Instantiate(getBackPrefab).GetComponent<Card>();
      getCard.Initialize(getWinner);
      WinningGetCardDeck.Add(getCard, false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveToGameWinner()
    {
      SceneManager.LoadScene("Scenes/WinScreen.unity");
    }

    public void MoveToGameplay()
    {
      SceneManager.LoadScene("Scenes/Demo");
    }
}
