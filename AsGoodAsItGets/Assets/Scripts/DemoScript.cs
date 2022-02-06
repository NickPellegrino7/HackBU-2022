using UnityEngine;
using System.Collections;
using TMPro;

public class DemoScript : MonoBehaviour
{
    public int handSize;
    public int deckSize;

    public CardsPile butHand;
    public CardsPile getHand;
    public CardsPile butDeck;
    public CardsPile getDeck;

    public GameObject [] playerPrefabs = new GameObject [8];
    public GameObject butBackPrefab;
    public GameObject getBackPrefab;

    public TextMeshProUGUI JudgeName;

    public CardsPile getCenter;
    public CardsPile butCenter;

    private bool _getSelected = false;
    private bool _butSelected = false;

    void Start()
    {
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

        ArrayList players = new ArrayList();
        players.Add("Adiel");
        players.Add("Brendan");
        players.Add("Nick");
        players.Add("Richard");
        InstantiatePlayerList(players);

        foreach(GameObject player in playerPrefabs){
            player.gameObject.SetActive(false);
        }

        StartCoroutine(DealCards());
        
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
					}
					else
					{
                        if (_butSelected) return;
                        butCenter.Add(card);
                        butHand.Remove(card);
                        _butSelected = true;
                    }
				}
			}            	    
        }
	}
}
