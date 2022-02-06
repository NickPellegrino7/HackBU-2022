using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeScript : MonoBehaviour
{
    public GameObject butBackPrefab;
    public GameObject getBackPrefab;

    public CardsPile GetCardDeck;
    public CardsPile ButCardDeck;

    int[] getCardIDs = new int[8];
    int[] butCardIDs = new int[8];

    int currentSetIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
      for(int i = 1; i < 9; i ++){
        int getID = PlayerPrefs.GetInt("ChosenGet" + i.ToString(), -1);
        int butID = PlayerPrefs.GetInt("ChosenBut" + i.ToString(), -1);
        getCardIDs[i - 1] = getID;
        butCardIDs[i - 1] = butID;

        Card butCard = Instantiate(butBackPrefab).GetComponent<Card>();
        butCard.Initialize(butID);
        ButCardDeck.Add(butCard, false);

        Card getCard = Instantiate(getBackPrefab).GetComponent<Card>();
        getCard.Initialize(getID);
        GetCardDeck.Add(getCard, false);
      }


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveCardsForward()
    {
        currentSetIndex++;

    }

    public void MoveCardsBackward()
    {
        currentSetIndex--;

    }
}
