using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedBuff
{
    protected int mTurn;
    protected int mStack;
    public bool isActive = false;
    public Buff Buff { get; }
    protected readonly Unit mOwner;
    protected readonly Unit mTarget;

    public TimedBuff(Buff buff, Unit owner, Unit target)
    {
        Buff = buff;
        mOwner = owner;
        mTarget = target;
    }

    public void Tick()
    {
        if (isActive)
        {
            if (mTurn <= 0)
            {
                isActive = false;
                End();
            }
            else
            {
                if (Buff.BuffTick)
                    AudioManager.PlaySfx(Buff.BuffTick);
                Apply();
            }
            mTurn--;
        }
    }
    public void Activate()
    {
        if (Buff.BuffStart)
            AudioManager.PlaySfx(Buff.BuffStart);
        Apply();
        if (!Buff.IsTurnFinished || mTurn <= 0)
            mTurn += Buff.mTurn;
        else if (!Buff.IsTurnFinished)
            mTurn = Buff.mTurn;
    }

    public void Inactivate()
    {
        isActive = false;
        End();
    }

    protected abstract void Apply();
    public abstract void End();
}
