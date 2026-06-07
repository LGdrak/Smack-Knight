using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class BatScript : MonoBehaviour
{
    private GameObject Player;
    private MoveControler PlayerHealth;

    public float speed = 1f;
    private Vector2 direction;
    private Rigidbody2D rb;
    bool isFacingRight = true;

    [SerializeField] GameObject firePoint;
    private float SpitCooldown = -3;
    [SerializeField] GameObject SpitAmmo;
    [SerializeField] GameObject SpitHolder;
    Animator batAnim;
    SimpleSpitScript Spitting;
    bool stop = true;
    bool spiting = false;
    private IEnumerator coroutine;
    private Collider2D col;
    [SerializeField] private AudioSource shoot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //used for finding the plyer object
        Player = GameObject.FindWithTag("Player");
        if(Player != null)
            PlayerHealth = Player.GetComponent<MoveControler>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        batAnim = GetComponent<Animator>();

        Spitting = SpitAmmo.GetComponent<SimpleSpitScript>();

// sets the ammo holder free so the bulets don't move with the enemy
        transform.GetChild(0).SetParent(null, true);
    }

    // Update is called once per frame
    void Update()
    {
        // flips the enemy to face the player (does't happen if the enemy is attacking)
        if (spiting == false)
            Flip();
        
        if(Player){
            direction = (Player.transform.position - transform.position).normalized;
        }
        
        float distanceToPlayer = Vector2.Distance(rb.position, Player.transform.position);

        // if the Player is within range and can attack attacks
        if(distanceToPlayer <= 7f){
            if(Time.time >= SpitCooldown){
                SpitCooldown = Time.time + 3f;
                coroutine = Spit(direction);
                StartCoroutine(coroutine);
            }
        }
    }

    void FixedUpdate()
    {
        // moving the enemy
        if(stop && spiting == false)
            rb.linearVelocity = direction * speed * Time.deltaTime;
// stoping the enmy when attacking
        if(spiting && stop)
            rb.linearVelocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D other){
        //damidging and pushing the player
        if(other.transform.gameObject.tag == "Player"){
            PlayerHealth.TakeDamage(1, direction);
        }
    }

    IEnumerator Spit(Vector2 _direction){
// pulling a bullet to the shooting position and setting its direction with IsFacingRight
        batAnim.SetBool("Shooting", true);
        spiting = true;

        yield return new WaitForSecondsRealtime(1.2f);

// shooting
        SpitAmmo.transform.position = firePoint.transform.position;
        Spitting.SetDirection(_direction);

        shoot.Play();
        batAnim.SetBool("Shooting", false);
        spiting = false;
    }

    public void Stop(bool halt){
        stop = halt;
        batAnim.SetBool("Hit", stop);
        // stops the enemy from moving so he can be knockbacked and interupts the attack (if atacking)
        if (stop == false && spiting == true)
            {
                batAnim.SetBool("Shooting", false);
                spiting = false;
                SpitCooldown = Time.time;
                StopCoroutine(coroutine);
            }
    }

    public void Die(){
// displays the death animation and stops the function of the enemy
        batAnim.SetBool("Death", true);
        col.enabled = false;
        rb.linearVelocity = Vector2.zero;
        if (spiting == true){
            StopCoroutine(coroutine);
        }
        Destroy(SpitHolder);
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
