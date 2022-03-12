using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpponentSelect : MonoBehaviour
{
    public GameObject OpponentTextBox;
    public GameObject Slider;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("ToMainScene", 0);
        PlayerPrefs.SetInt("JudgeIndex", 1);
    }


    public void UpdateOpponentNumber()
    {
      TextMeshProUGUI opponentText = OpponentTextBox.GetComponent<TextMeshProUGUI>();
      int NumOpponents = (int)Slider.GetComponent<Slider>().value;
      opponentText.text = NumOpponents.ToString();
      PlayerPrefs.SetInt("NumOpponents", NumOpponents);
    }

    public void SubmitOpponentNumber()
    {

      SceneManager.LoadScene("Scenes/Main");
    }
}
