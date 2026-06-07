using UnityEngine;
using Pathfinding;
using UnityEngine.Events;
using System.Collections;
public class SlugScript : MonoBehaviour
{
    private Vector3 target;
    private Vector3 offset;
    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 0;
    Seeker seeker;
    Rigidbody2D rb;

    Vector2 force;
    Vector2 direction;
    Vector2 directionToPlayer;
    private GameObject Player;
    private MoveControler PlayerHealth;
    bool stop = true;
    bool isFacingRight = true;
    [SerializeField] Animator SlugAnim;
    [SerializeField] private Collider2D col;
    public int range = 5;
    private float directionChange = -3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        Player = GameObject.FindWithTag("Player");
        if(Player != null)
            PlayerHealth = Player.GetComponent<MoveControler>();

// calculates the shortest path to the player with A* pathfinding
        InvokeRepeating("UpdatePath", 0f, 5f);
    }

    void UpdatePath()
    {
        // finding the path
        if(seeker.IsDone())
            offset = new Vector3(Random.Range(-range, range), Random.Range(-range, range), 0); 
            target = transform.position + offset;
            seeker.StartPath(rb.position, target, OnPathComplete);
    }

    void OnPathComplete(Path p){

// when a path is found assighn it to movement (does't happen if the enemy is attacking or being knockbacked)
        if(!p.error && stop){
            path = p;
            currentWaypoint = 0;
           }
    }


    // Update is called once per frame
    void Update()
    {
        if(path == null)
            return;


        float distanceToPlayer = Vector2.Distance(rb.position, Player.transform.position);
        
        if (distanceToPlayer >= 15f && Time.time >= directionChange)
            {
                Change();
            }
// flips the enemy to face the player (does't happen if the enemy is attacking or being knockbacked)
        if (stop)
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
        
// calculating variables for further use in attacking
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if(distance < nextWaypointDistance && stop)
        {
            currentWaypoint++;
        }
    }
        private void Change()
    {
        directionChange = Time.time + 5f;
        int index = Random.Range(0, 3);
        if (index == 0)
        {
            transform.position = Player.transform.position + new Vector3(Random.Range(-7f, 7f), Random.Range(5f, 7f), 0);
        }
        else if(index == 1)
        {
            transform.position = Player.transform.position + new Vector3(Random.Range(-7f, 7f), Random.Range(-5f, -7f), 0);
        }
        else if(index == 2)
        {
            transform.position = Player.transform.position + new Vector3(Random.Range(5f, 7f), Random.Range(-5f, 5f), 0);
        }
        else
        {
            transform.position = Player.transform.position + new Vector3(Random.Range(-5f, -7f), Random.Range(-5f, 5f), 0);
        }
    }

    private void FixedUpdate() {
        if(stop)
            rb.linearVelocity = force;
    }

    private void OnCollisionEnter2D(Collision2D other){
        //damidging and pushing the player
        if(other.transform.gameObject.tag == "Player"){
            PlayerHealth.TakeDamage(1, directionToPlayer);
        }
    }
    public void Stop(bool halt){
        stop = halt;
        SlugAnim.SetBool("Hit", stop);
    }

    public void Die(){
// displays the death animation and stops the function of the enemy
        SlugAnim.SetBool("Death", true);
        col.enabled = false;
        stop = false;
    }
        //turns the enemy to face the player when moving
    private void Flip()
    {
        if (isFacingRight && target.x - transform.position.x < 0 || !isFacingRight && target.x - transform.position.x > 0)
        {
            isFacingRight = !isFacingRight;
            
            transform.Rotate(0f, 180f, 0f);
        }
        
    }
}
