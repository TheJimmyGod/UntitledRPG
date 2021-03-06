using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/TargetAbility")]
public class TargetAbility : DamagableAbility
{
    private Unit mTarget;
    private GameObject mProjectile;
    public SKillShootType mShootType = SKillShootType.Melee;
    [SerializeField]
    private float mRange = 4.0f;
    [SerializeField]
    private Vector2 mStartPosition;

    public override void Activate(MonoBehaviour parent)
    {
        isActive = false;
        parent.StopAllCoroutines();
        mOwner = parent.transform.GetComponent<Unit>();
        parent.StartCoroutine(WaitforDecision());
    }

    public override IEnumerator WaitforDecision()
    {
        if(mOwner.mStatus.mMana < mManaCost)
            BattleManager.Instance.Cancel();
        else
        {
            if (mOwner.mAiBuild.type == AIType.Manual)
            {
                UIManager.Instance.ChangeOrderBarText(UIManager.Instance.mStorage.mTextForTarget);
                mTarget = null;

                foreach (GameObject unit in (mProperty == SkillProperty.Friendly) ? PlayerController.Instance.mHeroes : BattleManager.Instance.mEnemies)
                {
                    if (!unit.GetComponent<Unit>().mConditions.isDied)
                        unit.GetComponent<Unit>().mCanvas.transform.Find("Arrow").gameObject.SetActive(true);
                }
                while (true)
                {
                    Raycasting();
                    if(Input.GetMouseButtonDown(0) && mTarget)
                    {
                        isActive = true;
                        break;
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        mTarget?.mField.TargetedMagicHostile(false);
                        mTarget?.mField.TargetedFriendly(false);
                        mTarget = null;
                        isActive = false;
                        BattleManager.Instance.mSpellChanning = false;
                        UIManager.Instance.ChangeOrderBarText("Waiting for Order...");
                        break;
                    }
                    yield return null;
                }

                foreach (GameObject unit in (mProperty == SkillProperty.Friendly) ? PlayerController.Instance.mHeroes : BattleManager.Instance.mEnemies)
                {
                    unit.GetComponent<Unit>().mCanvas.transform.Find("Arrow").gameObject.SetActive(false);
                }

                UIManager.Instance.ChangeOrderBarText(UIManager.Instance.mStorage.mTextForAccpet);
            }
            else
            {
                isActive = true;
                mTarget = mOwner.mTarget;
            }

            if (isActive)
            {
                BattleManager.Instance.mSpellChanning = false;
                mTarget.mField.TargetedMagicHostile(false);
                mTarget.mField.TargetedFriendly(false);
                if(mTarget.GetType() == typeof(Player) && mProperty == SkillProperty.Friendly)
                {
                    Player playerunit = (Player)mTarget;
                    playerunit.mMyHealthBar.isTargetted = false;
                }

                UIManager.Instance.ChangeOrderBarText("<color=red>"+ mName + "!</color>");
                mOwner.mTarget = mTarget;
                mTarget?.mSelected.SetActive(false);
                bool hasState = mOwner.mAnimator.HasState(0, Animator.StringToHash(mAnimationName));
                mOwner.mMagicDistance = mRange;
                mOwner.mAiBuild.SetActionEvent(ActionEvent.MagicWalk);
                if(mProperty == SkillProperty.Friendly)
                    CameraSwitcher.Instance.StartCoroutine(CameraSwitcher.Instance.ZoomCamera(mEffectTime / 2.0f, Vector3.Lerp(mOwner.transform.position, mTarget.transform.position, 0.5f)));
                yield return new WaitUntil(() => mOwner.mAiBuild.actionEvent == ActionEvent.Busy);
                mOwner.mStatus.mMana -= mManaCost;

                if (mShootType == SKillShootType.Range)
                {
                    mOwner.mirror?.Play((hasState) ? mAnimationName : "Attack");
                    mOwner.mAnimator.Play((hasState) ? mAnimationName : "Attack");
                    yield return new WaitForSeconds(mEffectTime);
                    if(mOwner.mSkillClips.Count > 0)
                        AudioManager.PlaySfx(mOwner.mSkillClips[UnityEngine.Random.Range(0, mOwner.mSkillClips.Count - 1)].Clip, 1.0f);
                    Shoot();
                    yield return new WaitUntil(() => mProjectile.GetComponent<Projectile>().isCollide == true);
                }
                else if (mShootType == SKillShootType.Melee)
                {
                    yield return new WaitForSeconds(mEffectTime);
                    mOwner.mirror?.Play((hasState) ? mAnimationName : "Attack");
                    if (mActionTrigger != null)
                    {
                        mActionTrigger.Invoke();
                        yield return new WaitUntil(()=> mOwner.GetComponent<ActionTrigger>().isCompleted);
                    }
                    else
                    {
                        mOwner.mAnimator.Play((hasState) ? mAnimationName : "Attack");
                        yield return new WaitForSeconds(mOwner.mAnimator.GetCurrentAnimatorStateInfo(0).length + mEffectTime);
                        if (mOwner.mSkillClips.Count > 0)
                            AudioManager.PlaySfx(mOwner.mSkillClips[UnityEngine.Random.Range(0, mOwner.mSkillClips.Count - 1)].Clip, 1.0f);
                        CommonState();
                    }
                    yield return new WaitForSeconds(0.2f);
                }
                else if(mShootType == SKillShootType.Instant)
                {
                    mOwner.mirror?.Play((hasState) ? mAnimationName : "Attack");
                    mOwner.mAnimator.Play((hasState) ? mAnimationName : "Attack");
                    if (mOwner.mSkillClips.Count > 0)
                        AudioManager.PlaySfx(mOwner.mSkillClips[UnityEngine.Random.Range(0, mOwner.mSkillClips.Count - 1)].Clip, 1.0f);
                    yield return new WaitForSeconds(mEffectTime);
                    CommonState();
                }

                GameObject effect = Resources.Load<GameObject>("Prefabs/Effects/" + mName + "_Effect");
                if (effect != null)
                {
                    GameObject go = Instantiate(effect
    , mTarget.transform.position + effect.transform.position,Quaternion.Euler(effect.transform.eulerAngles));
                    Destroy(go, 1.1f);
                }
                yield return new WaitForSeconds(0.5f);
                mOwner.mAiBuild.SetActionEvent(ActionEvent.BackWalk);
            }
            else
                BattleManager.Instance.Cancel();
        }
        if(mOwner.mAiBuild.actionEvent == ActionEvent.BackWalk)
        {
            yield return new WaitUntil(() => mOwner.mAiBuild.actionEvent == ActionEvent.Busy);
        }
        isComplete = true;
        yield return null;
    }

