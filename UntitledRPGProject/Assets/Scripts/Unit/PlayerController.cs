using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float mSpeed = 5.0f;
    [SerializeField]
    private float mGroundDistance = 2.0f;
    private float mTrunSmoothVelocity = 0.0f;
    private bool isGrounded = true;

    private ControlState mState = new IdleState();

    public GameObject mOwner;
    public List<GameObject> mHeroes = new List<GameObject>();

    public LayerMask mGroundMask;
    private Vector3 mVelocity = Vector3.zero;
    private Vector3 mGroundPos = Vector3.zero;
    private CharacterController mCharacterController;

    [SerializeField]
    private Transform mCamera;
    private float mGravity = -9.8f;

    private bool onBattle = false;
    // Start is called before the first frame update
    void Start()
    {
        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.position = new Vector3(0.0f, -1.0f, 0.0f);
        groundCheck.transform.parent = transform;
        GameManager.Instance.mPlayer = this;
        mCharacterController = GetComponent<CharacterController>();
        GameManager.Instance.onPlayerBattleStart += OnBattleStart;
        GameManager.Instance.onPlayerBattleEnd += OnBattleEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if (mOwner.activeInHierarchy == false)
            return;
        mState = mState.Handle();
        isGrounded = Physics.CheckSphere(mGroundPos, mGroundDistance, mGroundMask);
        if(mState.ToString() != "BattleState")
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 direction = new Vector3(x, 0.0f, z).normalized;
            if (direction.magnitude > 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref mTrunSmoothVelocity, 0.15f);
                transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
                Vector3 moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
                mCharacterController.Move(moveDirection.normalized * mSpeed * Time.deltaTime);
            }
        }

        if (isGrounded && mVelocity.y <= 0.0f)
            mVelocity.y = -2.0f;

        mVelocity.y += mGravity * Time.deltaTime;

        mCharacterController.Move(mVelocity * Time.deltaTime);
    }

    private GameObject[] fields;
    private GameObject[] enemyFields;
    private Vector3 mPos;
    public void OnBattleStart()
    {
        onBattle = true;
        mState = new BattleState();
        mOwner.SetActive(false);
        fields = GameObject.FindGameObjectsWithTag("PlayerField");
        enemyFields = GameObject.FindGameObjectsWithTag("EnemyField");
        for (int i = 0; i < mHeroes.Count; ++i)
        {
            mHeroes[i].transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
            mHeroes[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            mHeroes[i].GetComponent<Unit>().SetPosition(fields[i].transform.position, enemyFields[i].transform.position);
            mHeroes[i].gameObject.SetActive(true);
            mHeroes[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        mPos = fields[0].transform.position;
        //TODO: Spawn them
    }

    public void OnBattleEnd()
    {
        mState = new IdleState();
        onBattle = false;
        GameManager.Instance.mEnemyProwler = null;
        transform.position = mPos;
        transform.rotation = mHeroes[0].transform.rotation;
        mOwner.SetActive(true);
        for (int i = 0; i < mHeroes.Count; ++i)
        {
            mHeroes[i].transform.position = transform.position;
            mHeroes[i].gameObject.SetActive(false);
        }
        //TODO: Make them hide
    }


    private void OnTriggerEnter(Collider other)
    {
        if (onBattle)
            return;
        if (other.GetComponent<EnemyProwler>() != null && !other.GetComponent<EnemyProwler>().onBattle)
        {
            other.GetComponent<EnemyProwler>().onBattle = onBattle = true;
            GameManager.Instance.mEnemyProwler = other.GetComponent<EnemyProwler>();
            BattleManager.Instance.SetBattleField();
            GameManager.Instance.OnBattleStart(other.GetComponent<EnemyProwler>().id);
            //TODO call game manager to start the battle
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.onPlayerBattleStart -= OnBattleStart;
        GameManager.Instance.onPlayerBattleEnd -= OnBattleEnd;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onPlayerBattleStart -= OnBattleStart;
        GameManager.Instance.onPlayerBattleEnd -= OnBattleEnd;
    }
}