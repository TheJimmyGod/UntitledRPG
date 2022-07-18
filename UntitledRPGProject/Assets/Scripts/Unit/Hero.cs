using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : NPC
{
    protected override void Start()
    {
        base.Start();
        mProperty = ((Companion)mProperty != null) ? (Companion)mProperty
    : Resources.Load<Companion>("Prefabs/Items/Companions/" + mName);
        Destroy(mInteraction.GetComponent<Billboard>());
        mCanvas.transform.localRotation = new Quaternion(0.0f, 208.0f, 0.0f, 1.0f);
        mProperty.Initialize(-1);
        Companion companion = (Companion)mProperty;
        companion.mTransform = transform;
    }

    public override IEnumerator Interact(Action Callback)
    {
        yield return StartCoroutine(base.Interact(Callback));
        yield return new WaitForSeconds(0.25f);
        if (isTrading) 
        {
            mProperty.End();
            Destroy(gameObject, 0.2f); 
        }
    }

    public override IEnumerator Event()
    {
        mTrigger = null;
        UIManager.Instance.ChangeTwoButtons(UIManager.Instance.mStorage.YesButtonImage,
UIManager.Instance.mStorage.NoButtonImage);
        UIManager.Instance.DisplayMoneyBoxInDialogue(true, mProperty.Value);
        UIManager.Instance.AddListenerRightButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        UIManager.Instance.AddListenerLeftButton(() => {
            if(PlayerController.Instance.mHeroes.Count >= 4)
            {
                m_DialogueQueue.Enqueue(new Dialogue("Hmm. It seems your party is full now.", Dialogue.TriggerType.None));
                mComplete = true;
            }
            else
            {
                if (mProperty.Value <= PlayerController.Instance.mGold)
                {
                    PlayerController.Instance.mGold -= mProperty.Value;
                    isTrading = true;
                }
                else
                    isTrading = false;

                if (isTrading)
                {
                    foreach (var dialogue in m_DialogueYesCase)
                        m_DialogueQueue.Enqueue(dialogue);
                    mProperty.Apply();
                }
                else
                    foreach (var dialogue in m_DialogueFailToTradeCase)
                        m_DialogueQueue.Enqueue(dialogue);
                mComplete = true;
            }

        });
        UIManager.Instance.DisplayEKeyInDialogue(false);
        UIManager.Instance.DisplayButtonsInDialogue(true);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayMoneyBoxInDialogue(false);
        UIManager.Instance.DisplayButtonsInDialogue(false);
    }
}
