using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class HealState : SwordBaseState
{
    private InputAction attackUp;
    private InputAction attackDown;
    private InputAction attackLeft;
    private InputAction attackRight;
    private InputAction Switch;
    private InputAction Switch1;
    private InputAction Switch2;
    private InputAction Switch3;

    Transform attackPointUp;
    Transform attackPointDown;
    Transform attackPointLeft;
    Transform attackPointRight;

    public float NextCastTime = 4f;
    private float AttackRate = 4f;
    // the amount of healt that is healed with the heal spell
    public int healtValue = 2;

    public override void EnterState(SwordStateManager sword, GameObject[] _SoulBlast){
        
        attackPointUp = sword.transform.GetChild(0).gameObject.transform;
        attackPointDown = sword.transform.GetChild(1).gameObject.transform;
        attackPointLeft = sword.transform.GetChild(2).gameObject.transform;
        attackPointRight = sword.transform.GetChild(3).gameObject.transform;

        attackUp = InputSystem.actions.FindAction("attack-up");
        attackDown = InputSystem.actions.FindAction("attack-down");
        attackLeft = InputSystem.actions.FindAction("attack-left");
        attackRight = InputSystem.actions.FindAction("attack-right");
        Switch1 = InputSystem.actions.FindAction("switch-spell-1");
        Switch2 = InputSystem.actions.FindAction("switch-spell-2");
        Switch3 = InputSystem.actions.FindAction("switch-spell-3");
        Switch = InputSystem.actions.FindAction("switch-sword");

    }
    
    public override void UpdateState(SwordStateManager sword){


        //switch to other state based on player input
        if(Switch.triggered){
            sword.SwitchState(sword.swordState);
        }
        else if(Switch2.triggered && sword.spells > 1){
            sword.SwitchState(sword.castState);
        }
        else if(Switch1.triggered){
            sword.SwitchState(sword.swordState);
        }
        else if(Switch3.triggered && sword.spells > 2){
            sword.SwitchState(sword.laserState);
        }

// heals based on player input and cooldown
        if(Time.time >= NextCastTime && sword.souls >= 2){
            
            if(attackUp.triggered){
                sword.StartCoroutine(Cast(Vector2.up, attackPointUp, sword));
                NextCastTime = Time.time + AttackRate;
                sword.cooldownHealObj.SetActive(true);
            }
            if(attackDown.triggered){
                sword.StartCoroutine(Cast(Vector2.down, attackPointDown, sword));
                NextCastTime = Time.time + AttackRate;
                sword.cooldownHealObj.SetActive(true);
            }
            if(attackLeft.triggered){
                sword.StartCoroutine(Cast(Vector2.left, attackPointLeft, sword));
                NextCastTime = Time.time + AttackRate;
                sword.cooldownHealObj.SetActive(true);
            }
            if(attackRight.triggered){
                sword.StartCoroutine(Cast(Vector2.right, attackPointRight, sword));
                NextCastTime = Time.time + AttackRate;
                sword.cooldownHealObj.SetActive(true);
            }
        }

    }
    
    public override void ExitState(SwordStateManager sword){}

    IEnumerator Cast(Vector2 _direction, Transform FirePoint, SwordStateManager sword)
    {
        // substracting souls and activating a wall
        sword.souls -= 2;
        FirePoint.GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1.5f);
// heal the player + a few miliseconds when he cant move + a wall

        sword.move.HealControl(healtValue);
        FirePoint.GetChild(0).gameObject.SetActive(false);

    }

}
