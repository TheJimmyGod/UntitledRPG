using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class BigHealthBar : HealthBar, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI mNameText;
    private TextMeshProUGUI mHPText;
    private TextMeshProUGUI mMPText;

    private Image mDamagedHealthBar;
    private Image mDamagedManaBar;
    private GameObject mTargetBar;
    public Player mTarget;
    public bool isTargetted = false;

    public float mNextHealth = 0.0f;

    private void Start()
    {
        mBorader = transform.GetComponent<Image>();
        mAnimator = transform.parent.GetComponent<Animator>();
        mBar = transform.Find("Health").GetComponent<Image>();
        mManaBar = transform.Find("ManaBorader").Find("Mana").GetComponent<Image>();
        mDamagedHealthBar = transform.Find("DamagedHealth").GetComponent<Image>();
        mDamagedManaBar = transform.Find("ManaBorader").Find("DamagedMana").GetComponent<Image>();
        mNameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        mHPText = transform.Find("HPValue").GetComponent<TextMeshProUGUI>();
        mMPText = transform.Find("MPValue").GetComponent<TextMeshProUGUI>();
        mTargetBar = transform.Find("Targetted").gameObject;
        mTargetBar.SetActive(false);
        Active(false);
    }

    public void Initialize(Player target)
    {
        mTarget = target;
        mNameText.text = mTarget.mSetting.Name;
        mCurrentHealth = mTarget.mStatus.mHealth;
        mMaxHealth = mTarget.mStatus.mMaxHealth;
        mCurrentMana = mTarget.mStatus.mMana;
        mMaxMana = mTarget.mStatus.mMaxMana;

        mBar.fillAmount = mCurrentHealth / mMaxHealth;
        mManaBar.fillAmount = mCurrentMana / mMaxMana;
        mHPText.text = mCurrentHealth.ToString();
        mMPText.text = mCurrentMana.ToString();
        isTargetted = false;
        isInitialized = true;
        Active(true);
    }

    protected override void Update()
    {
        if (isInitialized)
        {
            mTargetBar.SetActive(isTargetted);
            mBar.fillAmount = (mNextHealth > mCurrentHealth) ? Mathf.Lerp(mBar.fillAmount, mNextHealth / mMaxHealth, Time.deltaTime * 1.5f)
                : mCurrentHealth / mMaxHealth;
            if ((mNextHealth > mCurrentHealth) && mBar.fillAmount >= (mNextHealth / mMaxHealth) - 0.01f)
            {
                mCurrentHealth = mNextHealth;
                mNextHealth = 0.0f;
            }
            mManaBar.fillAmount = mCurrentMana / mMaxMana;
            mDamagedHealthBar.fillAmount = Mathf.Lerp(mDamagedHealthBar.fillAmount, mCurrentHealth / mMaxHealth,Time.deltaTime * 1.5f);
            mDamagedManaBar.fillAmount = Mathf.Lerp(mDamagedManaBar.fillAmount, mCurrentMana / mMaxMana,Time.deltaTime * 1.5f);
            mHPText.text = mCurrentHealth.ToString();
            mMPText.text = mCurrentMana.ToString();
        }
    }

    public override void Active(bool active)
    {
        base.Active(active);
        mDamagedHealthBar?.gameObject.SetActive(active);
        mNameText?.gameObject.SetActive(active);
        mHPText?.gameObject.SetActive(active);
        mMPText?.gameObject.SetActive(active);
        gameObject.SetActive(active);
    }

    public override IEnumerator PlayBleed()
    {
        mAnimator.SetTrigger("Wiggle");
        yield return new WaitForSeconds(0.51f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isInitialized && BattleManager.Instance.mSpellChanning)
        {
            isTargetted = true;
            Player unit = (Player)BattleManager.Instance.mCurrentUnit;
            unit.mTarget = mTarget;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isInitialized)
        {
            isTargetted = false;
            if(BattleManager.Instance.mCurrentUnit)
            {
                Player unit = (Player)BattleManager.Instance.mCurrentUnit;
                unit.mTarget = null;
            }
        }
    }
}