    private void CommonState()
    {
        float newValue = mValue + mOwner.mStatus.mMagicPower + mOwner.mBonusStatus.mMagicPower;
        bool isHit = true;
        switch (mSkillType)
        {
            case SkillType.Attack:
                {
                    isHit = mTarget.TakeDamage(newValue, DamageType.Magical);
                    foreach (var buff in mBuffList)
                        mOwner.SetBuff(buff.Initialize(mOwner, mOwner));
                    if (isHit)
                    {
                        foreach (var nerf in mNerfList)
                            mTarget.SetNerf(nerf.Initialize(mOwner, mTarget));
                    }

                }
                break;
            case SkillType.Buff:
                foreach (var buff in mBuffList)
                    mOwner.SetBuff(buff.Initialize(mOwner, mTarget));
                break;
            case SkillType.BuffNerf:
                {
                    foreach (var buff in mBuffList)
                        mTarget.SetBuff(buff.Initialize(mOwner, mTarget));
                    foreach (var nerf in mNerfList)
                        mTarget.SetNerf(nerf.Initialize(mOwner, mTarget));
                }
                break;
            case SkillType.Nerf:
                foreach (var nerf in mNerfList)
                {
                    mOwner.SetNerf(nerf.Initialize(mOwner, mTarget));
                }
                break;
            case SkillType.Heal:
                {
                    mTarget.TakeRecover(newValue);
                    foreach (var buff in mBuffList)
                        mOwner.SetBuff(buff.Initialize(mOwner, mTarget));
                    foreach (var nerf in mNerfList)
                        mOwner.SetNerf(nerf.Initialize(mOwner, mTarget));
                }
                    break;
        }
    }

