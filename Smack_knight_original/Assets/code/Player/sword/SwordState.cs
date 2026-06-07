using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SwordState : SwordBaseState
{
    // player input
    private PlayerInput playerInput;
    private InputAction attackUp;
    private InputAction attackDown;
    private InputAction attackLeft;
    private InputAction attackRight;
    private InputAction Switch1;
    private InputAction Switch2;
    private InputAction Switch3;

// attack points
    Transform attackPointUp;
    Transform attackPointDown;
    Transform attackPointLeft;
    Transform attackPointRight;
    private Transform attackPoint;
    public GameObject book;
    private int angle;

    public float attackRange = 1.6f;
    private LayerMask enemyLayers;
    private LayerMask wallLayers;
    Vector2 direction;
    public int damage = 20;
    public float AttackRate = 0.5f;
    public bool blocking;
    public bool poison;
    float NextAttackTime = 0f;

    enemy EnemyRef;
    SimpleSpitScript SpitRef;

    public override void EnterState(SwordStateManager sword, GameObject[] _SoulBlast){

        attackPointUp = sword.transform.GetChild(0).gameObject.transform;
        attackPointDown = sword.transform.GetChild(1).gameObject.transform;
        attackPointLeft = sword.transform.GetChild(2).gameObject.transform;
        attackPointRight = sword.transform.GetChild(3).gameObject.transform;
        book = sword.transform.GetChild(5).gameObject;

        enemyLayers = LayerMask.GetMask("enemyes");
        wallLayers = LayerMask.GetMask("walls", "border");

        playerInput = sword.GetComponent<PlayerInput>();

        Debug.Log(damage);

        attackUp = InputSystem.actions.FindAction("attack-up");
        attackDown = InputSystem.actions.FindAction("attack-down");
        attackLeft = InputSystem.actions.FindAction("attack-left");
        attackRight = InputSystem.actions.FindAction("attack-right");
        Switch1 = InputSystem.actions.FindAction("switch-spell-1");
        Switch2 = InputSystem.actions.FindAction("switch-spell-2");
        Switch3 = InputSystem.actions.FindAction("switch-spell-3");
    }
    
    // switches to other states based on player input and unlocked spells
    public override void UpdateState(SwordStateManager sword){
//switch to cast when switching
        if(Switch2.triggered && sword.spells > 1){
            sword.SwitchState(sword.castState);
            Debug.Log(sword.spells);
        }
        if(Switch1.triggered && sword.spells > 0){
            sword.SwitchState(sword.healState);
        }
        if(Switch3.triggered && sword.spells > 2){
            sword.SwitchState(sword.laserState);
        }

// attacks based on player input
        if(Time.time >= NextAttackTime){
            
            if(attackUp.triggered || attackUp.IsPressed()){
                sword.StartCoroutine(Attack(attackPointUp, sword));
                NextAttackTime = Time.time + AttackRate;
            }
            else if(attackDown.triggered || attackDown.IsPressed()){
                sword.StartCoroutine(Attack(attackPointDown, sword));
                NextAttackTime = Time.time + AttackRate;
            }
            else if(attackLeft.triggered || attackLeft.IsPressed()){
                sword.StartCoroutine(Attack(attackPointLeft, sword));
                NextAttackTime = Time.time + AttackRate;
            }
            else if(attackRight.triggered || attackRight.IsPressed()){
                sword.StartCoroutine(Attack(attackPointRight, sword));
                NextAttackTime = Time.time + AttackRate;
            }
        }
    }

    public override void ExitState(SwordStateManager sword){}

    IEnumerator Attack(Transform attackPoint, SwordStateManager sword)
    {
//animating swinging of the sword
// sets the right rotation of the swing
        if(attackPoint == attackPointUp){angle = 90;}
        else if(attackPoint == attackPointDown){angle = 270;}
        else if(attackPoint == attackPointLeft){angle = 180;}
        else if(attackPoint == attackPointRight){angle = 0;}

// angles the book in the right direction and triggers the attack anim
        book.transform.position = attackPoint.position;
        book.transform.rotation = Quaternion.Euler(0, 0, angle);
        sword.playerAnim.SetTrigger("swing");

        yield return new WaitForSecondsRealtime(AttackRate / 2);

//Searching for and hitting enmies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //damidging enemies and checking not to hit behind walls
        foreach(Collider2D enemy in hitEnemies)
        {
            direction = (enemy.gameObject.transform.position - sword.transform.parent.gameObject.transform.position).normalized;
            RaycastHit2D _hit = (Physics2D.Raycast(sword.transform.parent.gameObject.transform.position, direction, 100, wallLayers));

            if (Vector3.Distance(new Vector3(_hit.point.x, _hit.point.y, 0f), sword.transform.parent.gameObject.transform.position) > Vector3.Distance(enemy.gameObject.transform.position, sword.transform.parent.gameObject.transform.position))
            {
                EnemyRef = enemy.GetComponent<enemy>();
                if(EnemyRef != null)
                    if(poison == true){
                        EnemyRef.TakeDamage(damage, direction, true);
                        EnemyRef.Poison();
                    }
                    else{
                        EnemyRef.TakeDamage(damage, direction, true);
                    }
                    
                else if(EnemyRef == null && blocking == true)
                {
                    SpitRef = enemy.GetComponent<SimpleSpitScript>();
                    if (SpitRef != null)
                    {
                        SpitRef.Deactivate();
                    }
                }
            }
            else{
                Debug.Log("Enemy behind a wall");
            }    
        }
    }
}
