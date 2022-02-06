using System.Collections;
using System.Collections.Generic;
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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateOpponentNumber()
    {
      Text opponentText = OpponentTextBox.GetComponent<Text>();
      int NumOpponents = (int)Slider.GetComponent<Slider>().value;
      opponentText.text = NumOpponents.ToString();
      PlayerPrefs.SetInt("NumOpponents", NumOpponents);
    }

    public void SubmitOpponentNumber()
    {

      SceneManager.LoadScene("Scenes/Demo");
    }
}
