using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrap : Item
{
    [HideInInspector]
    public Transform mTransform;
    [HideInInspector]
    public bool isSuccess = false;
    public override void Apply()
    {
        Vector3 pos = mTransform.position - new Vector3(0, 2, 0);
        GameObject place =  new GameObject("Spawner");
        place.transform.position = pos;
        place.AddComponent<EnemySpawner>();

        EnemyTrapInfo info = (EnemyTrapInfo)Info;

        if (info.mEnemyUnits.Length == 0)
            return;

        GameObject newEnemyProwler = new GameObject("Enemy" + " " + GameManager.s_ID++);
        newEnemyProwler.transform.position = new Vector3(pos.x,
            pos.y + 1.0f,
            pos.z);

        int LeaderCount = 0;
        for (int i = 0; i < info.mEnemyUnits.Length; ++i)
        {
            if (info.mEnemyUnits[i] == EnemyUnit.None)
                LeaderCount++;
            else
                break;
        }

        GameObject newModel = Instantiate(Resources.Load<GameObject>("Prefabs/Units/Enemys/" + info.mEnemyUnits[LeaderCount].ToString()), newEnemyProwler.transform.position, Quaternion.identity, newEnemyProwler.transform);
        newEnemyProwler.tag = "EnemyProwler";
        newEnemyProwler.layer = 6;
        newEnemyProwler.AddComponent<EnemyProwler>().Setup(30, 15, 3, ID, newModel.gameObject);

        for (int i = 0; i < info.mEnemyUnits.Length; i++)
        {
            if (info.mEnemyUnits[i] == EnemyUnit.None)
                continue;
            GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Units/Enemys/" + info.mEnemyUnits[i].ToString() + "_Unit"), pos, Quaternion.identity, newEnemyProwler.transform);
            newEnemyProwler.GetComponent<EnemyProwler>().mEnemySpawnGroup.Add(obj);
            obj.SetActive(false);
        }

        newEnemyProwler.GetComponent<EnemyProwler>()._RunClip = newEnemyProwler.GetComponent<EnemyProwler>().mEnemySpawnGroup[0].GetComponent<Unit>().mSetting.Clips.FindAll(
            delegate (SoundClip s)
            {
                return s.Type == SoundClip.SoundType.Run;
            });

        newEnemyProwler.GetComponent<EnemyProwler>().Initialize(place.GetComponent<EnemySpawner>());
        isSuccess = true;
    }

    public override void End()
    {
    }
}
