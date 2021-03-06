using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject mEffect;
    public bool isCollide = false;
    protected bool isEffect = false;
    [SerializeField]
    protected float mSpeed = 10.0f;
    [SerializeField]
    protected float mChannelingTime = 1.0f;
    [SerializeField]
    protected float mMaximumDistance = 2.5f;
    protected Unit mTarget;
    protected Action mActionEvent;
    protected int mAnimationCount;
    [HideInInspector]
    public float mDamage;

    public AudioClip[] clip = new AudioClip[3];

    protected virtual void Start()
    {
        if(GetComponent<Animator>() != null)
            mAnimationCount = GetComponent<Animator>().runtimeAnimatorController.animationClips.Length;
        mEffect = transform.GetChild(0).transform.gameObject;
        if (mEffect)
        {
            mEffect.transform.position = transform.position;
            if(transform.GetComponent<SpriteRenderer>()!= null)
            {
                if (mTarget.mFlag == Flag.Player)
                {
                    mEffect.transform.eulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
                    transform.GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    mEffect.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
                    transform.GetComponent<SpriteRenderer>().flipX = false;
                }
            }

            isEffect = true;
            StartCoroutine(WaitforSecond());
            mEffect.SetActive(true);
        }    
    }
    public virtual void Initialize(Unit target, Action actionEvent)
    {
        mTarget = target;
        mActionEvent += actionEvent;
    }

    protected virtual void FixedUpdate()
    {
        if(isCollide == false)
        {
            if (isEffect)
                return;
            transform.position = Vector3.MoveTowards(transform.position, mTarget.transform.position, mSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, mTarget.transform.position) < mMaximumDistance && !isCollide)
            {
                isCollide = true;
                int random = UnityEngine.Random.Range(1, 2);
                if(GetComponent<Animator>() != null)
                    GetComponent<Animator>().Play((mAnimationCount >= 3) ? "Burst" + UnityEngine.Random.Range(1, 2) : "Burst");
                mActionEvent?.Invoke();
                if(clip.Length > 0)
                    AudioManager.PlaySfx(clip[UnityEngine.Random.Range(0,clip.Length-1)]);
                Destroy(this.gameObject,1.0f);
            }
        }
    }

    protected virtual IEnumerator WaitforSecond()
    {
        yield return new WaitForSeconds(mChannelingTime);
        isEffect = false;
    }
}
