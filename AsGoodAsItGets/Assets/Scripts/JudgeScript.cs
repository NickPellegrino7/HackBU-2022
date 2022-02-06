using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JudgeScript : MonoBehaviour
{
    public GameObject getBackPrefab;
    public GameObject butBackPrefab;

    public CardsPile GetCardDeck;
    public CardsPile ButCardDeck;

    public CardsPile GetDisplay;
    public CardsPile ButDisplay;

    public GameObject ChooseWinner;

    public int numSubmissions = 7;

    private Card[] getCards = new Card[7];
    private Card[] butCards = new Card[7];
    private bool[] seenCombo = new bool[7];

    public Card currentGet;
    public Card currentBut;

    int currentSetIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
      PlayerPrefs.SetInt("ToMainScene", 1);
      for(int i = 1; i < 9; i ++){
        int getID = PlayerPrefs.GetInt("ChosenGet" + i.ToString(), -1);
        int butID = PlayerPrefs.GetInt("ChosenBut" + i.ToString(), -1);

        if(getID == -1 || butID == -1){
          continue;
        }

        Card butCard = Instantiate(butBackPrefab).GetComponent<Card>();
        butCard.Initialize(butID);
        ButCardDeck.Add(butCard, false);

        Card getCard = Instantiate(getBackPrefab).GetComponent<Card>();
        getCard.Initialize(getID);
        GetCardDeck.Add(getCard, false);

        getCards[i - 1] = getCard;
        butCards[i - 1] = butCard;
      }
      CardSwitcheroo();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void WinnerChosen() {
        PlayerPrefs.SetInt("RoundGetWinner", currentGet.Id);
        PlayerPrefs.SetInt("RoundButWinner", currentBut.Id);
        PlayerPrefs.SetString("RoundWinnerName", "Player " + (currentSetIndex + 1).ToString());
        SceneManager.LoadScene("Scenes/RoundWinScreen.unity");
    }

    private void CardSwitcheroo(){
      if (currentGet) {
              GetDisplay.Remove(currentGet);
              currentGet.Flip();
              GetCardDeck.Add(currentGet, true);
          }
      if (currentBut) {
          ButDisplay.Remove(currentBut);
          currentBut.Flip();
          ButCardDeck.Add(currentBut, true);
      }
      currentGet = getCards[currentSetIndex];
      currentBut = butCards[currentSetIndex];
      seenCombo[currentSetIndex] = true;
      GetCardDeck.Remove(currentGet);
      ButCardDeck.Remove(currentBut);
      GetDisplay.Add(currentGet, true);
      ButDisplay.Add(currentBut, true);
      currentGet.Flip();
      currentBut.Flip();
      if (!ChooseWinner.GetComponent<Button>().interactable) {
          bool seenAllCombos = true;
          for (int i = 0; i < numSubmissions; i++) {
              if (!seenCombo[i]) {
                  seenAllCombos = false;
              }
          }
          if (seenAllCombos) {
              ChooseWinner.GetComponent<Button>().interactable = true;
          }
      }
    }

    public void MoveCardsForward()
    {
        currentSetIndex++;
        if (currentSetIndex > (numSubmissions-1)) {
            currentSetIndex = 0;
        }
        CardSwitcheroo();
    }

    public void MoveCardsBackward()
    {
        currentSetIndex--;
        if (currentSetIndex < 0) {
            currentSetIndex = (numSubmissions-1);
        }
        CardSwitcheroo();
    }
}
