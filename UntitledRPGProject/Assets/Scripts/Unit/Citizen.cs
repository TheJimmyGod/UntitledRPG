using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : NPC
{
    [SerializeField]
    protected Dialogue m_DialogueResultCase;

    private int mMoney = 0;
    private bool isEvent = false;

    [SerializeField]
    private float mRateToTurnEnemy = 30.0f;

    [SerializeField]
    private string[] names = new string[50]
    {
            "Amy","Billy","Carry", "Danny", "Ebi",
            "Fury","Gon", "Hugo", "Ivony", "Jenny",
            "Ken", "Leon", "Mol", "Nani", "Oppa",
            "Peon", "Quan", "Robert", "Steven", "Uven",
            "Vanny", "Wrath", "Xath", "Yammy", "Zen",
            "Aqua", "Bean", "Curry", "Don", "Eric",
            "Frick", "Grak", "Han", "Itch", "Jhon",
            "Khan", "Leona", "Mint Choco", "Noob","Oasis",
            "Penny", "Queen", "Reni", "Seth", "Uza",
            "Vill", "Wendy", "Xens", "York", "Zero"
    };
    [SerializeField]
    private string[] eventContexts = new string[25]
        {
            "Help, Help! I need you help!",
            "I need your help, please!!",
            "...Can you help?",
            "Ehhhhhhhhhhhhhh~~!!",
            "Hehehe I am drunk~",
            "Kyaaaaaaaa~~~!!",
            "*Sigh...",
            "HELP ME!!",
            "I don't want to die!!",
            "Grrrrrrrrrr...",
            "Oh Noooooooooo!!!!",
            "Please please help me!!",
            "Can you play with me?",
            "You are a handsome boy.",
            "Sorry, can you help me?",
            "Support me!",
            "I am your fan!! Sign it please!",
            "Oh my god! It's you, my friend!",
            "The world shall be ended...",
            "Hahahahahahahahaha...",
            "I like you.",
            "I love you.",
            "I hate you.",
            "Hey, dude!",
            "I beg you! Help me!"
        };
    [SerializeField]
    private string[] normalContexts = new string[25]
        {
            "Greetings!",
                "Hello, boy!",
                "I have never seen a person like you.",
                "Are you our savior? Ah...",
                "I have not eaten anything... because the <color=red>king</color> stole my ketchup!",
                "Please kick the <color=red>king</color>'s ass!",
                "I am sorry. I cannot help you.",
                "We are powerless...",
                "Please save the world!",
                "You look delicious.",
                "Nice boy!",
                "Oh my god, our good friend!",
                "Have a nice hell!",
                "We can do nothing!!",
                "Grrrrrrrrrrr... Oh, I just get hungry",
                "Hi, boy!",
                "The <color=red>king</color> has done brutal and horrible things on our town!",
                "Noooooo, We are useless!",
                "Play fun with me!",
                "Gotcha! You are a hero, right?",
                "This town is boring, and you look boring.",
                "I want to invite you my home!",
                "What the hell?",
                "The <color=red>king</color> dominates our town.",
                "This is the end of the world"
        };

    protected override void Start()
    {
        base.Start();
        isEvent = UnityEngine.Random.Range(0, 100) <= 30.0f ? true : false;

        mName = names[UnityEngine.Random.Range(0, names.Length)];
        mMoney = UnityEngine.Random.Range(5, 100);
        if(isEvent)
        {
            m_DialogueList.Clear();
            m_DialogueList.Add(new Dialogue($"{eventContexts[UnityEngine.Random.Range(0, eventContexts.Length)]} \n (Do you want to help them?)", Dialogue.TriggerType.Event));
            if (UnityEngine.Random.Range(0, 100) <= mRateToTurnEnemy)
            {
                mProperty = Resources.Load<EnemyTrap>("Prefabs/Items/EnemyTrap");

                if (UnityEngine.Random.Range(0, 100) <= 50)
                {
                    m_DialogueYesCase.Add(new Dialogue("Hehehe~ I would eat you all~", Dialogue.TriggerType.Fail));
                    m_DialogueNoCase.Add(new Dialogue($"(You have stolen <color=yellow>{mMoney}</color>)", Dialogue.TriggerType.Success));
                }
                else
                {
                    m_DialogueNoCase.Add(new Dialogue("Hehehe~ I would eat you all~", Dialogue.TriggerType.Fail));
                    m_DialogueYesCase.Add(new Dialogue($"(You have stolen <color=yellow>{mMoney}</color>)", Dialogue.TriggerType.Success));
                }

            }
            else
            {
                GameObject item = (UnityEngine.Random.Range(0, 100) <= 50) ? Instantiate((UnityEngine.Random.Range(0, 100) <= 50) ?
                    GameManager.Instance.mArmorPool[UnityEngine.Random.Range(0, GameManager.Instance.mArmorPool.Length)] :
                    GameManager.Instance.mWeaponPool[UnityEngine.Random.Range(0, GameManager.Instance.mWeaponPool.Length)], transform) : null;
                if (item != null)
                {
                    mProperty = item.GetComponent<Item>();
                    mProperty.isSold = true;
                }

                string what = ((item == null) ? "Gold: " + mMoney.ToString() : item.GetComponent<Item>().Name);
                m_DialogueNoCase.Add(new Dialogue("Aww.. Okay", Dialogue.TriggerType.Fail));
                m_DialogueYesCase.Add(new Dialogue($"Hey, hero! This is for you! \n (You have obtained <color=yellow>{what}</color>)", Dialogue.TriggerType.Success));
            }
        }
        else
        {
            m_DialogueList.Clear();
            m_DialogueList.Add(new Dialogue($"{normalContexts[UnityEngine.Random.Range(0, normalContexts.Length)]}", Dialogue.TriggerType.None));
        }

        if (mProperty != null)
            mProperty.Initialize(-1);
    }

    public override IEnumerator Interact(Action Callback)
    {
        foreach (Dialogue dialogue in m_DialogueList)
        {
            m_DialogueQueue.Enqueue(dialogue);
        }
        UIManager.Instance.FadeInScreen();
        UIManager.Instance.DisplayDialogueBox(true);
        while (m_DialogueQueue.Count > 0)
        {
            var dialogue = m_DialogueQueue.Dequeue();
            UIManager.Instance.ChangeDialogueText(mName + ": " + dialogue.Text);
            yield return new WaitForSeconds(0.5f);
            switch (dialogue.Trigger)
            {
                case Dialogue.TriggerType.None:
                    IconEnable();
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                    break;
                case Dialogue.TriggerType.Event:
                    mComplete = false;
                    mTrigger = Event;
                    break;
                case Dialogue.TriggerType.Fail:
                    {
                        IconEnable();
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                        if(mProperty.GetType().IsAssignableFrom(typeof(EnemyTrap)))
                        {
                            AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mItemTrapSFX);
                            EnemyTrap trap = (EnemyTrap)mProperty;
                            trap.mTransform = transform;
                            trap.Apply();
                            Callback += () => { Destroy(gameObject,2.0f); };
                        }
                        else
                            ResultDialogue(". . .");
                    }
                    break;
                case Dialogue.TriggerType.Success:
                    {
                        // TODO: Quest completed
                        IconEnable();
                        if(mProperty)
                        {
                            if (mProperty.GetType().IsAssignableFrom(typeof(EnemyTrap)))
                            {
                                PlayerController.Instance.GetGold(mProperty.Value);
                                ResultDialogue("??? Where is my money??");
                            }
                            else
                            {
                                GameObject newItem = new GameObject(mProperty.Name);
                                newItem.AddComponent<EquipmentItem>();
                                newItem.GetComponent<EquipmentItem>().Info = mProperty.Info;
                                newItem.GetComponent<EquipmentItem>().Initialize(mProperty.ID);
                                PlayerController.Instance.mInventory.Add(newItem.GetComponent<Item>());
                                newItem.transform.SetParent(PlayerController.Instance.mBag.transform);
                                ResultDialogue("Greetings, hero!");
                            }
                        }
                        else
                        {
                            PlayerController.Instance.GetGold(mMoney);
                            ResultDialogue("Greetings, my hero!");
                        }
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                    }
                    break;
                default:
                    break;
            }
            yield return (mTrigger != null) ? StartCoroutine(mTrigger()) : null;
            yield return (mTrigger != null) ? new WaitUntil(() => mComplete) : null;
        }
        UIManager.Instance.FadeOutScreen();
        UIManager.Instance.ChangeDialogueText("");
        UIManager.Instance.DisplayDialogueBox(false);
        UIManager.Instance.DisplayEKeyInDialogue(false);
        Callback?.Invoke();
        mComplete = false;
        mTrigger = null;
    }

    private void ResultDialogue(string text)
    {
        m_DialogueList.Clear();
        m_DialogueResultCase = new Dialogue(text, Dialogue.TriggerType.None);
        m_DialogueList.Add(m_DialogueResultCase);
    }

    private void IconEnable()
    {
        UIManager.Instance.DisplayButtonsInDialogue(false);
        UIManager.Instance.DisplayEKeyInDialogue(true);
    }

    public override IEnumerator Event()
    {
        mTrigger = null;
        UIManager.Instance.ChangeTwoButtons(UIManager.Instance.mStorage.YesButtonImage,
    UIManager.Instance.mStorage.NoButtonImage);
        UIManager.Instance.AddListenerRightButton(() => {
            foreach (var dialogue in m_DialogueNoCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        UIManager.Instance.AddListenerLeftButton(() => {
            foreach (var dialogue in m_DialogueYesCase)
                m_DialogueQueue.Enqueue(dialogue);
            mComplete = true;
        });
        UIManager.Instance.DisplayButtonsInDialogue(true);
        UIManager.Instance.DisplayEKeyInDialogue(false);
        yield return new WaitUntil(() => mComplete);
        UIManager.Instance.DisplayButtonsInDialogue(false);
        //TODO: Quest?
    }
}
