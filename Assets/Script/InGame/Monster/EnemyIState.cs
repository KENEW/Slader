using UnityEngine;

public interface MonsterIState
{
    void OperateEnter();
    void OperateUpdate();
    void OperateExit();
}

public class StateMachine
{
    public MonsterIState CurrentState { get; private set; }

    public StateMachine(MonsterIState defaultState)
    {
        CurrentState = defaultState;
        CurrentState.OperateEnter();
    }

    public void SetState(MonsterIState state)
    {
        if (CurrentState == state)
        {
            return;
        }

        CurrentState.OperateExit();
        CurrentState = state;
        CurrentState.OperateEnter();
    }

    public void DoOperateupdate()
    {
        CurrentState.OperateUpdate();
    }
}

public class StateIdle : MonoBehaviour, MonsterIState
{
    protected Enemy enemy;
    protected Player player;

    public StateIdle(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public virtual void OperateEnter()
    {
    }
    public virtual void OperateExit()
    {
    }
    public virtual void OperateUpdate()
    {
    }
}
public class StateTrace : MonoBehaviour, MonsterIState
{
    protected Player player;
    protected Enemy enemy;

    public StateTrace(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public virtual void OperateEnter()
    {
    }
    public virtual void OperateExit()
    {
    }
    public virtual void OperateUpdate()
    {
    }
}
public class StateAttack : MonoBehaviour, MonsterIState
{
    protected Player player;
    protected Enemy enemy;

    public StateAttack(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public virtual void OperateEnter()
    {
    }
    public virtual void OperateExit()
    {
    }
    public virtual void OperateUpdate()
    {
    }
}