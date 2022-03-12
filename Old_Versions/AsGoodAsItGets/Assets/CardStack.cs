using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStack : MonoBehaviour
{
    private List<int> _availableCards = new List<int>();
    private List<int> _discardPile = new List<int>();

    private CardsPile _cardsPile;

    // Start is called before the first frame update
    void Start()
    {
        _cardsPile = GetComponent<CardsPile>();

        for(int i = 1; i < 101; i++)
		{
            _availableCards.Add(i);
		}
    }

    public void Deal()
	{

	}
}
