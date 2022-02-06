using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JudgeScript : MonoBehaviour
{
    public GameObject getBackPrefab;
    public GameObject butBackPrefab;

    public GameObject backButton;
    public GameObject forwardsButton;

    public CardsPile GetCardDeck;
    public CardsPile ButCardDeck;

    public CardsPile GetDisplay;
    public CardsPile ButDisplay;

    public GameObject ChooseWinner;

    int numSubmissions = 0;

    private Card[] getCards = new Card[8];
    private Card[] butCards = new Card[8];
    private bool[] seenCombo = new bool[8];

    public Card currentGet;
    public Card currentBut;

    int currentSetIndex = 0;

    // Start is called before the first frame update
    void Start()
    {

        CardsPile [] piles = FindObjectsOfType<CardsPile>(true);
        foreach (CardsPile pile in piles)
        {
            if (pile.gameObject.name == "GetDeck") pile.gameObject.SetActive(false);
            else if (pile.gameObject.name == "ButDeck") pile.gameObject.SetActive(false);
            else if (pile.gameObject.name == "GetHand") pile.gameObject.SetActive(false);
            else if (pile.gameObject.name == "ButHand") pile.gameObject.SetActive(false);
            else if (pile.gameObject.name == "GetDiscard") pile.gameObject.SetActive(false);
            else if (pile.gameObject.name == "ButDiscard") pile.gameObject.SetActive(false);
            else if (pile.gameObject.name == "GetCenter") pile.gameObject.SetActive(false);
            else if (pile.gameObject.name == "ButCenter") pile.gameObject.SetActive(false);
        }



        PlayerPrefs.SetInt("ToMainScene", 1);
      for(int i = 1; i < 9; i ++){
        int getID = PlayerPrefs.GetInt("ChosenGet" + i.ToString(), i);
        int butID = PlayerPrefs.GetInt("ChosenBut" + i.ToString(), i);

        if(getID == -1 || butID == -1){
          continue;
        }

        numSubmissions++;
        Debug.Log(numSubmissions.ToString());

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

      int judgeIndex = PlayerPrefs.GetInt("JudgeIndex", -1);
      //judgeIndex = 1;

      if(judgeIndex != 0){
        StartCoroutine(botJudging(-1));
        backButton.SetActive(false);
        forwardsButton.SetActive(false);
        ChooseWinner.SetActive(false);

        string[] getStrings = new string[numSubmissions];
        string[] butStrings = new string[numSubmissions];

        int i = 0;
        while(i < numSubmissions){
          Card getCard = getCards[i];
          Card butCard = butCards[i];

          if (!getCard){
                    i++;
            continue;
          }

          getStrings[i] = "G" + getCard.Id.ToString();
          butStrings[i] = "B" + butCard.Id.ToString();

          i++;
        }

        ReinBot bot = FindObjectOfType<ReinBot>();
        int winner = bot.chooseWinner(getStrings, butStrings);

        StartCoroutine(botJudging(winner));

        WinnerChosen();

      }
    }

    private IEnumerator botJudging(int idx){
      do{
        System.Random rnd = new System.Random();
        int timeToWait = rnd.Next(2,6);

        yield return new WaitForSeconds(2);
        MoveCardsForward();
        if (currentSetIndex == idx){
          break;
        }
      } while(currentSetIndex != 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void WinnerChosen() {
        PlayerPrefs.SetInt("RoundGetWinner", currentGet.Id);
        PlayerPrefs.SetInt("RoundButWinner", currentBut.Id);
        int judgeIndex = PlayerPrefs.GetInt("JudgeIndex", -1);
        if(currentSetIndex >= judgeIndex){
          currentSetIndex++;
        }
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
        if(currentSetIndex == PlayerPrefs.GetInt("JudgeIndex", -1)){
          currentSetIndex++;
        }
        if (currentSetIndex > (numSubmissions-1)) {
            currentSetIndex = 0;
        }
        CardSwitcheroo();
    }

    public void MoveCardsBackward()
    {
        currentSetIndex--;
        if(currentSetIndex == PlayerPrefs.GetInt("JudgeIndex", -1)){
          currentSetIndex--;
        }
        if (currentSetIndex < 0) {
            currentSetIndex = (numSubmissions-1);
        }
        CardSwitcheroo();
    }
}
