using UnityEngine;
using Pathfinding;
using UnityEngine.Events;
using System.Collections;

public class Brim : MonoBehaviour
{
    Transform playerPos;
    private float SeekDistance;
    private float SeekDistanceChance;
    private Vector2 offset = new Vector2(0 , 0);

    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    private float AttackCooldown = -3;

    Path path;
    int currentWaypoint = 0;
    bool attacking = false;

    Seeker seeker;
    Rigidbody2D rb;

    Vector2 force;
    Vector2 direction;
    Vector2 directionToPlayer;
    private GameObject Player;
    private MoveControler PlayerHealth;
    bool stop = true;
    bool isFacingRight = true;
    private IEnumerator coroutine;
    [SerializeField] private Collider2D col;
    [SerializeField] private Transform FirePoint_;
    Vector2 direction_;
    float DefDistanceRay = 100f;
    float laserDamageTime = 0f;
    LayerMask barriers;
    GameObject LineObject;
    LineRenderer line;
    [SerializeField] private int aimOffset = 5;
    [SerializeField] Animator brimAnim;
    [SerializeField] private AudioSource charge;
    [SerializeField] private AudioSource laser;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        Player = GameObject.FindWithTag("Player");
        playerPos = Player.transform;
        barriers = LayerMask.GetMask("walls", "border", "Block", "player");
        LineObject = transform.GetChild(2).gameObject;
        line = LineObject.GetComponent<LineRenderer>();

        if(Player != null)
            PlayerHealth = Player.GetComponent<MoveControler>();

        // calculates the shortest path to the player with A* pathfinding
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        if(seeker.IsDone())

            SeekDistance = Vector2.Distance(rb.position, playerPos.position);

// calculating the nearest point on the x or y axis in a certain range of the player
            for (int i = -4; i <= 4; i++)
            {
                SeekDistanceChance = Vector2.Distance(rb.position, (Vector2)playerPos.position + new Vector2(0, i));
                if (Mathf.Abs(SeekDistanceChance) < Mathf.Abs(SeekDistance)){
                    SeekDistance = SeekDistanceChance;
                    offset = (Vector2)playerPos.position + new Vector2(0, i);
                }

                SeekDistanceChance = Vector2.Distance(rb.position, (Vector2)playerPos.position + new Vector2(i, 0));
                if (Mathf.Abs(SeekDistanceChance) < Mathf.Abs(SeekDistance)){
                    SeekDistance = SeekDistanceChance;
                    offset = (Vector2)playerPos.position + new Vector2(i, 0);
                }
            }

// finding the path
            seeker.StartPath(rb.position, offset, OnPathComplete);
    }

    void OnPathComplete(Path p){

// when a path is found assighn it to movement (does't happen if the enemy is attacking or being knockbacked)
        if(!p.error && stop && attacking == false){
            path = p;
            currentWaypoint = 0;
           }
    }


    // Update is called once per frame
    void Update()
    {
        // creating a laser when attacking
        if (attacking == true){
            if (Physics2D.Raycast(FirePoint_.position, direction_, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(FirePoint_.position, direction_, DefDistanceRay, barriers));
                Draw2DRay(FirePoint_.position, _hit.point);

                if(_hit.transform.gameObject.tag == "Player" && laserDamageTime < Time.time){
                    PlayerHealth.TakeDamage(2, direction);
                    laserDamageTime = Time.time + 1f;
                }
            }
            else
            {
                Draw2DRay(FirePoint_.position, direction_ * DefDistanceRay);
            }
        }

        if(path == null)
            return;

// flips the enemy to face the player (does't happen if the enemy is attacking or being knockbacked)
        if (stop && attacking == false)
            Flip();
        
        // checking if the enemy reached the end of the path
        if(currentWaypoint >= path.vectorPath.Count){
            UpdatePath();
            return;
        }

// calculating variables for further use in attacking
        direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        directionToPlayer = ((Vector2)Player.transform.position - rb.position).normalized;
        force = direction * speed;

// moving the enemy (does't happen if the enemy is attacking or being knockbacked)
        if(stop && attacking == false){
            rb.linearVelocity = force;
        }

// calculating variables for further use in attacking
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        float distanceToPlayer = Vector2.Distance(rb.position, Player.transform.position);

// increasing waypoints (does't happen if the enemy is attacking or being knockbacked)
        if(distance < nextWaypointDistance && stop && attacking == false)
        {
            currentWaypoint++;
        }

// triggering attack if the enemy is near the player and can attack
        if(currentWaypoint + aimOffset >= path.vectorPath.Count && Time.time >= AttackCooldown){
            AttackCooldown = Time.time + 3f;

// deciding the side to attack 
            if (directionToPlayer.x < 0 && Mathf.Abs(directionToPlayer.y) < Mathf.Abs(directionToPlayer.x)){
                direction_ = Vector2.left;
            }
            else if (directionToPlayer.x > 0 && Mathf.Abs(directionToPlayer.y) < Mathf.Abs(directionToPlayer.x)){
                direction_ = Vector2.right;
            }
            else if (directionToPlayer.y < 0 && Mathf.Abs(directionToPlayer.x) < Mathf.Abs(directionToPlayer.y)){
                direction_ = Vector2.down;
            }
            else if (directionToPlayer.y > 0 && Mathf.Abs(directionToPlayer.x) < Mathf.Abs(directionToPlayer.y)){
                direction_ = Vector2.up;
            }

            coroutine = Cast();
            StartCoroutine(coroutine);
        }
    }

    private void OnCollisionEnter2D(Collision2D other){
        //damidging and pushing the player
        if(other.transform.gameObject.tag == "Player"){
            PlayerHealth.TakeDamage(1, directionToPlayer);
        }
    }

