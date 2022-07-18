using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : CollectableObject
{
    public int value = 2;
    bool isGained = false;
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Gain(other);
    }

    protected override void OnTriggerStay(Collider other)
    {
        Gain(other);
    }

    private void Gain(Collider other)
    {
        if (!_initialize)
            return;
        if (other.CompareTag("Player") && !isGained)
        {
            isGained = true;
            PlayerController.Instance.GetGold(value);
            mOwner.mCollectObjects.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
