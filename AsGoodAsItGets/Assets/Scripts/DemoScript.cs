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

    void Start()
    {
        for (int i = 0; i < handSize; i++)
            getHand.Add(Instantiate(getBackPrefab), false);

        for (int i = 0; i < deckSize; i++)
            getDeck.Add(Instantiate(getBackPrefab), false);

        for (int i = 0; i < handSize; i++)
            butHand.Add(Instantiate(butBackPrefab), false);

        for (int i = 0; i < deckSize; i++)
            butDeck.Add(Instantiate(butBackPrefab), false);

        ArrayList players = new ArrayList();
        players.Add("Adiel");
        players.Add("Brendan");
        players.Add("Nick");
        players.Add("Richard");
        InstantiatePlayerList(players);

        foreach(GameObject player in playerPrefabs){
            player.gameObject.SetActive(false);
        }
    }

    public void SpawnGetCard()
    {
        if (getDeck.Cards.Count == 0)
            return;

        GameObject card = getDeck.Cards[getDeck.Cards.Count - 1];
        getDeck.Remove(card);
        getHand.Add(card, 0);
    }

    public void RemoveGetCard()
    {
        if (getHand.Cards.Count == 0)
            return;

        GameObject card = getHand.Cards[getHand.Cards.Count - 1];
        getHand.Remove(card);
        getDeck.Add(card);
    }

    public void SpawnButCard()
    {
        if (butDeck.Cards.Count == 0)
            return;

        GameObject card = butDeck.Cards[butDeck.Cards.Count - 1];
        butDeck.Remove(card);
        butHand.Add(card, 0);
    }

    public void RemoveButCard()
    {
        if (butHand.Cards.Count == 0)
            return;

        GameObject card = butHand.Cards[butHand.Cards.Count - 1];
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
}