// attacking
    IEnumerator Cast()
    {
        // animation and attack bools
        brimAnim.SetBool("attacking", true);
        rb.linearVelocity = Vector2.zero;
        charge.Play();

// charging the laser
        yield return new WaitForSecondsRealtime(0.5f);

// enabel the collision and draw the laser
        attacking = true;
        
        if (Physics2D.Raycast(FirePoint_.position, direction_, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(FirePoint_.position, direction_, DefDistanceRay, barriers));
                Draw2DRay(FirePoint_.position, _hit.point);

                if(_hit.transform.gameObject.tag == "Player" && laserDamageTime < Time.time){
                    PlayerHealth.TakeDamage(2, direction);
                    laserDamageTime = Time.time + 0.5f;
                }
            }
            else
            {
                Draw2DRay(FirePoint_.position, FirePoint_.right * DefDistanceRay);
            }
        
        LineObject.SetActive(true);
        laser.Play();

        yield return new WaitForSecondsRealtime(1f);

// stop fireing and turn off the bools
        laser.Stop();
        attacking = false;
        brimAnim.SetBool("attacking", false);
        LineObject.SetActive(false);
    }

// the logic for drrawing rays
    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
    }

    public void Stop(bool halt){
        // stops the enemy from moving so he can be knockbacked and interupts the attack (if atacking)
        stop = halt;
        brimAnim.SetBool("hit", halt);
        if (stop == false && attacking == true)
            {
                attacking = false;
                AttackCooldown = Time.time + 3f;
                LineObject.SetActive(false);
                brimAnim.SetBool("attacking", false);
                laser.Stop();
                StopCoroutine(coroutine);
            }

        if (stop){
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void Die(){
        // displays the death animation and stops the function of the enemy
        brimAnim.SetBool("death", true);
        col.enabled = false;
        stop = false;
        if (attacking == true){
            laser.Stop();
            StopCoroutine(coroutine);
        }
    }

        //turns the enemy to face the player when moving
    private void Flip()
    {
        if (isFacingRight && Player.transform.position.x - transform.position.x < 0 || !isFacingRight && Player.transform.position.x - transform.position.x > 0)
        {
            isFacingRight = !isFacingRight;
            
            transform.Rotate(0f, 180f, 0f);
        }
    }

}
