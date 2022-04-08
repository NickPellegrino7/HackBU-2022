using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOtherPlayers : MonoBehaviour
{
    public GameObject playerClone;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        playerClone = GameObject.Find("Player (Clone)");
        if (playerClone) { playerClone.SetActive(false); }
    }
}
