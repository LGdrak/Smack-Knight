using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DashState : MoveDefaultState
{

    private Rigidbody2D rb;

    public float dashSpeed = 1000f;
    public float dashTime = 0.3f;
    private TrailRenderer tr;
    CapsuleCollider2D col;

    public override void EnterState(MoveControler move, Vector2 currentDirection){

        rb = move.GetComponent<Rigidbody2D>();
        tr = move.GetComponent<TrailRenderer>();
        col = move.GetComponent<CapsuleCollider2D>();
        
        Debug.Log("dash");

// start dashing
        move.StartCoroutine(Dash(currentDirection, move));
    }

    public override void UpdateState(MoveControler move){
        
    }

    public override void FixedUpdateState(MoveControler move){

    }

    public override Vector2 ExitState(MoveControler move){
        return new Vector2(0,0);
    }



//Dash
    IEnumerator Dash(Vector2 direction,MoveControler move) {

// disables the collider so that the player can go through walls and enemyes
        tr.emitting = true;
        col.enabled = false;

        rb.linearVelocity = new Vector2(direction.x * dashSpeed * Time.fixedDeltaTime, direction.y * dashSpeed * Time.fixedDeltaTime);
        Debug.Log(rb.linearVelocity);

        yield return new WaitForSecondsRealtime(dashTime);
// waits, disables boolians and ends
        tr.emitting = false;
        col.enabled = true;
        move.SwitchState(move.walkState);
    }
}
