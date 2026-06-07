using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class LaserState : SwordBaseState
{
    private InputAction attackUp;
    private InputAction attackDown;
    private InputAction attackLeft;
    private InputAction attackRight;
    private InputAction Switch;
    private InputAction Switch2;
    private InputAction Switch1;
    private InputAction Switch3;


    Transform attackPointUp;
    Transform attackPointDown;
    Transform attackPointLeft;
    Transform attackPointRight;

    float DefDistanceRay = 100f;
    public int LaserDamageValue = 100;
    GameObject LineObject;
    LineRenderer line;
    
    public float NextCastTime = 4f;
    private float AttackRate = 4f;
    bool casting = false;
    Vector2 direction_;
    Transform FirePoint_;
    LayerMask barriers;
    BoxCollider2D col;

    public override void EnterState(SwordStateManager sword, GameObject[] _SoulBlast){
        
        attackPointUp = sword.transform.GetChild(0).gameObject.transform;
        attackPointDown = sword.transform.GetChild(1).gameObject.transform;
        attackPointLeft = sword.transform.GetChild(2).gameObject.transform;
        attackPointRight = sword.transform.GetChild(3).gameObject.transform;
        LineObject = sword.transform.GetChild(4).gameObject;
        Debug.Log(LineObject);

        line = LineObject.GetComponent<LineRenderer>();

        attackUp = InputSystem.actions.FindAction("attack-up");
        attackDown = InputSystem.actions.FindAction("attack-down");
        attackLeft = InputSystem.actions.FindAction("attack-left");
        attackRight = InputSystem.actions.FindAction("attack-right");
        Switch1 = InputSystem.actions.FindAction("switch-spell-1");
        Switch2 = InputSystem.actions.FindAction("switch-spell-2");
        Switch3 = InputSystem.actions.FindAction("switch-spell-3");
        Switch = InputSystem.actions.FindAction("switch-sword");

        barriers = LayerMask.GetMask("walls", "border");
        col = LineObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>();

    }
    
    public override void UpdateState(SwordStateManager sword){

// casts a laser when attacking
        if (casting == true){
            if (Physics2D.Raycast(FirePoint_.position, direction_, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(FirePoint_.position, direction_, DefDistanceRay, barriers));
                Draw2DRay(FirePoint_.position, _hit.point);
            }
            else
            {
                Draw2DRay(FirePoint_.position, direction_ * DefDistanceRay);
            }
        }

        //switch to other state based on player input
        if(Switch.triggered){
            sword.SwitchState(sword.swordState);
        }
        else if(Switch1.triggered && sword.spells > 0){
            sword.SwitchState(sword.healState);
        }
        else if(Switch2.triggered && sword.spells > 1){
            sword.SwitchState(sword.castState);
        }
        else if(Switch3.triggered){
            sword.SwitchState(sword.swordState);
        }

// attacking based on player input and cooldown
        if(Time.time >= NextCastTime && sword.souls >= 3 && casting == false){
            
            if(attackUp.triggered){
                sword.StartCoroutine(Cast(Vector2.up, attackPointUp, sword));
                NextCastTime = Time.time + AttackRate;
                sword.cooldownLaserObj.SetActive(true);
            }
            if(attackDown.triggered){
                sword.StartCoroutine(Cast(Vector2.down, attackPointDown, sword));
                NextCastTime = Time.time + AttackRate;
                sword.cooldownLaserObj.SetActive(true);
            }
            if(attackLeft.triggered){
                sword.StartCoroutine(Cast(Vector2.left, attackPointLeft, sword));
                NextCastTime = Time.time + AttackRate;
                sword.cooldownLaserObj.SetActive(true);
            }
            if(attackRight.triggered){
                sword.StartCoroutine(Cast(Vector2.right, attackPointRight, sword));
                NextCastTime = Time.time + AttackRate;
                sword.cooldownLaserObj.SetActive(true);
            }
        }

    }
    
    public override void ExitState(SwordStateManager sword){}

    IEnumerator Cast(Vector2 _direction, Transform FirePoint, SwordStateManager sword)
    {
        // fireing a laser, all of the bools needed
        sword.souls -= 3;
        direction_ = _direction;
        FirePoint_ = FirePoint;
        casting = true;
        col.enabled = true;

        if (Physics2D.Raycast(FirePoint_.position, direction_, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(FirePoint_.position, direction_, DefDistanceRay, barriers));
                Draw2DRay(FirePoint_.position, _hit.point);
            }
            else
            {
                Draw2DRay(FirePoint_.position, FirePoint_.right * DefDistanceRay);
            }
        
        // activates the laser
        LineObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

// stops the laser frol fireing
        col.enabled = false;
        casting = false;
        LineObject.SetActive(false);
    }

// draws the laser and sets its collider if statements for diferent axises
    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);

        if(direction_ == Vector2.right || direction_ == Vector2.left){
            col.gameObject.transform.position = new Vector2((endPos.x - startPos.x) / 2f + startPos.x, endPos.y);
            col.size = new Vector2(Mathf.Abs(endPos.x - startPos.x), 0.5f);
        }
        else if(direction_ == Vector2.up || direction_ == Vector2.down){
            col.gameObject.transform.position = new Vector2(endPos.x, (endPos.y - startPos.y) / 2f + startPos.y);
            col.size = new Vector2(0.5f, Mathf.Abs(endPos.y - startPos.y));
        }
        
    }

    public void LaserDamage(Collider2D laser)
    {
//damidging enemies
        try
        {
            laser.GetComponent<enemy>().TakeDamage(LaserDamageValue, direction_, false);
        }
        catch (System.Exception)
        {
            Debug.Log("not a damidgable enemy");
        }  
    }
}
