using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DemoScript : MonoBehaviour
{
    public int handSize;
    public int deckSize;

    public CardsPile butHand;
    public CardsPile getHand;
    public CardsPile butDeck;
    public CardsPile getDeck;

    public GameObject butBackPrefab;
    public GameObject getBackPrefab;
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject player3Prefab;
    public GameObject player4Prefab;
    public GameObject player5Prefab;
    public GameObject player6Prefab;
    public GameObject player7Prefab;
    public GameObject player8Prefab;

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
        if(players.Count > 0){
            player1Prefab.GetComponent<UnityEngine.UI.Text>().text = (string)players[0];
        } else {
            player1Prefab.GetComponent<UnityEngine.UI.Text>().text = " ";
        }
        if(players.Count > 1){
            player2Prefab.GetComponent<UnityEngine.UI.Text>().text = (string)players[1];
        } else {
            player2Prefab.GetComponent<UnityEngine.UI.Text>().text = " ";
        }
        if(players.Count > 2){
            player3Prefab.GetComponent<UnityEngine.UI.Text>().text = (string)players[2];
        } else {
            player3Prefab.GetComponent<UnityEngine.UI.Text>().text = " ";
        }
        if(players.Count > 3){
            player4Prefab.GetComponent<UnityEngine.UI.Text>().text = (string)players[3];
        } else {
            player4Prefab.GetComponent<UnityEngine.UI.Text>().text = " ";
        }
        if(players.Count > 4){
            player5Prefab.GetComponent<UnityEngine.UI.Text>().text = (string)players[4];
        } else {
            player5Prefab.GetComponent<UnityEngine.UI.Text>().text = " ";
        }
        if(players.Count > 5){
            player6Prefab.GetComponent<UnityEngine.UI.Text>().text = (string)players[5];
        } else {
            player6Prefab.GetComponent<UnityEngine.UI.Text>().text = " ";
        }
        if(players.Count > 6){
            player7Prefab.GetComponent<UnityEngine.UI.Text>().text = (string)players[6];
        } else {
            player7Prefab.GetComponent<UnityEngine.UI.Text>().text = " ";
        }
        if(players.Count > 7){
            player8Prefab.GetComponent<UnityEngine.UI.Text>().text = (string)players[7];
        } else {
            player8Prefab.GetComponent<UnityEngine.UI.Text>().text = " ";
        }
    }
}
