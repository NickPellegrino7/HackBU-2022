using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WinnerScript : MonoBehaviour
{
    public GameObject WinnerText;

    // Start is called before the first frame update
    void Start()
    {
      TextMeshProUGUI winnerText = WinnerText.GetComponent<TextMeshProUGUI>();
      string winner = PlayerPrefs.GetString("Winner", "No One");
      winnerText.text = winner + " Wins!";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReturnToMainMenu()
    {
      SceneManager.LoadScene("Hand&Deck/Scenes/Title Scene.unity");
    }

    public void EndGame()
    {
      Application.Quit();
    }
}
