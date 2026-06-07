using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WalkState : MoveDefaultState
{

    public float MoveSpeed = 6f;
    private Vector2 movement;
    private Vector2 smoothMovement;
    private Vector2 movementhSmoothVelocity;
    private Vector2 velocityRef = Vector3.zero;
    float smoothTime = 0.1f;

    private float dashRate = 10f;
    float NextDashTime = 0f;

    public Vector2 currentDirection;

    private Rigidbody2D rb;

    private InputAction horizontalAction;
    private InputAction verticalAction;

    private InputAction dash;
    private SwordStateManager sword;
    private Vector2 localMove;
    private Vector2 Move;
    private LayerMask nothing;
    private LayerMask enemyLayers;

    public override void EnterState(MoveControler move,Vector2 currentDirection){
        rb = move.GetComponent<Rigidbody2D>();

        sword = move.gameObject.transform.GetChild(0).GetComponent<SwordStateManager>();

        horizontalAction = InputSystem.actions.FindAction("horizontal");
        verticalAction = InputSystem.actions.FindAction("vertical");

        dash = InputSystem.actions.FindAction("dash");

// gives the player some extra time to recover after being hit
        if (move.knockbackNoHitBool == true)
        {
            move.StartCoroutine(KnockbackNoHit(move));
        }

    }

    public override void UpdateState(MoveControler move){

//movement
        movement.y = verticalAction.ReadValue<float>();
        movement.x = horizontalAction.ReadValue<float>();

        //switching to dash
        // && sword.currentState != sword.swordState ??????????? nechat nebo ne?
        if (dash.triggered && Time.time >= NextDashTime && Move != new Vector2(0, 0)){
            currentDirection = localMove;
            move.SwitchState(move.dashState);
            NextDashTime = Time.time + dashRate;
            move.dashObj.SetActive(false);
        }
// shoving the player can dash in UI
        if (Time.time >= NextDashTime)
        {
            move.dashObj.SetActive(true);
        }
    }

    public override Vector2 ExitState(MoveControler move){
        return currentDirection;
    }
    
//movement execution
    public override void FixedUpdateState(MoveControler move)
    {
// remove MovePosition and use dinamic forces!!!

        Move = new Vector2(movement.x, movement.y).normalized;
        localMove = move.transform.TransformDirection(Move);
        Vector2 targetVelocity = localMove * MoveSpeed;

        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocityRef, smoothTime);

    }

// gives the player some extra time to recover after being hit
    IEnumerator KnockbackNoHit(MoveControler move)
    {
        rb.excludeLayers = enemyLayers;

        yield return new WaitForSecondsRealtime(1f);

        rb.excludeLayers = nothing;

        move.knockbackNoHitBool = false;
    }

}
