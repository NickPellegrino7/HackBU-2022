using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

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

    public GameObject [] playerPrefabs = new GameObject [8];
    public GameObject butBackPrefab;
    public GameObject getBackPrefab;

    public TextMeshProUGUI JudgeName;

    public CardsPile getCenter;
    public CardsPile butCenter;

    private bool _getSelected = false;
    private bool _butSelected = false;
    private bool _canSelectBut = false;

    public List<ReinBot> bots = new List<ReinBot>();
    public int nBots;

    private Card _myCard;
    private List<Card> _botCards = new List<Card>();


    public GameObject botsHolder;

    void Start()
    {
        nBots = 4;// PlayerPrefs.GetInt("NumOpponents");
        for (int i = 0; i < nBots; i++)
		{
            GameObject go = new GameObject();
            GameObject botGo = Instantiate(go, botsHolder.transform);
            ReinBot bot = botGo.AddComponent<ReinBot>();
            bots.Add(bot);
		}

        for (int i = 1; i < deckSize + 1; i++)
        {
            Card card = Instantiate(getBackPrefab).GetComponent<Card>();
            card.Initialize(i);

            getDeck.Add(card, false);
        }

        for (int i = 1; i < deckSize + 1; i++)
        {
            Card card = Instantiate(butBackPrefab).GetComponent<Card>();
            card.Initialize(i);

            butDeck.Add(card, false);
        }

        for(int i = nBots+1 ; i < 8; i++)
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

    private IEnumerator DealCards()
	{
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(.2f);
            SpawnGetCard();
        }
        for (int i = 0; i < 4; i++)
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
				if (card)
				{
                    if(card.Type == "Get")
					{
                        if (_getSelected) return;
                        getCenter.Add(card);
                        getHand.Remove(card);
                        _getSelected = true;
                        
                        _myCard = card;
                        SubmitGetCard();
                    }
					else
					{
                        if (!_canSelectBut) return;
                        if (_butSelected) return;
                        butCenter.Add(card);
                        butHand.Remove(card);
                        _butSelected = true;

                        foreach(ReinBot bot in bots)
						{
                            Card getcard = getCenter.Cards[0];
                            string getString = "G" + getcard.Id.ToString();
                            string butString = "B" + card.Id.ToString();
                            bot.learnExperience(getString, butString);
                        }
                        

                        SubmitButCard();
                    }

                    
				}
			}            	    
        }
	}

    private void SubmitGetCard()
	{
        //List<Card> botcards = new List<Card>();

        foreach (ReinBot bot in bots)
		{
            Card card = bot.PickRandomGet();
            _botCards.Add(card);
		}

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
	}

    private void passInCard()
	{
        getDiscard.Add(_myCard);
        getCenter.Remove(_myCard);

        RecieveGetCard();
    }

    private void RecieveGetCard()
	{
        Card card = _botCards[nBots - 1];
        card.Flip();
        getCenter.Add(card);
        getDiscard.Remove(card);
        _canSelectBut = true;

    }

    private void SubmitButCard()
	{
        int i = 0;
        foreach (ReinBot bot in bots)
        {
            if(i == 0)
			{
                bot.chooseBut("G" + _myCard.Id.ToString());
			}
			else
			{
                bot.chooseBut("G" + _botCards[i-1].Id.ToString());
            }
            i++;
        }
    }
}
