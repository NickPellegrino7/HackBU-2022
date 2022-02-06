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
    public GameObject nextRoundButton;
    public GameObject winGameButton;

    public CardsPile WinningGetCardDeck;
    public CardsPile WinningButCardDeck;

    // Start is called before the first frame update
    void Start()
    {
      TextMeshProUGUI winnerText = WinnerText.GetComponent<TextMeshProUGUI>();
      string winner = PlayerPrefs.GetString("RoundWinnerName", "No One");
      winnerText.text = winner + " Wins This Round!";

      int getWinner = PlayerPrefs.GetInt("RoundGetWinner", 1);
      int butWinner = PlayerPrefs.GetInt("RoundButWinner", 1);

      Card butCard = Instantiate(butBackPrefab).GetComponent<Card>();
      butCard.Initialize(butWinner);
      butCard.Flip();
      WinningButCardDeck.Add(butCard, false);

      Card getCard = Instantiate(getBackPrefab).GetComponent<Card>();
      getCard.Initialize(getWinner);
      getCard.Flip();
      WinningGetCardDeck.Add(getCard, false);

      int winnerIndex = PlayerPrefs.GetInt("RoundWinnerIndex", -1);
      int winnerPoints = PlayerPrefs.GetInt("Player" + winnerIndex.ToString() + "Points", 0);

      if(winnerPoints == 4){
        nextRoundButton.SetActive(false);
        winGameButton.SetActive(false);
      } else {
        PlayerPrefs.SetInt("Player" + winnerIndex.ToString() + "Points", winnerPoints + 1);
      }
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
      SceneManager.LoadScene("Scenes/Main");
    }
}
