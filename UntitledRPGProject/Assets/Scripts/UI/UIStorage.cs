using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIStorage : MonoBehaviour
{
    [TextArea]
    public string mTextForTarget;
    [TextArea]
    public string mTextForAccpet;
    [TextArea]
    public string mTextForGameOver = "Wasted!";

    public GameObject mSkillDescription;
    public GameObject mAttackDescription;
    public GameObject mDefendDescription;

    public Sprite YesButtonImage;
    public Sprite NoButtonImage;
    public Sprite BuyButtonImage;
    public Sprite SellButtonImage;
    public Image ExitButtonImage;
    public Image EKeyImage;

    public Button mLeftButton;
    public Button mRightButton;
    public Button mExitButton;
    public Button mBackButton;
    public GameObject mBattleUI;
    public GameObject mBasicText;
    public GameObject mDialogueBox;
    public GameObject mRequireMoneyBox;
    public GameObject mFadeScreen;
    public GameObject mOrderbar;
    public GameObject mScreenTransition;
    public GameObject mVictoryScreenTransition;
    public GameObject mMinimap;

    public GameObject mTutorialMouseIcon;
    public GameObject mTutorialWASDIcon;
    public GameObject mTutorialInteractIcon;
    public GameObject mTutorialItemIcon;
    public GameObject mTutorialHereIcon;

    public GameObject mCurrentMoney;

    public TextMeshProUGUI mDialogueText;
    public void Initialize()
    {
        mLeftButton = mDialogueBox.transform.Find("YesButton").GetComponent<Button>();
        mRightButton = mDialogueBox.transform.Find("NoButton").GetComponent<Button>();
        mExitButton = mDialogueBox.transform.Find("ExitButton").GetComponent<Button>();
        mRequireMoneyBox = mDialogueBox.transform.Find("CompanionBox").gameObject;
        mDialogueText = mDialogueBox.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        EKeyImage = mDialogueBox.transform.Find("E_key").GetComponent<Image>();
        mMinimap = UIManager.Instance.mCanvas.transform.Find("MinimapWindow").gameObject;
        mLeftButton.onClick.RemoveAllListeners();
        mRightButton.onClick.RemoveAllListeners();
        mOrderbar.GetComponent<OrderBar>().Initialize();
        mScreenTransition = UIManager.Instance.mCanvas.transform.Find("ScreenTransition").gameObject;
        mCurrentMoney = UIManager.Instance.mCanvas.transform.Find("MyMoney").gameObject;
        mVictoryScreenTransition = UIManager.Instance.mCanvas.transform.Find("ScreenTransitionVictory").gameObject;
        mFadeScreen.SetActive(false);
        mScreenTransition.SetActive(false);
        mVictoryScreenTransition.SetActive(false);
        mCurrentMoney.SetActive(false);
        mRequireMoneyBox.SetActive(false);
    }

    private void Update()
    {
        if(PlayerController.Instance && mCurrentMoney.activeSelf)
        {
            mCurrentMoney.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = PlayerController.Instance.mGold.ToString();
        }
    }
}
