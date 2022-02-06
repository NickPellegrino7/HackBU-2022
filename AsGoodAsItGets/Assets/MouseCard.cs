using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

public class MouseCard : MonoBehaviour
{
    public GameObject image;
    public float waitTime = 0.005f;
    public bool localLock;
    public int mouseOver = -1;
    public bool active = true;
    public float state = 1;

    public void SetState(float num) {
      state = num;
    }

    async void AnimateUp()
    {
      bool ready = false;
      while(ready == false) {
        await Task.Delay(TimeSpan.FromSeconds(waitTime));
        if (localLock == false) {
          localLock = true;
          ready = true;
        }
      }
      Vector3 pos = image.GetComponent<Transform>().position;
      for (float i = pos.z; i < GetComponent<Transform>().position.z+(state); i += 0.1f) {
        pos = image.GetComponent<Transform>().position;
        pos.z = i;
        image.GetComponent<Transform>().position = pos;
        await Task.Delay(TimeSpan.FromSeconds(waitTime));
      }
      pos = GetComponent<Transform>().position;
      pos.z = GetComponent<Transform>().position.z+state;
      image.GetComponent<Transform>().position = pos;
      localLock = false;
    }

    async void AnimateDown()
    {
      bool ready = false;
      while(ready == false) {
        await Task.Delay(TimeSpan.FromSeconds(waitTime));
        if (localLock == false) {
          localLock = true;
          ready = true;
        }
      }
      Vector3 pos = image.GetComponent<Transform>().position;
      for (float i = pos.z; i > GetComponent<Transform>().position.z; i -= 0.1f) {
        pos = image.GetComponent<Transform>().position;
        pos.z = i;
        image.GetComponent<Transform>().position = pos;
        await Task.Delay(TimeSpan.FromSeconds(waitTime));
      }
      image.GetComponent<Transform>().position = GetComponent<Transform>().position;;
      localLock = false;
    }

    void OnMouseEnter()
    {
      AnimateUp();
    }

    void OnMouseOver()
    {
      mouseOver = 0;
    }

    void OnMouseExit()
    {
      AnimateDown();
    }

    async void Update()
    {
      await Task.Delay(TimeSpan.FromSeconds(waitTime));
      if ((mouseOver >= 50) && (mouseOver != -1)) {
        AnimateDown();
        mouseOver = -1;
      } else if ((mouseOver >= 0) && (mouseOver < 50)) {
        mouseOver += 1;
      }
    }
}
