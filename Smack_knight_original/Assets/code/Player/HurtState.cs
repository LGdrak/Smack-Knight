using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HurtState : MoveDefaultState
{
    public int currentHealth = 6;
    int knockbackSpeed = 300;
    float knockbackTime = 0.1f;
    Vector2 _direction;
    Rigidbody2D rb;
    GameObject Canvas;
    DeathMenu deathMenu;
    UIHeart uiHeart;
    GameObject Heart;
    private LayerMask enemyLayers;

    public override void EnterState(MoveControler move, Vector2 currentDirection){

        uiHeart = move.uiHeart;
        enemyLayers = LayerMask.GetMask("enemyes");

        rb = move.GetComponent<Rigidbody2D>();
        currentHealth -= move.currentDamage;
        uiHeart.Damaged(currentHealth);
        
        Canvas = GameObject.FindWithTag("Canvas");
        deathMenu = Canvas.GetComponent<DeathMenu>();
        
        _direction = move.direction;

            // checking if health isn't below zero and killing the player
        if(currentHealth <= 0 && deathMenu.GameOver == false){
            Die(move);
        }
        else
        {    // knockbacks the player
            move.StartCoroutine(Knockback(_direction, move));
        }
    }

    public override void UpdateState(MoveControler move){

        
    }

    public override void FixedUpdateState(MoveControler move){

    }

    public override Vector2 ExitState(MoveControler move){
        return new Vector2(0,0);
    }

    IEnumerator Knockback(Vector2 _direction,MoveControler move) {

// knockback the player
        move.Hurt.Play();
        rb.linearVelocity = new Vector2(_direction.x * knockbackSpeed * Time.fixedDeltaTime, _direction.y * knockbackSpeed * Time.fixedDeltaTime);

        yield return new WaitForSecondsRealtime(knockbackTime);

    // prolongs the knockback even when the player can move so he can get away
        rb.excludeLayers = enemyLayers;
        move.knockbackNoHitBool = true;
        move.SwitchState(move.walkState);

    }

    void Die(MoveControler move){
//killing the player
        move.gameObject.SetActive(false);
        deathMenu.Death();
    }
}