    private void Shoot()
    {
        float newValue = mValue + mOwner.mStatus.mMagicPower + mOwner.mBonusStatus.mMagicPower;
        Vector3 dir = (mTarget.transform.position - mOwner.transform.position).normalized;
        mProjectile = Instantiate(Resources.Load<GameObject>("Prefabs/Bullets/" + mName), mOwner.transform.position + dir * mStartPosition.x, Quaternion.identity);
        mProjectile.GetComponent<Projectile>().mDamage = newValue;
        mProjectile.transform.LookAt(dir);

        mProjectile.GetComponent<Projectile>().Initialize(mTarget,
    () => {
        bool isHit = true;
        isHit = mTarget.TakeDamage(newValue, DamageType.Magical);
        foreach (var buff in mBuffList)
            mOwner.SetBuff(buff.Initialize(mOwner, mOwner));
        if (isHit)
        {
            foreach (var nerf in mNerfList)
                mOwner.SetNerf(nerf.Initialize(mOwner, mTarget));
        }
    });

    }
    float maxDist = 0.0f;
    private void Raycasting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (mProperty == SkillProperty.Friendly)
        {                   
            BattleManager.Instance.mSpellChanning = true;

            if (Physics.Raycast(ray, out hit, 100, (mOwner.GetComponent<Unit>().mFlag == Flag.Player) ? LayerMask.GetMask("Ally") 
                : LayerMask.GetMask("Enemy")))
            {
                if (maxDist < hit.distance)
                {
                    mTarget?.mField.TargetedFriendly(false);
                    mTarget?.mSelected.SetActive(false);
                    mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                    mTarget?.mSelected.SetActive(true);
                    maxDist = hit.distance;
                }

                if (mTarget.gameObject != hit.collider.gameObject)
                {
                    mTarget?.mField.TargetedFriendly(false);
                    mTarget?.mSelected.SetActive(false);
                    mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                    mTarget?.mSelected.SetActive(true);
                    maxDist = hit.distance;
                }

            }
            else if(BattleManager.Instance.mCurrentUnit.mTarget)
            {
                if(mTarget != BattleManager.Instance.mCurrentUnit.mTarget)
                {
                    mTarget?.mField.TargetedFriendly(false);
                    mTarget?.mSelected.SetActive(false);
                    mTarget = (BattleManager.Instance.mCurrentUnit.mTarget.mConditions.isDied == false) ? BattleManager.Instance.mCurrentUnit.mTarget : null;
                    mTarget?.mSelected.SetActive(true);
                }
            }
            else
            {
                maxDist = 0.0f;
                mTarget?.mField.TargetedFriendly(false);
                mTarget?.mSelected.SetActive(false);
                mTarget = null;
            }
            mTarget?.mField.TargetedFriendly(true);
        }
        else
        {
            if (Physics.Raycast(ray, out hit, 100, (mOwner.GetComponent<Unit>().mFlag == Flag.Player) ? LayerMask.GetMask("Enemy")
                : LayerMask.GetMask("Ally")))
            {
                if (maxDist < hit.distance)
                {
                    mTarget?.mField.TargetedMagicHostile(false);
                    mTarget?.mSelected.SetActive(false);
                    mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                    mTarget?.mSelected.SetActive(true);
                    maxDist = hit.distance;
                }

                if (mTarget && mTarget.gameObject != hit.collider.gameObject)
                {
                    mTarget?.mField.TargetedMagicHostile(false);
                    mTarget?.mSelected.SetActive(false);
                    mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                    mTarget?.mSelected.SetActive(true);
                    maxDist = hit.distance;
                }

                mTarget?.mField.TargetedMagicHostile(true);
            }
            else
            {
                maxDist = 0.0f;
                mTarget?.mField.TargetedMagicHostile(false);
                mTarget?.mSelected.SetActive(false);
                mTarget = null;
            }
        }

    }

    public override void Initialize(Unit owner)
    {
        mOwner = owner;
        if (mValue <= 0.0f)
            mValue = owner.mStatus.mMagicPower;
    }
}
