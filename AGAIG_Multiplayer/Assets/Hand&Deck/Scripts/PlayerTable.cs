﻿using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// TODO: Somehow anchor the CardsPile objects to the camera, in case player has a weird screen size

public class PlayerTable : MonoBehaviour
{
  public static MultiplayerAGAIG MultiplayerManager;

  public CardsPile getDeck;
  public CardsPile butDeck;

  public GameObject getBackPrefab;
  public GameObject butBackPrefab;

  private bool _multiplayerFound;

  private int deckSize = 30;

  void Start() {
    // Find Multiplayer Manager
    MultiplayerManager = GameObject.FindObjectOfType<MultiplayerAGAIG>();

    // Set Up Card Decks
    for (int i = 1; i < deckSize + 1; i++) {
        Card card = Instantiate(getBackPrefab).GetComponent<Card>();
        // card.gameObject.name = card.gameObject.name + i.ToString();
        // card.Initialize(i);
        getDeck.Add(card, false);
    }
    for (int i = 1; i < deckSize + 1; i++) {
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
    }
    // Other updates, given that multiplayer has connected
    else {
      // Make sure none of the player tables overlap by moving them 30 * connection_id on the x-axis
      this.transform.position = new Vector3((MultiplayerManager._myPlayer.ConnectionId * 30), 0, 0);
    }
  }

}
