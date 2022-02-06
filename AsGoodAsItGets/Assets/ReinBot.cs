using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ReinBot : MonoBehaviour
{
    // put global values up here

    private float ALPHA = 0.5f;
    // private float DISCOUNT = 0.9f; // there's only one trasition to do, so no reason to check future moves
    // private float EPSILON = 0.2f; // we never need to move randomly

    private float[,,,] Q_Values = new float[5,3,5,3];

    private IDictionary<string, int[]> cardValues = new Dictionary<string, int[]>();

    public List<Card> GetCards = new List<Card>();
    public List<Card> ButCards = new List<Card>();
    

    // Start is called before the first frame update
    void Start()
    {

        using(var reader = new StreamReader(@"./Assets/AGAIG_Data.csv"))
        {
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                string key = values[1];

                int[] category_severity = {Int32.Parse(values[2]), Int32.Parse(values[3])};

                cardValues.Add(key, category_severity);

            }
        }

        DontDestroyOnLoad(this.gameObject);

        // Example :D
        // learnExperience("G7", "B5");
        // string[] buts_in_hand = {"B99", "B2", "B5", "B88"};
        // string result = chooseBut("G7", buts_in_hand);
        // Debug.Log(result);
    }

    int[] getState(string card) {
        // card = "G2" or "B17"

        int[] cat_sev_pair = cardValues[card];

        int category = cat_sev_pair[0]; // 0-4 is the category
        int severity = cat_sev_pair[1]; // 0(mild)-2(severe) is the severity

        // Leave this part alone:
        int[] state = new int[2];
        state[0] = category;
        state[1] = severity;
        return state;
    }

    void setGetQValue(string get, string but, float new_value) {
        Q_Values[get[0],get[1],but[0],but[1]] = new_value;
    }

    public void learnExperience(string get, string but) {
        int[] get_state = getState(get);
        int[] but_state = getState(but);
        float q_value = Q_Values[get_state[0],get_state[1],but_state[0],but_state[1]];

        int REWARD = 1; // reward is always postitive since a player chose it, so it's good
        // float sample = REWARD + (DISCOUNT * butQValue(but));
        float sample = REWARD; // there's only one trasition to do, so no reason to check future moves
        float new_value = ((1 - ALPHA) * (q_value)) + ((ALPHA) * (sample));
        Q_Values[get_state[0],get_state[1],but_state[0],but_state[1]] = new_value;
    }

    public string chooseBut(string get) { // buts_in_hand should be length 4        
        string[] buts_in_hand = new string [4];
        int i = 0;
        foreach(Card card in ButCards)
		{
            buts_in_hand[i] = "B" + card.Id.ToString();
            i++;
		}
        int[] get_state = getState(get);
        int[] but_state = getState(buts_in_hand[0]);
        float q_value = Q_Values[get_state[0],get_state[1],but_state[0],but_state[1]];

        string best_but = buts_in_hand[0];
        float best_q_value = Q_Values[get_state[0],get_state[1],but_state[0],but_state[1]];

        int bestIndex = 0;
        for (i = 1; i < 4; i++) {
            but_state = getState(buts_in_hand[i]);
            float new_q_value = Q_Values[get_state[0],get_state[1],but_state[0],but_state[1]];
            if (new_q_value > best_q_value) {
                best_q_value = new_q_value;
                best_but = buts_in_hand[i];
                bestIndex = i;
            }
        }

        ButCards.RemoveAt(bestIndex);

        return best_but;
    }

    public Card PickRandomGet() 
    {
		System.Random rnd = new System.Random();
        int num = rnd.Next(4);

        Card card = GetCards[num];
        GetCards.RemoveAt(num);

        return card;
    }
}
