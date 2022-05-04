using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

// TODO: Somehow anchor the CardsPile objects to the camera, in case player has a weird screen size

public class PlayerTable : MonoBehaviour
{
  public static MultiplayerAGAIG MultiplayerManager;

  public CardsPile getDeck;
  public CardsPile butDeck;

  public CardsPile getHand;
  public CardsPile butHand;

  public CardsPile discardDeck;
  // public CardsPile playDeck;
  // public CardsPile selectedDeck;

  public GameObject getBackPrefab;
  public GameObject butBackPrefab;

  private Ray _ray;
  private RaycastHit _hit;

  private bool _multiplayerFound;

  private int _deckSize = 30;

  void Start() {
    // Find Multiplayer Manager
    MultiplayerManager = GameObject.FindObjectOfType<MultiplayerAGAIG>();

    // Set Up Card Decks
    for (int i = 1; i < _deckSize + 1; i++) {
        Card card = Instantiate(getBackPrefab).GetComponent<Card>();
        // card.gameObject.name = card.gameObject.name + i.ToString();
        // card.Initialize(i);
        getDeck.Add(card, false);
    }
    for (int i = 1; i < _deckSize + 1; i++) {
        Card card = Instantiate(butBackPrefab).GetComponent<Card>();
        // card.gameObject.name = card.gameObject.name + i.ToString();
        // card.Initialize(i);
        butDeck.Add(card, false);
    }
    _multiplayerFound = false;
  }

  void Update() {
    // Connect to multiplayer
    if (!_multiplayerFound && MultiplayerManager != null) {
      _multiplayerFound = true;
      MultiplayerManager.SendMessageOut("Hello World!");
    } else {
      if (MultiplayerManager.drawReady) {
        MultiplayerManager.drawReady = false;
        CleanHand(); // Discard any cards which shouldn't be in the hand
        StartCoroutine(Draw()); // Draw cards which should be in the hand but are missing
      }
    }

    _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if(Physics.Raycast(_ray, out _hit)) {
        if (_hit.collider.GetComponent<Card>() != null) {
          _hit.collider.GetComponent<Card>().HoverOver();
        }
    }
  }

  public void CleanHand() {
    for (int i = 0; i < getHand.getCount(); i++) {
      Card card = getHand.getCard(i);
      if (Array.IndexOf(MultiplayerManager._localGetCards, card.Id) == -1) {
        getHand.Remove(card);
        discardDeck.Add(card, true);
      }
      i--;
    }
    for (int i = 0; i < butHand.getCount(); i++) {
      Card card = butHand.getCard(i);
      if (Array.IndexOf(MultiplayerManager._localButCards, card.Id) == -1) {
        butHand.Remove(card);
        discardDeck.Add(card, true);
      }
      i--;
    }
    discardDeck.DestroyAll();
  }

  private IEnumerator Draw() {
    int cardNumber;
    for (int i = 0; i < 4; i++) {
      cardNumber = MultiplayerManager._localGetCards[i];
      if (!getDeck.InHand(cardNumber)) {
        Card card = Instantiate(getBackPrefab).GetComponent<Card>();
        card.Initialize(cardNumber);
        Card getDummy = getDeck.getCard(0);
        getDeck.RemoveAt(0);
        getDeck.Add(card, false);
        yield return new WaitForSeconds(.2f);
        getDeck.Remove(card);
        card.Flip();
        getHand.Add(card); // Animation from getDeck --> getHand
        getDeck.Add(getDummy, getDeck.getCount()-1, false);
        yield return new WaitForSeconds(.2f);
      }
    }
    for (int i = 0; i < 4; i++) {
      cardNumber = MultiplayerManager._localButCards[i];
      if (!butDeck.InHand(cardNumber)) {
        Card card = Instantiate(butBackPrefab).GetComponent<Card>();
        card.Initialize(cardNumber);
        Card butDummy = butDeck.getCard(0);
        butDeck.RemoveAt(0);
        butDeck.Add(card, false);
        yield return new WaitForSeconds(.2f);
        butDeck.Remove(card);
        card.Flip();
        butHand.Add(card); // Animation from butDeck --> butHand
        butDeck.Add(butDummy, butDeck.getCount()-1, false);
        yield return new WaitForSeconds(.2f);
      }
    }
  }

}
