using UnityEngine;

// a abstract class that defines all states
public abstract class MoveDefaultState
{
    public abstract void EnterState(MoveControler move, Vector2 currentDirection);

    public abstract void UpdateState(MoveControler move);

    public abstract void FixedUpdateState(MoveControler move);

    public abstract Vector2 ExitState(MoveControler move);
}
