using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CastState : SwordBaseState
{

    private InputAction attackUp;
    private InputAction attackDown;
    private InputAction attackLeft;
    private InputAction attackRight;
    private InputAction Switch;
    private InputAction Switch2;
    private InputAction Switch3;
    private InputAction Switch1;

    Transform attackPointUp;
    Transform attackPointDown;
    Transform attackPointLeft;
    Transform attackPointRight;

    public float NextCastTime = 2f;
    private float AttackRate = 10f;
    public int blastDamage = 50;
    GameObject[] SoulBlast;

    public override void EnterState(SwordStateManager sword, GameObject[] _SoulBlast){
        
        attackPointUp = sword.transform.GetChild(0).gameObject.transform;
        attackPointDown = sword.transform.GetChild(1).gameObject.transform;
        attackPointLeft = sword.transform.GetChild(2).gameObject.transform;
        attackPointRight = sword.transform.GetChild(3).gameObject.transform;

        SoulBlast = _SoulBlast;

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
        else if(Switch1.triggered && sword.spells > 0){
            sword.SwitchState(sword.healState);
        }
        else if(Switch3.triggered && sword.spells > 2){
            sword.SwitchState(sword.laserState);
        }
        else if(Switch2.triggered){
            sword.SwitchState(sword.swordState);
        }

// attacking based on player input and cooldown
        if(Time.time >= NextCastTime){
            
            if(attackUp.triggered){
                Cast(Vector2.up, attackPointUp, sword);
                NextCastTime = Time.time + AttackRate;
                sword.cooldownCastObj.SetActive(true);
            }
            if(attackDown.triggered){
                Cast(Vector2.down, attackPointDown, sword);
                NextCastTime = Time.time + AttackRate;
                sword.cooldownCastObj.SetActive(true);
            }
            if(attackLeft.triggered){
                Cast(Vector2.left, attackPointLeft, sword);
                NextCastTime = Time.time + AttackRate;
                sword.cooldownCastObj.SetActive(true);
            }
            if(attackRight.triggered){
                Cast(Vector2.right, attackPointRight, sword);
                NextCastTime = Time.time + AttackRate;
                sword.cooldownCastObj.SetActive(true);
            }
        }

    }
    
    public override void ExitState(SwordStateManager sword){}

    void Cast(Vector2 _direction, Transform FirePoint, SwordStateManager sword)
    {
// pulling a bullet to the shooting position and setting its direction
        SoulBlast[FindSoulBlast()].transform.position = FirePoint.position;
        SoulBlast[FindSoulBlast()].GetComponent<Soul_Blast>().SetDirection(_direction, blastDamage);
    }

// finds a bullet not being used to fire
    private int FindSoulBlast()
    {
        for (int i = 0; i < SoulBlast.Length; i++)
        {
            if (!SoulBlast[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
}
