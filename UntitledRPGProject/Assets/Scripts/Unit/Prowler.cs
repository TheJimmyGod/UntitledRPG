using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Prowler : MonoBehaviour
{
    public GameObject mModel;
    public GameObject mGroundCheck;
    public Animator mAnimator;
    public NavMeshAgent mAgent;
    public Rigidbody mRigidbody;
    protected BoxCollider mCollider;
    protected ProwlerStateMachine mStateMachine;
    protected SpriteRenderer mSpriteRenderer;
    public int id = 0;
    public bool onBattle = false;

    public float mRadius = 5.0f;
    public float mAngle = 60.0f;
    public float mSpeed = 1.0f;
    public float mStandbyTime = 3.0f;

    public Vector3 mLastPos = Vector3.zero;
    public Vector3 mVelocity = Vector3.zero;
    public float mOriginalSpeed = 0.0f;

    public float mWalkTime = 0.0f;
    public float mMaxWalkTime = 0.3f;
    private bool isGrounded = false;


    public List<SoundClip> _RunClip = new List<SoundClip>();

    protected virtual void Start()
    {
        mLastPos = transform.position;
    }

    public virtual void Setup(float rad, float ang, float speed, int _id, GameObject model)
    {
        mRadius = rad;
        mAngle = ang;
        mSpeed = speed;
        id = _id;
        mModel = model;
    }

    public virtual void Initialize()
    {
        mCollider = gameObject.AddComponent<BoxCollider>();
        mCollider.size = mModel.GetComponent<BoxCollider>().size;
        mCollider.isTrigger = true;

        if (mGroundCheck == null)
        {
            GameObject groundCheck = Instantiate(Resources.Load<GameObject>("Prefabs/UnitGroundCheck"), (
             new Vector3(transform.position.x,
                transform.position.y - (transform.GetComponent<BoxCollider>().size.y / 2.0f), transform.position.z)), Quaternion.identity);
            groundCheck.transform.parent = transform;
            mGroundCheck = groundCheck;
        }

        mRigidbody = gameObject.AddComponent<Rigidbody>();
        mRigidbody.detectCollisions = true;
        mAnimator = mModel.GetComponent<Animator>();
        mSpriteRenderer = mModel.GetComponent<SpriteRenderer>();
        mOriginalSpeed = mSpeed;

        mStateMachine = gameObject.AddComponent<ProwlerStateMachine>();
        mStateMachine.mAgent = this;
        mStateMachine.AddState<P_State>(new Idle(), "Idle");
        mStateMachine.AddState<P_State>(new Find(), "Find");
        mStateMachine.AddState<P_State>(new Stop(), "Stop");
        mStateMachine.AddState<P_State>(new Pursuit(), "Pursuit");
        mStateMachine.ChangeState("Idle");
    }

    protected virtual void Update()
    {
        CheckGround();
        if (LevelManager.Instance.isNext || GameManager.Instance.IsCinematicEvent || BattleManager.Instance.status != BattleManager.GameStatus.None
    || PlayerController.Instance.Interaction)
        {
            mStateMachine.ChangeState("Stop");
            return;
        }
        else
        {
            mStateMachine.ActivateState();
        }
        mSpriteRenderer.flipX = (mVelocity.x < -0.1f) ? true : false;

    }
    protected void CheckGround()
    {
        isGrounded = Physics.CheckSphere(mGroundCheck.transform.position, 2.0f, LayerMask.GetMask("Ground"));
        if (isGrounded && mVelocity.y < 0.0f)
            mVelocity.y = 0.0f;
        mVelocity.y += -9.8f * Time.deltaTime;
        mRigidbody.AddForce(mVelocity * Time.deltaTime);
    }
    public virtual void ChangeBehavior(string name)
    {
        mStateMachine.ChangeState(name);
    }
}
