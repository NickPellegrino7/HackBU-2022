using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardsPile : MonoBehaviour
{
	public float height = 0.5f;
	public float width = 1f;
	[Range(0f, 90f)] public float maxCardAngle = 5f;
	public float yPerCard = -0.005f;
	public float zDistance;

	public float moveDuration = 1f;
	public Transform cardHolderPrefab;

	readonly List<Card> cards = new List<Card>();

	public List<Card> Cards => new List<Card>(cards);

	public event Action<int> OnCountChanged;

	readonly List<Transform> cardsHolders = new List<Transform>();

	public bool _dontDestroyOnLoad = false;

	bool updatePositions;
	readonly List<Card> forceSetPosition = new List<Card>();

	private void Start()
	{
		if (_dontDestroyOnLoad) DontDestroyOnLoad(this.gameObject);
	}

	public void Add(Card card, bool moveAnimation = true) => Add(card, -1, moveAnimation);

	public void Add(Card card, int index, bool moveAnimation = true)
	{
		Transform cardHolder = GetCardHolder();

		if (index == -1)
		{
			cards.Add(card);
			cardsHolders.Add(cardHolder);
		}
		else
		{
			cards.Insert(index, card);
			cardsHolders.Insert(index, cardHolder);
		}

		updatePositions = true;

		if (!moveAnimation)
			forceSetPosition.Add(card);

		OnCountChanged?.Invoke(cards.Count);
	}

	public void Remove(Card card)
	{
		if (!cards.Contains(card))
			return;

		Transform cardHolder = cardsHolders[cards.IndexOf(card)];
		cardsHolders.Remove(cardHolder);
		Destroy(cardHolder.gameObject);

		cards.Remove(card);
		card.transform.DOKill();
		card.transform.SetParent(null);
		updatePositions = true;

		OnCountChanged?.Invoke(cards.Count);
	}

	public void RemoveAt(int index)
	{
		Remove(cards[index]);
	}

	public void RemoveAll()
	{
		while (cards.Count > 0)
			Remove(cards[0]);
	}

	public void DestroyAll()
	{
		while (cards.Count > 0) {
			Card card = cards[0];
			Remove(card);
			Destroy(card.gameObject);
		}
	}

	Transform GetCardHolder()
	{
		Transform cardHolder = Instantiate(cardHolderPrefab, transform, false);
		return cardHolder;
	}

	void UpdatePositions()
	{
		float radius = Mathf.Abs(height) < 0.001f
			? width * width / 0.001f * Mathf.Sign(height)
			: height / 2f + width * width / (8f * height);

		float angle = 2f * Mathf.Asin(0.5f * width / radius) * Mathf.Rad2Deg;
		angle = Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(angle), maxCardAngle * (cards.Count - 1));
		float cardAngle = cards.Count == 1 ? 0f : angle / (cards.Count - 1f);

		for (int i = 0; i < cards.Count; i++)
		{
			cards[i].transform.SetParent(transform, true);

			Vector3 position = new Vector3(0f, radius, 0f);
			position = Quaternion.Euler(0f, 0f, angle / 2f - cardAngle * i) * position;
			position.y += height - radius;
			position += i * new Vector3(0f, yPerCard, zDistance);

			cardsHolders[i].transform.localPosition = position;
			cardsHolders[i].transform.localEulerAngles = new Vector3(0f, 0f, angle / 2f - cardAngle * i);

			cards[i].transform.SetParent(cardsHolders[i].transform, true);

			if (!forceSetPosition.Contains(cards[i]))
			{
				cards[i].transform.DOKill();
				cards[i].transform.DOLocalMove(Vector3.zero, moveDuration);
				cards[i].transform.DOLocalRotate(Vector3.zero, moveDuration);
				cards[i].transform.DOScale(Vector3.one, moveDuration);
			}
			else
			{
				forceSetPosition.Remove(cards[i]);

				cards[i].transform.localPosition = Vector3.zero;
				cards[i].transform.localRotation = Quaternion.identity;
				cards[i].transform.localScale = Vector3.one;
			}
		}
	}

	void LateUpdate()
	{
		if (updatePositions)
		{
			updatePositions = false;
			UpdatePositions();
		}
	}

	void OnValidate()
	{
		updatePositions = true;
	}

	public int getCount()
	{
		return cards.Count;
	}

	public Card getCard(int cardNumber)
	{
		return cards[cardNumber];
	}

	public void DestroyAllCards()
	{
		while(cards.Count > 0) {
			Card myCard = cards[0];
			Remove(myCard);
			Destroy(myCard);
		}
	}

	public bool InHand(int cardNumber) {
		for (int i = 0; i < cards.Count; i++) {
			if (cards[i].Id == cardNumber) {
				return true;
			}
		}
		return false;
	}
}
