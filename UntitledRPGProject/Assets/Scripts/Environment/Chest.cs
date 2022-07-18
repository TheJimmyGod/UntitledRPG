using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableEnvironment, ILockable
{
    [SerializeField]
    private float mItemDropRate = 30.0f;

    [SerializeField]
    [Range(10, 1000)]
    private int mMaximumGold = 200;

    private int mMoney = 0;

    private List<GameObject> mItems;
    [SerializeField]
    protected List<Dialogue> m_DialogueYesCase = new List<Dialogue>();
    [SerializeField]
    protected List<Dialogue> m_DialogueNoCase = new List<Dialogue>();
    protected delegate IEnumerator TriggerEvent();
    protected TriggerEvent mTrigger;
    private bool _DialogueComplete = false;
    public bool isLock = false;
    private Animator mAnimator;

    public override void Initialize(int id)
    {
        base.Initialize(id);
        Canvas_Initialize();
        if ((UnityEngine.Random.Range(0.0f, 100.0f) <= mItemDropRate))
        {
            int count = UnityEngine.Random.Range(1, 3);
            mItems = new List<GameObject>();

            List<GameObject> allItems = new List<GameObject>();
            allItems.AddRange(GameManager.Instance.mWeaponPool);
            allItems.AddRange(GameManager.Instance.mArmorPool);
            for (int i = 0; i < count; ++i)
                mItems.Add(Instantiate(allItems[UnityEngine.Random.Range(0, allItems.Count)], transform));
        }
        else
            mMoney = UnityEngine.Random.Range(100, mMaximumGold);
        mInteraction.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.0f);
        isLock = (UnityEngine.Random.Range(0,100) <= 40) ? true : false;
    }

    public override IEnumerator Interact(Action action = null)
    {
        mAnimator = transform.Find("Object").GetComponent<Animator>();
        foreach (Dialogue dialogue in mDialogue)
        {
            m_DialogueQueue.Enqueue(dialogue);
        }
        UIManager.Instance.DisplayDialogueBox(true);
        while (m_DialogueQueue.Count > 0)
        {
            var dialogue = m_DialogueQueue.Dequeue();
            UIManager.Instance.ChangeDialogueText("Jimmy" + ": " + dialogue.Text);
            yield return new WaitForSeconds(0.5f);
            switch (dialogue.Trigger)
            {
                case Dialogue.TriggerType.None:
                    {
                        EnableIcon();
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                    }
                    break;
                case Dialogue.TriggerType.Event:
                    UIManager.Instance.DisplayEKeyInDialogue(false);
                    _DialogueComplete = false;
                    mTrigger = Event;
                    break;
                case Dialogue.TriggerType.Success:
                    {
                        EnableIcon();
                        if(mItems == null)
                            PlayerController.Instance.GetGold(mMoney);
                        else
                        {
                            for (int i = 0; i < mItems.Count; ++i)
                            {
                                GameObject item = Instantiate(mItems[i], PlayerController.Instance.transform.Find("Bag"));
                                item.GetComponent<Item>().isSold = true;
                                PlayerController.Instance.mInventory.Add(item.GetComponent<Item>());
                            }
                            for (int i = 0; i < mItems.Count; ++i)
                                Destroy(mItems[i]);
                            mItems.Clear();
                        }

                        mAnimator.SetBool("IsOpen", true);
                        AudioManager.PlaySfx(mSFX);
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                        _Completed = true;
                    }
                    break;
                case Dialogue.TriggerType.Trade:
                    break;
                case Dialogue.TriggerType.Fail:
                    {
                        EnableIcon();
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                    }
                    break;
            }
            yield return (mTrigger != null) ? StartCoroutine(mTrigger()) : null;
            yield return (mTrigger != null) ? new WaitUntil(() => _DialogueComplete) : null;
        }
        UIManager.Instance.ChangeDialogueText("");
        UIManager.Instance.DisplayDialogueBox(false);
        UIManager.Instance.DisplayEKeyInDialogue(false);
        action?.Invoke();
        if (_Completed)
            mAnimator.SetBool("IsOpen", false);
    }

    private void EnableIcon()
    {
        UIManager.Instance.DisplayButtonsInDialogue(false);
        UIManager.Instance.DisplayEKeyInDialogue(true);
    }

    public override void Reset()
    {
        _Completed = false;
        mAnimator = transform.Find("Object").GetComponent<Animator>();
    }

    public IEnumerator Event()
    {
        mTrigger = null;
        UIManager.Instance.ChangeTwoButtons(UIManager.Instance.mStorage.YesButtonImage,
    UIManager.Instance.mStorage.NoButtonImage);
        UIManager.Instance.AddListenerRightButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            _DialogueComplete = true;
        });
        if(IsLocked())
        {
            Key key = PlayerController.Instance.mInventory.Get("Key") as Key;
            if (key != null)
            {
                key.End();
                UnLock();
                UIManager.Instance.AddListenerLeftButton(() => {
                    foreach (var dialogue in m_DialogueYesCase)
                        m_DialogueQueue.Enqueue(dialogue);
                    _DialogueComplete = true;
                });
            }
            else
            {
                UIManager.Instance.AddListenerLeftButton(() => {
                    m_DialogueQueue.Enqueue(m_DialogueFailCase);
                    _DialogueComplete = true;
                });
            }
        }
        else
        {
            UIManager.Instance.AddListenerLeftButton(() => {
                foreach (var dialogue in m_DialogueYesCase)
                    m_DialogueQueue.Enqueue(dialogue);
                _DialogueComplete = true;
            });
        }

        UIManager.Instance.DisplayButtonsInDialogue(true);
        yield return new WaitUntil(() => _DialogueComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
    }

    public void UnLock()
    {
        isLock = true;
    }

    public bool IsLocked()
    {
        return isLock;
    }
}
