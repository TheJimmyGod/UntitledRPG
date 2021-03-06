using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    public GameObject mObject;
    public bool mInitialized = false;
    protected int ID = 0;
    public Color mGizmoColor;
    protected abstract GameObject CreateNewObject();

    protected virtual void Start()
    {
        GameObject manager = FindObjectOfType<UnitSpawnManager>().gameObject;
        transform.SetParent(manager.transform);
    }

    public virtual void Spawn(bool isDungeon = false)
    {
        if (mInitialized)
            return;
        ID = GameManager.s_ID++;
        mObject = CreateNewObject();
        if (mObject == null)
        {
            Debug.Log("Failed to create");
            mInitialized = false;
        }
        else
            mInitialized = true;
    }
    public virtual void ResetSpawn()
    {
        mInitialized = false;

        if (mObject != null)
            Destroy(mObject);
        Spawn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = mGizmoColor;
        Gizmos.color = new Color(mGizmoColor.r, mGizmoColor.g, mGizmoColor.b,1.0f);
        Gizmos.DrawCube(transform.position, new Vector3(1.0f, 1.0f, 1.0f));
    }
}
