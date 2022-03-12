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
    public bool isDisplay = false;
    private Card displayCard;

    public void SetState(float num) {
      state = num;
    }

    public void SetDisplay(Card card) {
      displayCard = card;
    }

    async void AnimateUp()
    {
      if (displayCard != null) {
        if (image.GetComponentInChildren<SpriteRenderer>().enabled) {
          displayCard.GetComponent<MouseCard>().image.GetComponentInChildren<SpriteRenderer>().sprite = image.GetComponentInChildren<SpriteRenderer>().sprite;
        }
      }
      bool ready = false;
      while(ready == false) {
        await Task.Delay(TimeSpan.FromSeconds(waitTime));
        if (localLock == false) {
          localLock = true;
          ready = true;
        }
      }
      Vector3 pos = image.transform.position;
      for (float i = pos.z; i < transform.position.z+(state); i += 0.1f) {
        pos = image.transform.position;
        pos.z = i;
        image.transform.position = pos;
        await Task.Delay(TimeSpan.FromSeconds(waitTime));
      }
      pos = transform.position;
      pos.z = transform.position.z+state;
      image.transform.position = pos;
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
      Vector3 pos = image.transform.position;
      for (float i = pos.z; i > transform.position.z; i -= 0.1f) {
        pos = image.transform.position;
        pos.z = i;
        image.transform.position = pos;
        await Task.Delay(TimeSpan.FromSeconds(waitTime));
      }
      image.transform.position = transform.position;;
      localLock = false;
    }

    void OnMouseEnter()
    {
      if (!isDisplay) {
        AnimateUp();
      }
    }

    void OnMouseOver()
    {
      if (!isDisplay) {
        mouseOver = 0;
      }
    }

    void OnMouseExit()
    {
      if (!isDisplay) {
        AnimateDown();
      }
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
