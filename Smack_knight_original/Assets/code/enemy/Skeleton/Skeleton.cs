using UnityEngine;
using Pathfinding;
using UnityEngine.Events;
using System.Collections;

public class Skeleton : MonoBehaviour
{
    Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    private float ChompCooldown = -3;

    Path path;
    int currentWaypoint = 0;
    bool chomping = false;

    Seeker seeker;
    Rigidbody2D rb;

    public LayerMask walls;
    public LayerMask nothing;

    Vector2 force;
    Vector2 direction;
    Vector2 directionToPlayer;
    Vector2 directionToChomp;
    Vector3 savedPos;
    private GameObject Player;
    private MoveControler PlayerHealth;
    bool stop = true;
    bool isFacingRight = true;
    [SerializeField] Animator chaseAnim;
    private IEnumerator coroutine;
    private bool attackChomp;
    [SerializeField] private Collider2D col;
    [SerializeField] public float chompSpeed;
    private TrailRenderer tr;
    [SerializeField] private AudioSource charge;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        tr = transform.GetChild(0).gameObject.GetComponent<TrailRenderer>();

        Player = GameObject.FindWithTag("Player");
        target = Player.transform;
        if(Player != null)
            PlayerHealth = Player.GetComponent<MoveControler>();

// calculates the shortest path to the player with A* pathfinding
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        // finding the path
        if(seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p){

// when a path is found assighn it to movement (does't happen if the enemy is attacking or being knockbacked)
        if(!p.error && stop && chomping == false){
            path = p;
            currentWaypoint = 0;
           }
    }


    // Update is called once per frame
    void Update()
    {
        if(path == null)
            return;

// flips the enemy to face the player (does't happen if the enemy is attacking or being knockbacked)
        if (stop && chomping == false)
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

        if(stop && chomping == false)
            rb.linearVelocity = force;

// calculating variables for further use in attacking
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        float distanceToPlayer = Vector2.Distance(rb.position, Player.transform.position);

        if(distance < nextWaypointDistance && stop && chomping == false)
        {
            currentWaypoint++;
        }

// attacking if the distance to the player is small enough and can attack
        if(distanceToPlayer <= 8f && Time.time >= ChompCooldown){
            ChompCooldown = Time.time + 3f;
            coroutine = Chomp();
            StartCoroutine(coroutine);
        }
    }

    private void OnCollisionEnter2D(Collision2D other){
        //damidging and pushing the player
        if(other.transform.gameObject.tag == "Player"){
            PlayerHealth.TakeDamage(1, directionToPlayer);
        }
    }

    IEnumerator Chomp(){
// saving the position player is at and than waiting a 0.5 s before leaping
        chaseAnim.SetBool("chomp", true);
        chomping = true;
        charge.Play();
        
        savedPos = Player.transform.position;
        directionToChomp = ((Vector2)savedPos - rb.position).normalized;

        yield return new WaitForSecondsRealtime(0.5f);

// an attemp to rotate a collider to face the player => next time watch a video tutorial
// and it uses radiants idiot you need deegreeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeees

    //    attackCol.gameObject.transform.position = new Vector2(((savedPos.x) - transform.position.x) / 2f + transform.position.x, ((savedPos.y) - transform.position.y) / 2f + transform.position.y);
    //    attackCol.size = new Vector2(Mathf.Abs(savedPos.x - transform.position.x), 2f);

        //Quaternion rot = new Quaternion(0, 0, 0, 0);
        //rot = Quaternion.Euler((Vector3)directionToPlayer);

        //rot.SetFromToRotation(attackCol.gameObject.transform.position, savedPos);
        // I need to rotate the game object onli on the z axis

// leaping for 0.2 s
        rb.excludeLayers = walls;
        attackChomp = true;
        tr.emitting = true;

        yield return new WaitForSecondsRealtime(0.2f);

// seting everything needed for attacking back to normal
        tr.emitting = false;
        chaseAnim.SetBool("chomp", false);
        rb.excludeLayers = nothing;
        chomping = false;
        attackChomp = false;

    }
    void FixedUpdate()
    {
        // attacking (leaping)
        if (attackChomp)
        {
            rb.linearVelocity = directionToChomp * chompSpeed;
        }
    }

    public void Stop(bool halt){
        stop = halt;
        chaseAnim.SetBool("Hit", stop);
        // stops the enemy from moving so he can be knockbacked and interupts the attack (if atacking)
        if (stop == false && chomping == true)
            {
                chaseAnim.SetBool("chomp", false);
                chomping = false;
                attackChomp = false;
                tr.emitting = false;
                rb.excludeLayers = nothing;
                StopCoroutine(coroutine);
            }
    }

    public void Die(){
// displays the death animation and stops the function of the enemy
        chaseAnim.SetBool("Death", true);
        col.enabled = false;
        stop = false;
        tr.emitting = false;
        attackChomp = false;
        if (chomping == true){
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
