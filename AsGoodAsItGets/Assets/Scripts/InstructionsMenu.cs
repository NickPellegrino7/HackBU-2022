using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionsMenu : MonoBehaviour
{

    public GameObject Instructions1;
    public GameObject Instructions2;
    public GameObject Arrow;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeInstructions()
    {
      Instructions1.SetActive(!Instructions1.activeInHierarchy);
      Instructions2.SetActive(!Instructions2.activeInHierarchy);
      Vector3 angles = Arrow.transform.localEulerAngles;
      Arrow.transform.localEulerAngles = new Vector3(angles.x, 180 - angles.y, angles.z);
    }

    public void ReturnToMainMenu()
    {
      SceneManager.LoadScene("Hand&Deck/Scenes/Title Scene.unity");
    }
}
