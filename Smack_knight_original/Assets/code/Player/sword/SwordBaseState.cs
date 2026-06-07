using UnityEngine;

// a abstract class for all of the states
public abstract class SwordBaseState 
{
    public abstract void EnterState(SwordStateManager sword, GameObject[] _SoulBlast);

    public abstract void UpdateState(SwordStateManager sword);

    public abstract void ExitState(SwordStateManager sword);
}
