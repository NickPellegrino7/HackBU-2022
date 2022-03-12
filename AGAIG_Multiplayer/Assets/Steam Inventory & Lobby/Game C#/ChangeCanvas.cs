using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrettArnett
{
    public class ChangeCanvas : MonoBehaviour
    {
        public GameObject CanvasA;
        public GameObject CanvasB;

        public void ToA()
        {
            CanvasA.SetActive(true);
            CanvasB.SetActive(false);
        }

        public void ToB()
        {
            CanvasA.SetActive(false);
            CanvasB.SetActive(true);
        }
    }
}