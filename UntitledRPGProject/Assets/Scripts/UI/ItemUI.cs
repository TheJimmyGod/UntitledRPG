using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public enum ItemUIType
    {
        Equip,
        Unequip,
        Sell,
        Buy,
        Non_Activate
    }

    public Button mButton;
    public Image mSprite;
    private int ID;
    private Item mItem;
    private ItemUIType mType;
    
    public void Initialize(int ID, Item it, ItemUIType type)
    {
        mItem = it;
        mType = type;
        mButton = GetComponent<Button>();
        mButton.onClick.RemoveAllListeners();
        switch (mType)
        {
            case ItemUIType.Equip:
                {
                    mSprite = transform.Find("ItemSprite").GetComponent<Image>();
                    mSprite.sprite = mItem.Info.mSprite;
                    GetComponent<HoverTip>().mTipToShow = "<color=yellow>" + mItem.Name + "</color>";
                    Debug.Log("Here");
                    if (typeof(EquipmentInfo).IsAssignableFrom(mItem.Info.GetType()))
                    {
                        var item = (EquipmentInfo)mItem.Info;

                        if(typeof(WeaponInfo).IsAssignableFrom(item.GetType()))
                        {
                            var weapon = (WeaponInfo)mItem.Info;
                            string unitName = string.Empty;
                            switch(weapon.mWeaponType)
                            {
                                case WeaponType.Soul:
                                    unitName = "Jimmy";
                                    break;
                                case WeaponType.Bow:
                                    unitName = "Eleven";
                                    break;
                                case WeaponType.Orb:
                                    unitName = "Victor";
                                    break;
                                case WeaponType.Spear:
                                    unitName = "Roger";
                                    break;
                                case WeaponType.Double_Swords:
                                    unitName = "Vin";
                                    break;
                                default:
                                    break;
                            }
                            GetComponent<HoverTip>().mTipToShow += $"\n Vaild Units: <color=yellow>{unitName}</color>\n";
                        }
                        foreach (var i in item.mBonusAbilities)
                        {
                            if (i.Type == BonusAbility.AbilityType.Magic)
                                GetComponent<HoverTip>().mTipToShow += "\n This item can use " + i.Type.ToString();
                            else
                                GetComponent<HoverTip>().mTipToShow += "\n<color=green>" + i.Type.ToString() + "</color>: " + i.Value;
                        }
                    }
                    else
                    {
                        var item = (ExpendablesInfo)mItem.Info;
                        GetComponent<HoverTip>().mTipToShow = "<color=yellow>" + mItem.Name + $"</color> ({item.mAmount})";
                    }
                    ID = mItem.ID;

                    mButton.onClick.AddListener(() => Activate());
                }

                break;
            case ItemUIType.Unequip:
                {
                    mSprite = GetComponent<Image>();
                    GetComponent<HoverTip>().mTipToShow = "<color=yellow>" + mItem.Name + "</color>";
                    if (typeof(EquipmentInfo).IsAssignableFrom(mItem.Info.GetType()))
                    {
                        var item = (EquipmentInfo)mItem.Info;
                        foreach (var i in item.mBonusAbilities)
                        {
                            if (i.Type == BonusAbility.AbilityType.Magic)
                                GetComponent<HoverTip>().mTipToShow += "\n This item can use " + i.Type.ToString();
                            else
                                GetComponent<HoverTip>().mTipToShow += "\n<color=green>" + i.Type.ToString() + "</color>: " + i.Value;
                        }
                    }
                    ID = mItem.ID;
                    mButton.onClick.AddListener(() => Inactivate());
                }
                break;
            case ItemUIType.Sell:
                {
                    mSprite = transform.Find("ItemSprite").GetComponent<Image>();
                    mSprite.sprite = mItem.Info.mSprite;
                    if (mItem.GetType().IsSubclassOf(typeof(Expendables)))
                    {
                        ExpendablesInfo info = (ExpendablesInfo)mItem.Info;
                    }
                    ID = mItem.ID;
                    mButton.onClick.AddListener(() => Sell());
                }
                break;
            case ItemUIType.Buy:
                {
                    mButton.onClick.AddListener(() => Buy());
                }
                break;
            case ItemUIType.Non_Activate:
                {
                    mSprite = transform.Find("ItemSprite").GetComponent<Image>();
                    mSprite.sprite = mItem.Info.mSprite;
                    if(mItem.GetType().IsSubclassOf(typeof(Expendables)))
                    {
                        var item = (ExpendablesInfo)mItem.Info;
                        GetComponent<HoverTip>().mTipToShow = "<color=yellow>" + mItem.Name + $"</color> ({item.mAmount})";
                    }
                    else
                        GetComponent<HoverTip>().mTipToShow = "<color=yellow>" + mItem.Name + "</color>";
                }
                break;
            default:
                break;
        }


    }

    public void Activate()
    {
        int index = UIManager.Instance.mInventoryUI.mIndex;
        var unit = PlayerController.Instance.mHeroes[index].GetComponent<InventroySystem>();
        var item = (EquipmentItem)mItem;
        AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mItemEquipSFX);
        if (unit.Equip(item))
        {
            Debug.Log("Hi");
            if (mItem.GetType().IsAssignableFrom(typeof(EquipmentItem)))
                gameObject.SetActive(false);
        }
    }

    public void Inactivate()
    {
        int index = UIManager.Instance.mInventoryUI.mIndex;
        var unit = PlayerController.Instance.mHeroes[index].GetComponent<InventroySystem>();
        switch (name)
        {
            case "Arm":
                unit.UnEquip(unit.mInventoryInfo.Arm);
                break;
            case "Body":
                unit.UnEquip(unit.mInventoryInfo.Body);
                break;
            case "Head":
                unit.UnEquip(unit.mInventoryInfo.Head);
                break;
            case "Leg":
                unit.UnEquip(unit.mInventoryInfo.Leg);
                break;
            case "Weapon":
                unit.UnEquip(unit.mInventoryInfo.Weapon);
                break;
        }

        mSprite.sprite = UIManager.Instance.mInventoryUI.mEmptyImage;
        mItem = null;
        UIManager.Instance.mInventoryUI.InventoryUpdate();
    }

    public void Sell()
    {
        AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mItemPurchaseSFX);
        if (mItem.GetType().IsSubclassOf(typeof(Expendables)))
        {
            Expendables expendables = (Expendables)mItem;
            mItem.isSold = false;
            PlayerController.Instance.mInventory.Remove(mItem);
            mItem.gameObject.transform.SetParent(UIManager.Instance.mMerchantScreen.mSoldItemsGroup.Find("Objects"));
            PlayerController.Instance.mGold += mItem.Value * expendables.Amount;
            UIManager.Instance.mMerchantScreen.EnqueueSoldItem(ID, mItem, gameObject);
        }
        else
        {
            mItem.isSold = false;
            PlayerController.Instance.mGold += mItem.Value;
            PlayerController.Instance.mInventory.Remove(mItem);
            mItem.gameObject.transform.SetParent(UIManager.Instance.mMerchantScreen.mSoldItemsGroup.Find("Objects"));
            UIManager.Instance.mMerchantScreen.EnqueueSoldItem(ID, mItem, gameObject);
        }
        UIManager.Instance.mInventoryUI.InventoryUpdate();

    }

    public void Buy()
    {
        if (mItem.GetType().IsSubclassOf(typeof(Expendables)))
        {
            Expendables expendables = (Expendables)mItem;
            if (PlayerController.Instance.mGold >= mItem.Value * expendables.Amount)
            {
                AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mItemPurchaseSFX);
                PlayerController.Instance.mGold -= mItem.Value * expendables.Amount;
                mItem.gameObject.transform.SetParent(PlayerController.Instance.transform.Find("Bag"));
                PlayerController.Instance.mInventory.Add(mItem.gameObject.GetComponent<Item>());
                mItem.isSold = true;
                UIManager.Instance.mMerchantScreen.EnqueueItem(ID, mItem, gameObject);
            }
        }
        else
        {
            if (PlayerController.Instance.mGold >= mItem.Value)
            {
                AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mItemPurchaseSFX);

                PlayerController.Instance.mGold -= mItem.Value;
                mItem.gameObject.transform.SetParent(PlayerController.Instance.transform.Find("Bag"));
                PlayerController.Instance.mInventory.Add(mItem.gameObject.GetComponent<Item>());
                mItem.isSold = true;
                UIManager.Instance.mMerchantScreen.EnqueueItem(ID, mItem, gameObject);
            }
        }

    }
}
