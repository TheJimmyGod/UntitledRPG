using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyProwler : Prowler
{
    public List<GameObject> mEnemySpawnGroup;
    public bool isWin = false;

    public override void Setup(float rad, float ang, int _id, GameObject model)
    {
        base.Setup(rad, ang, _id, model);
        mEnemySpawnGroup = new List<GameObject>();
    }

    public override void Initialize()
    {
        GameManager.Instance.onBattle += EnemySpawn;
        GameManager.Instance.onEnemyWin += Win;

        base.Initialize();

        GameObject[] agent = GameObject.FindGameObjectsWithTag("Enemy");
        if (agent.Length > 1)
            for (int i = 0; i < agent.Length; i++)
                Physics.IgnoreCollision(this.GetComponent<Collider>(), agent[i].GetComponent<Collider>());
    }

    public void EnemySpawn(int id)
    {
        if(id == this.id)
        {
            mCollider.enabled = false;
            mModel.SetActive(false);
            StartCoroutine(WaitForSpawn());
        }
    }

    public void Win(int id, Action onAction = null)
    {
        if(id == this.id)
        {
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < mEnemySpawnGroup.Count; ++i)
            {
                if (mEnemySpawnGroup[i].GetComponent<Enemy>().mConditions.isDied)
                {
                    Destroy(mEnemySpawnGroup[i], 3.0f);
                    continue;
                }    
                list.Add(mEnemySpawnGroup[i]);
            }

            mEnemySpawnGroup.Clear();
            mEnemySpawnGroup = new List<GameObject>(list);

            for (int i = 0; i < mEnemySpawnGroup.Count; ++i)
            {
                mEnemySpawnGroup[i].GetComponent<Unit>().DisableUnit(transform.position);
                mEnemySpawnGroup[i].gameObject.SetActive(false);
            }
        }
        onBattle = false;
        mModel.SetActive(true);
        ChangeBehavior("Idle");
        onAction?.Invoke();
    }

    private IEnumerator WaitForSpawn()
    { 
        for (int i = 0; i < mEnemySpawnGroup.Count; ++i)
        {
            yield return new WaitForSeconds(0.1f);
            mEnemySpawnGroup[i].GetComponent<Unit>().EnableUnit(i);
        }
        onBattle = true;
    }    

    private void OnDestroy()
    {
        GameManager.Instance.onBattle -= EnemySpawn;
        GameManager.Instance.onEnemyWin -= Win;
    }
}
