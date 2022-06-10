using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagicBehavior : State
{
    private List<GameObject> list = new List<GameObject>();
    private Skill_Setting skill;
    public override void Enter(Unit agent)
    {
        skill = agent.GetComponent<Skill_DataBase>().Skill;
        list = (agent.mFlag == Flag.Enemy) ? GameManager.Instance.mEnemyProwler.mEnemySpawnGroup.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList()
: PlayerController.Instance.mHeroes.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList();
        FindTarget(agent);
    }

    public override void Execute(Unit agent)
    {
        BattleManager.Instance.Magic();
        agent.mAiBuild.stateMachine.ChangeState("Waiting");
    }

    public override void Exit(Unit agent)
    {
    }

    private void FindTarget(Unit agent)
    {
        if (skill.mProperty == SkillProperty.Friendly)
        {
            switch (skill.mSkillType)
            {
                case SkillType.Heal:
                    {
                        float minPercent = float.MaxValue;
                        for (int i = 0; i < list.Count; i++)
                        {
                            var unit = list[i].GetComponent<Unit>();
                            float percent = Mathf.Round((100.0f * unit.mStatus.mHealth) / unit.mStatus.mMaxHealth);
                            if (minPercent > percent)
                            {
                                minPercent = percent;
                                agent.mTarget = unit;
                            }
                        }
                    }
                    break;
                case SkillType.BuffNerf:
                case SkillType.Buff:
                    {
                        float maxDamage = float.MaxValue;
                        for (int i = 0; i < list.Count; i++)
                        {
                            var unit = list[i].GetComponent<Unit>();
                            if (maxDamage > unit.mStatus.mDamage)
                            {
                                maxDamage = unit.mStatus.mDamage;
                                agent.mTarget = list[i].GetComponent<Unit>();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
