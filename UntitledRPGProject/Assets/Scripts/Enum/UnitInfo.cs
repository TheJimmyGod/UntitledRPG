using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIProperty
{
    Offensive,
    Defensive
}

public enum AIType
{
    None,
    Manual,
    Auto
}

public enum AITargetPriority
{
    None,
    AimToHighHealth
}

public enum ActionEvent
{
    None,
    IntroWalk,
    AttackWalk,
    MagicWalk,
    Busy,
    BackWalk,
    DodgeWalk,
    DodgeBack
}

public struct AIBuild
{
    public StateMachine stateMachine;
    public AIProperty property;
    public AITargetPriority priority;
    public AIType type;
    public ActionEvent actionEvent;

    public void SetActionEvent(ActionEvent action)
    {
        actionEvent = action;
    }

    public void ChangeState(string stateName)
    {
        stateMachine.ChangeState(stateName);
    }

    public void Update(bool isAI)
    {
        if(isAI)
            stateMachine.ActivateState();
    }
}

public struct Status
{
    public int mLevel;
    public int mEXP;
    public int mGold;
    public float mMaxHealth;
    public float mHealth;
    public float mMana;
    public float mMaxMana;
    public float mDamage;
    public float mArmor;
    public float mMagic_Resistance;
    public float mDefend;
    public float mAgility;
    public float mMagicPower;
    public WeaponType mWeaponType;
    public Status(int level, int exp, int gold, float maxHp, float hp, float mana, float maxMp, float dmg, float am, float mr, float de, float ag, float mp, WeaponType wp = WeaponType.None)
    {
        mLevel = level;
        mEXP = exp;
        mGold = gold;
        mMaxHealth = maxHp;
        mHealth = hp;
        mMana = mana;
        mMaxMana = maxMp;
        mDamage = dmg;
        mArmor = am;
        mMagic_Resistance = mr;
        mDefend = de;
        mAgility = ag;
        mMagicPower = mp;
        mWeaponType = wp;
    }
}
public struct BonusStatus
{
    public float mHealth;
    public float mMana;
    public float mDamage;
    public float mArmor;
    public float mMagic_Resistance;
    public float mDefend;
    public float mAgility;
    public float mMagicPower;
}

public struct SkillTreeBonus
{
    public float mArmor;
    public float mDamage;
    public float mHealth;
    public float mMana;
    
    public float mHPRegeneration;
    public float mMPRegeneration;
    public float mShieldValue;

    public GameObject mShield;

    public bool IsHPRegeneration;
    public bool IsMPRegeneration;
    public bool IsShield;
}

public struct Conditions
{
    public bool isDied;
    public bool isDefend;
    public bool isCancel;
    public bool isBattleComplete;

    public Conditions(bool die, bool de, bool can, bool bat)
    {
        isDied = die;
        isDefend = de;
        isCancel = can;
        isBattleComplete = bat;
    }
}

public enum Order
{
    Standby,
    TurnEnd
}

public enum Flag
{
    Player,
    Neutral,
    Enemy
}

[Serializable]
public class CharacterExist
{
    public NPCUnit mUnit = NPCUnit.None;
    public bool isExist = false;
    public CharacterExist(NPCUnit mUnit, bool isExist)
    {
        this.mUnit = mUnit;
        this.isExist = isExist;
    }
}