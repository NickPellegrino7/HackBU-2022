using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DemoScript : MonoBehaviour
{
    public int handSize;
    public int deckSize;

    public CardsPile butHand;
    public CardsPile getHand;
    public CardsPile butDeck;
    public CardsPile getDeck;
    public CardsPile butDiscard;
    public CardsPile getDiscard;

	public CardsPile displayDeck;
	private Card displayCard;

    public GameObject [] playerPrefabs = new GameObject [8];
    public GameObject butBackPrefab;
    public GameObject getBackPrefab;
	public GameObject displayPrefab;

    public TextMeshProUGUI JudgeName;

    public CardsPile getCenter;
    public CardsPile butCenter;

    private bool _getSelected = false;
    private bool _butSelected = false;
    private bool _canSelectBut = false;

    public List<ReinBot> bots = new List<ReinBot>();
    public int nBots;

    private List<Card> _botCards = new List<Card>();

    private Card yourGet;
    private Card yourBut;

    public GameObject botsHolder;

    private int judgeIndex;

    void Start()
    {
        if(PlayerPrefs.GetInt("ToMainScene", 0) == 0){
            FullStart();
		}
		else
		{
            Restart();
		}
        judgeIndex = PlayerPrefs.GetInt("JudgeIndex", 1);
    }

    private void FullStart()
	{
        nBots = PlayerPrefs.GetInt("NumOpponents", 4);
        for (int i = 0; i < nBots; i++)
        {
            GameObject go = new GameObject();
            GameObject botGo = Instantiate(go, botsHolder.transform);
            ReinBot bot = botGo.AddComponent<ReinBot>();
            bots.Add(bot);
        }

        displayCard = Instantiate(displayPrefab).GetComponent<Card>();
        displayDeck.Add(displayCard, false);

        for (int i = 1; i < deckSize + 1; i++)
        {
            Card card = Instantiate(getBackPrefab).GetComponent<Card>();
            card.gameObject.name = card.gameObject.name + i.ToString();
            card.Initialize(i);
            card.GetComponent<MouseCard>().SetDisplay(displayCard);

            getDeck.Add(card, false);
        }

        for (int i = 1; i < deckSize + 1; i++)
        {
            Card card = Instantiate(butBackPrefab).GetComponent<Card>();
            card.gameObject.name = card.gameObject.name + i.ToString();
            card.Initialize(i);
            card.GetComponent<MouseCard>().SetDisplay(displayCard);

            butDeck.Add(card, false);
        }

        ShuffleDecks();


        for (int i = nBots + 1; i < 8; i++)
        {
            playerPrefabs[i].SetActive(false);
        }

        StartCoroutine(DealCards());

        foreach (ReinBot bot in bots)
        {
            for (int i = 0; i < 4; i++)
            {
                Card card = getDeck.Cards[getDeck.Cards.Count - 1];
                getDiscard.Add(card);
                getDeck.Remove(card);

                bot.GetCards.Add(card);

                card = butDeck.Cards[butDeck.Cards.Count - 1];
                butDiscard.Add(card);
                butDeck.Remove(card);

                bot.ButCards.Add(card);
            }
        }
    }

    private void Restart()
	{
        nBots = 4;// PlayerPrefs.GetInt("NumOpponents");
        bots = new List<ReinBot>(FindObjectsOfType<ReinBot>());

        //displayCard = Instantiate(displayPrefab).GetComponent<Card>();
        //displayDeck.Add(displayCard, false);

        CardsPile[] piles = FindObjectsOfType<CardsPile>(true);

		foreach (CardsPile pile in piles)
		{
            if (pile.gameObject.name == "GetDeck")
            {
                pile.gameObject.SetActive(true);
                getDeck = pile;
            }
            else if (pile.gameObject.name == "ButDeck")
            {
                pile.gameObject.SetActive(true);
                butDeck = pile;
            }
            else if (pile.gameObject.name == "GetHand")
            {
                getHand = pile;
            }
            else if (pile.gameObject.name == "ButHand")
            {
                pile.gameObject.SetActive(true);
                butHand = pile;
            }
            else if (pile.gameObject.name == "GetDiscard")
            {
                pile.gameObject.SetActive(true);
                getDiscard = pile;
            }
            else if (pile.gameObject.name == "ButDiscard")
            {
                pile.gameObject.SetActive(true);
                butDiscard = pile;
            }
            else if (pile.gameObject.name == "GetCenter")
            {
                pile.gameObject.SetActive(true);
                getCenter = pile;
            }
            else if (pile.gameObject.name == "ButCenter")
            {
                pile.gameObject.SetActive(true);
                butCenter = pile;
            }
        }

        for (int i = nBots + 1; i < 8; i++)
        {
            playerPrefabs[i].SetActive(false);
        }

        StartCoroutine(DealCards());

        foreach (ReinBot bot in bots)
        {
            Card card = getDeck.Cards[getDeck.Cards.Count - 1];
            getDiscard.Add(card);
            getDeck.Remove(card);

            bot.GetCards.Add(card);

            card = butDeck.Cards[butDeck.Cards.Count - 1];
            butDiscard.Add(card);
            butDeck.Remove(card);

            bot.ButCards.Add(card);
        }
    }

    private IEnumerator DealCards()
	{
        for (int i = getHand.Cards.Count; i < 4; i++)
        {
            yield return new WaitForSeconds(.2f);
            SpawnGetCard();
        }
        for (int i = butHand.Cards.Count; i < 4; i++)
        {
            yield return new WaitForSeconds(.2f);
            SpawnButCard();
        }
    }

    public void SpawnGetCard()
    {
        if (getDeck.Cards.Count == 0)
            return;

        Card card = getDeck.Cards[getDeck.Cards.Count - 1];
        getDeck.Remove(card);
        getHand.Add(card, 0);
        card.Flip();
    }

    public void RemoveGetCard()
    {
        if (getHand.Cards.Count == 0)
            return;

        Card card = getHand.Cards[getHand.Cards.Count - 1];
        getHand.Remove(card);
        getDeck.Add(card);
    }

    public void SpawnButCard()
    {
        if (butDeck.Cards.Count == 0)
            return;

        Card card = butDeck.Cards[butDeck.Cards.Count - 1];
        butDeck.Remove(card);
        butHand.Add(card, 0);
        card.Flip();
    }

    public void RemoveButCard()
    {
        if (butHand.Cards.Count == 0)
            return;

        Card card = butHand.Cards[butHand.Cards.Count - 1];
        butHand.Remove(card);
        butDeck.Add(card);
    }

    public void InstantiatePlayerList(ArrayList players)
    {
        int i = 0;
        foreach(GameObject player in playerPrefabs)
        {
            if(players.Count > i){
                player.GetComponentInChildren<TextMeshProUGUI>().text = (string)players[i];
            }
            else
            {
                player.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
            i++;
        }
    }

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, ~LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Collide))
			{
                Card card = hit.collider.GetComponent<Card>();
				if ((card) && (card.GetComponentInChildren<SpriteRenderer>().enabled) && (!card.GetComponent<MouseCard>().isDisplay)) // check a card exists under the mouse, and that card is face-up, and it's not the display card
				{
                    if(card.Type == "Get")
					{
                        if (_getSelected) return;
                        getCenter.Add(card);
                        getHand.Remove(card);
                        _getSelected = true;

                        yourGet = card;
                        SubmitGetCard();
                    }
					else
					{
                        if (!_canSelectBut) return;
                        if (_butSelected) return;
                        butCenter.Add(card);
                        butHand.Remove(card);
                        _butSelected = true;
                        yourBut = card;
                        foreach (ReinBot bot in bots)
						{
                            Card getcard = getCenter.Cards[0];
                            string getString = "G" + getcard.Id.ToString();
                            string butString = "B" + card.Id.ToString();
                            bot.learnExperience(getString, butString);
                        }
                        SubmitButCard();
                        SceneManager.LoadScene("Scenes/Judge");
                    }
				}
			}
        }
	}

    private void SubmitGetCard()
	{
        //List<Card> botcards = new List<Card>();
        int i = 2;
        foreach (ReinBot bot in bots)
		{
            
            if(i-1 == judgeIndex)
			{
                PlayerPrefs.SetInt("ChosenGet" + i.ToString(), -1);
                _botCards.Add(null);
			}
			else
			{
                Card card = bot.PickRandomGet();
                PlayerPrefs.SetInt("ChosenGet" + i.ToString(), card.Id);
                _botCards.Add(card);
			}
            i++;
		}
        PlayerPrefs.SetInt("ChosenGet0", yourGet.Id);

        playerPrefabs[0].GetComponentInChildren<Image>().enabled = true;
        StartCoroutine(SetOpponentCheckmarks());
	}

    private IEnumerator SetOpponentCheckmarks()
	{
        System.Random rnd = new System.Random();
        for (int i = 1; i < 8; i++)
		{
            float f = (float)rnd.NextDouble();
            yield return new WaitForSeconds(f);
            playerPrefabs[i].GetComponentInChildren<Image>().enabled = true;
		}

        passInCard();
        yield return new WaitForSeconds(.5f);
        RecieveGetCard();
    }

    private void passInCard()
	{
        getDiscard.Add(yourGet);
        getCenter.Remove(yourGet);
    }

    private void RecieveGetCard()
	{
        Card card;
        if (judgeIndex == nBots)
		{
            card = _botCards[nBots - 2];
        }
        else
		{
            card = _botCards[nBots - 1];
		}
        card.Flip();
        getCenter.Add(card);
        getDiscard.Remove(card);
        _canSelectBut = true;

    }

    private void SubmitButCard()
	{
        PlayerPrefs.SetInt("Chosenbut0", yourBut.Id);

        for (int i = 0; i < bots.Count; i++)
		{
            if(i == 0)
			{
                if(i+1 == judgeIndex)
				{
                    bots[i+1].chooseBut("G" + yourBut.Id.ToString());
                    PlayerPrefs.SetInt("ChosenBut" + i.ToString(), -1);
                    PlayerPrefs.SetInt("ChosenBut" + i+1.ToString(), -1);
                    i++;
                }
				else
				{
                    bots[i].chooseBut("G" + yourBut.Id.ToString());
                    PlayerPrefs.SetInt("ChosenBut" + i.ToString(), yourBut.Id);
                }
			}
			else
			{
                if (i + 1 == judgeIndex)
                {
                    if(judgeIndex == nBots)
					{
                        bots[i + 1].chooseBut("G" + yourBut.Id.ToString());
                        PlayerPrefs.SetInt("ChosenBut" + i.ToString(), -1);
                        i++;
                    }
					else
					{
                        bots[i+1].chooseBut("G" + _botCards[i - 1].Id.ToString());
                        PlayerPrefs.SetInt("ChosenBut" + i.ToString(), -1);
                        PlayerPrefs.SetInt("ChosenBut" + i+1.ToString(), _botCards[i-1 - 1].Id);
                        i++;
					}
                }
				else
				{
                    bots[i].chooseBut("G" + _botCards[i - 1].Id.ToString());
                    PlayerPrefs.SetInt("ChosenBut" + i.ToString(), _botCards[i - 1].Id);
                }
            }
		}
    }

    private void ShuffleDecks()
    {
        System.Random rnd = new System.Random();
        for (int i = 0; i < getDeck.Cards.Count * 2; i++)
		{
            int index1 = rnd.Next(getDeck.Cards.Count);
            int index2 = rnd.Next(getDeck.Cards.Count);

            Card c1 = getDeck.Cards[index1];
            getDeck.RemoveAt(index1);
            getDeck.Add(c1, index2);

        }

        for (int i = 0; i < butDeck.Cards.Count; i++)
        {
            int index1 = rnd.Next(butDeck.Cards.Count);
            int index2 = rnd.Next(butDeck.Cards.Count);

            Card c1 = butDeck.Cards[index1];
            butDeck.RemoveAt(index1);
            butDeck.Add(c1, index2);
        }
    }
}
