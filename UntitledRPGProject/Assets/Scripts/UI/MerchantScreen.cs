using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantScreen : MonoBehaviour
{
    private Transform mBoarder;
    private Slot[] slots = new Slot[3];
    void Start()
    {
        mBoarder = transform;
        mBoarder.gameObject.SetActive(false);
        slots[0] = transform.Find("Slots").Find("Slot1").GetComponent<Slot>();
        slots[1] = transform.Find("Slots").Find("Slot2").GetComponent<Slot>();
        slots[2] = transform.Find("Slots").Find("Slot3").GetComponent<Slot>();
    }

    public void InitializeALL(ref GameObject[] obj)
    {
        for (int i = 0; i < slots.Length; ++i)
        {
            if (obj[i].GetComponent<Item>().isSold)
                continue;
            slots[i].Initialize(ref obj[i]);
        }
    }

    public void Active(bool active)
    {
        for (int i = 0; i < slots.Length; ++i)
            slots[i].gameObject.SetActive(active);
        mBoarder.gameObject.SetActive(active);
    }
}
