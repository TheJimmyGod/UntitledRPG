using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    private Transform[] Boraders;
    private Transform mItemList;
    private UIManager _Manager;
    private Sprite _basicSprite;
    private bool isLevelUP;
    public void Initialize(UIManager manager)
    {
        Boraders = Array.FindAll(transform.Find("Panel").Find("Characters").GetComponentsInChildren<Transform>(), x => x.name == "Borader");
        _Manager = manager;
        mItemList = transform.Find("Panel").Find("ItemList");
        _basicSprite = Boraders[0].Find("Sprite").GetComponent<Image>().sprite;
        foreach (Transform t in Boraders)
            t.gameObject.SetActive(false);
        Active(false);
    }

    void LateUpdate()
    {
        for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
        {
            var fill = Boraders[i].Find("EXP").Find("Fill").GetComponent<Image>().fillAmount;
            var hero = PlayerController.Instance.mHeroes[i].GetComponent<Unit>().mStatus;
            Boraders[i].Find("EXP").Find("Fill").GetComponent<Image>().fillAmount = Mathf.Lerp(fill, (float)(hero.mEXP) / (float)(GameManager.Instance.mRequiredEXP + (50 * hero.mLevel)), Time.deltaTime * 1.5f);
        }
    }

    public void Active(bool active)
    {
        gameObject.SetActive(active);
        if (active)
        {
            UpdateCharacterList();
            StartCoroutine(_Manager.Celebration(isLevelUP));
        }
        else
        {
            foreach (Transform t in mItemList.Find("ItemScroll").Find("Items"))
                Destroy(t.gameObject);
        }
        if (PlayerController.Instance != null)
        {
            for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
            {
                Boraders[i].gameObject.SetActive(active);
            }
        }
    }

    private void UpdateCharacterList()
    {
        isLevelUP = false;
        bool LevelEvent = false;

        BonusStatus[] bonusStatuses = new BonusStatus[PlayerController.Instance.mHeroes.Count];
        bool[] islevelUpGroup = new bool[PlayerController.Instance.mHeroes.Count];
        do
        {
            LevelEvent = false;
            for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
                LevelEvent = CheckLevelUp(ref bonusStatuses, ref islevelUpGroup, i);
        }
        while (LevelEvent);

        for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
        {
            var hero = PlayerController.Instance.mHeroes[i].GetComponent<Unit>();
            var borader = Boraders[i].transform;
            PrintOutHeroStatus(bonusStatuses, islevelUpGroup, i, hero, borader);
        }
    }

    private void PrintOutHeroStatus(BonusStatus[] bonusStatuses, bool[] islevelUpGroup, int i, Unit hero, Transform borader)
    {
        borader.Find("Status").gameObject.SetActive(islevelUpGroup[i]);
        if (islevelUpGroup[i])
        {
            borader.Find("Status").Find("Health").Find("Value").GetComponent<TextMeshProUGUI>().text = "+ " + bonusStatuses[i].mHealth.ToString();
            borader.Find("Status").Find("Mana").Find("Value").GetComponent<TextMeshProUGUI>().text = "+ " + bonusStatuses[i].mMana.ToString();
            borader.Find("Status").Find("Sub_First").Find("Value").GetComponent<TextMeshProUGUI>().text = "+ " + bonusStatuses[i].mDamage.ToString();
            borader.Find("Status").Find("Sub_Second").Find("Value").GetComponent<TextMeshProUGUI>().text = "+ " + bonusStatuses[i].mMagicPower.ToString();
            borader.Find("Status").Find("Sub_Third").Find("Value").GetComponent<TextMeshProUGUI>().text = "+ " + bonusStatuses[i].mArmor.ToString();
            borader.Find("Status").Find("Sub_Forth").Find("Value").GetComponent<TextMeshProUGUI>().text = "+ " + bonusStatuses[i].mMagic_Resistance.ToString();
        }
        borader.Find("Name").GetComponent<TextMeshProUGUI>().text = hero.mSetting.Name;
        borader.Find("Level").GetComponent<TextMeshProUGUI>().text = "Lv. " + hero.mStatus.mLevel.ToString();
        borader.Find("EXPValue").GetComponent<TextMeshProUGUI>().text = (islevelUpGroup[i]) ? "Level up!" : hero.mStatus.mEXP.ToString() + " / " + (GameManager.Instance.mRequiredEXP + (50 * hero.mStatus.mLevel));
        borader.Find("Sprite").GetComponent<Image>().sprite = Resources.Load<Image>("Prefabs/UI/" + hero.mSetting.Name + "_UI").sprite;
        borader.Find("Sprite").GetComponent<Animator>().runtimeAnimatorController = Resources.Load<Animator>("Prefabs/UI/" + hero.mSetting.Name + "_UI").runtimeAnimatorController;
    }

    private bool CheckLevelUp(ref BonusStatus[] bonusStatuses, ref bool[] islevelUpGroup, int i)
    {
        var hero = PlayerController.Instance.mHeroes[i].GetComponent<Unit>();
        var levelUp = hero.LevelUP();
        if (levelUp.Key)
        {
            islevelUpGroup[i] = levelUp.Key;
            bonusStatuses[i].mHealth += levelUp.Value.mHealth;
            bonusStatuses[i].mArmor += levelUp.Value.mArmor;
            bonusStatuses[i].mDamage += levelUp.Value.mDamage;
            bonusStatuses[i].mMagicPower += levelUp.Value.mMagicPower;
            bonusStatuses[i].mMagic_Resistance += levelUp.Value.mMagic_Resistance;
            bonusStatuses[i].mMana += levelUp.Value.mMana;
            isLevelUP = true;
        }

        return levelUp.Key;
    }

    public IEnumerator WaitForEnd()
    {
        GetComponent<Animator>().Play("Outro");
        yield return new WaitForSeconds(1.0f);
        Active(false);
    }

    public void UpdateItemList(List<GameObject> ItemList)
    {
        mItemList.Find("Gold").GetComponent<TextMeshProUGUI>().text = GameManager.s_TotalGold.ToString();
        mItemList.Find("Soul").GetComponent<TextMeshProUGUI>().text = GameManager.s_TotalSoul.ToString();
        GameObject prefab = (Resources.Load<GameObject>("Prefabs/UI/ObtainedItem"));
        foreach (var item in ItemList)
        {
            GameObject slot = Instantiate(prefab, mItemList.transform.position, Quaternion.identity, mItemList.Find("ItemScroll").Find("Items"));
            slot.transform.Find("Sprite").GetComponent<Image>().sprite = item.GetComponent<Item>().Info.mSprite;
            slot.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = item.GetComponent<Item>().Info.mName;
        }
    }
}
