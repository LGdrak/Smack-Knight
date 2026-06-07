using UnityEngine;
using System.Collections;
public class Kim : MonoBehaviour
{
    public float speed = 200f;

    Rigidbody2D rb;

    Vector2 force;
    Vector2 direction;
    Vector2 directionToPlayer;
    private GameObject Player;
    private MoveControler PlayerHealth;
    bool stop = true;
    [SerializeField] Animator SlugAnim;
    [SerializeField] private Collider2D col;
    private float directionChange = -3;
    public float attackRange = 3.3f;
    public LayerMask enemyLayers;
    public bool laser = false;
    private bool attacking = false;
    [SerializeField] private GameObject LineObj1;
    [SerializeField] private GameObject LineObj2;
    [SerializeField] private GameObject LineObj3;
    [SerializeField] private GameObject LineObj4;
    float laserDamageTime;
    float DefDistanceRay = 100f;
    LayerMask barriers;
    LineRenderer line1;
    LineRenderer line2;
    LineRenderer line3;
    LineRenderer line4;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (laser)
        {
            line1 = LineObj1.GetComponent<LineRenderer>();
            line2 = LineObj2.GetComponent<LineRenderer>();
            line3 = LineObj3.GetComponent<LineRenderer>();
            line4 = LineObj4.GetComponent<LineRenderer>();
        }

        barriers = LayerMask.GetMask("walls", "border", "Block", "player");
        Player = GameObject.FindWithTag("Player");
        Debug.Log(Player);
        if(Player != null)
            PlayerHealth = Player.GetComponent<MoveControler>();

        direction = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
    }
    // Update is called once per frame
    void Update()
    {
                // creating a laser when attacking
        if (attacking == true){
            if (Physics2D.Raycast(transform.position, Vector2.up, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(transform.position, Vector2.up, DefDistanceRay, barriers));
                Draw2DRay(transform.position, _hit.point, Vector2.up, line1);

                if(_hit.transform.gameObject.tag == "Player" && laserDamageTime < Time.time){
                    PlayerHealth.TakeDamage(2, direction);
                    laserDamageTime = Time.time + 1f;
                }
            }
            else
            {
                Draw2DRay(transform.position, Vector2.up * DefDistanceRay, Vector2.up, line1);
            }
        if (Physics2D.Raycast(transform.position, Vector2.down, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(transform.position, Vector2.down, DefDistanceRay, barriers));
                Draw2DRay(transform.position, _hit.point, Vector2.down, line2);

                if(_hit.transform.gameObject.tag == "Player" && laserDamageTime < Time.time){
                    PlayerHealth.TakeDamage(2, direction);
                    laserDamageTime = Time.time + 0.5f;
                }
            }
            else
            {
                Draw2DRay(transform.position, Vector2.down * DefDistanceRay, Vector2.down, line2);
            }
        if (Physics2D.Raycast(transform.position, Vector2.left, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(transform.position, Vector2.left, DefDistanceRay, barriers));
                Draw2DRay(transform.position, _hit.point, Vector2.left, line3);

                if(_hit.transform.gameObject.tag == "Player" && laserDamageTime < Time.time){
                    PlayerHealth.TakeDamage(2, direction);
                    laserDamageTime = Time.time + 0.5f;
                }
            }
            else
            {
                Draw2DRay(transform.position, Vector2.left * DefDistanceRay, Vector2.left, line3);
            }
        if (Physics2D.Raycast(transform.position, Vector2.right, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(transform.position, Vector2.right, DefDistanceRay, barriers));
                Draw2DRay(transform.position, _hit.point, Vector2.right, line4);

                if(_hit.transform.gameObject.tag == "Player" && laserDamageTime < Time.time){
                    PlayerHealth.TakeDamage(2, direction);
                    laserDamageTime = Time.time + 0.5f;
                }
            }
            else
            {
                Draw2DRay(transform.position, Vector2.right * DefDistanceRay, Vector2.right, line4);
            }
        }
        
        directionToPlayer = (Vector2)Player.transform.position - rb.position;
        force = direction * speed;
        float distanceToPlayer = Vector2.Distance(rb.position, Player.transform.position);
        
        if (distanceToPlayer >= 15f && Time.time >= directionChange)
            {
                Change();
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

        direction = Vector2.Reflect(direction, (Vector2)other.gameObject.transform.position).normalized;
    }
    public void Stop(bool halt){
        stop = halt;
        SlugAnim.SetBool("Hit", stop);
    }

    public void Die(){
// displays the death animation and stops the function of the enemy
        col.enabled = false;
        stop = false;
        if (laser)
        {
            SlugAnim.SetBool("Death", true);
            DeathLaser();
        }
        else
        {
            StartCoroutine(Explode());
        }
    }
        //turns the enemy to face the player when moving
    private IEnumerator Explode()
    {
        SlugAnim.SetBool("Explode", true);

        yield return new WaitForSecondsRealtime(0.216f);

        //Checking for collision with bulets and lowering hp
//Searching for and hitting enmies affected by the explosion
        Collider2D[] explosion = Physics2D.OverlapCircleAll(gameObject.transform.position, attackRange, enemyLayers);

        foreach(Collider2D foe in explosion)
        {
        //damidging and pushing the player
            if(foe.transform.gameObject.tag == "Player"){
                PlayerHealth.TakeDamage(2, directionToPlayer);
            }
        }

        yield return new WaitForSecondsRealtime(0.033f);
        SlugAnim.SetBool("Explode", false);
    }
    private void DeathLaser()
    {
        // animation and attack bools
        attacking = true;

        if (attacking == true){
            if (Physics2D.Raycast(transform.position, Vector2.up, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(transform.position, Vector2.up, DefDistanceRay, barriers));
                Draw2DRay(transform.position, _hit.point, Vector2.up, line1);

                if(_hit.transform.gameObject.tag == "Player" && laserDamageTime < Time.time){
                    PlayerHealth.TakeDamage(2, direction);
                    laserDamageTime = Time.time + 0.5f;
                }
            }
            else
            {
                Draw2DRay(transform.position, Vector2.up * DefDistanceRay, Vector2.up, line1);
            }
        if (Physics2D.Raycast(transform.position, Vector2.down, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(transform.position, Vector2.down, DefDistanceRay, barriers));
                Draw2DRay(transform.position, _hit.point, Vector2.down, line2);

                if(_hit.transform.gameObject.tag == "Player" && laserDamageTime < Time.time){
                    PlayerHealth.TakeDamage(2, direction);
                    laserDamageTime = Time.time + 0.5f;
                }
            }
            else
            {
                Draw2DRay(transform.position, Vector2.down * DefDistanceRay, Vector2.down, line2);
            }
        if (Physics2D.Raycast(transform.position, Vector2.left, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(transform.position, Vector2.left, DefDistanceRay, barriers));
                Draw2DRay(transform.position, _hit.point, Vector2.left, line3);

                if(_hit.transform.gameObject.tag == "Player" && laserDamageTime < Time.time){
                    PlayerHealth.TakeDamage(2, direction);
                    laserDamageTime = Time.time + 0.5f;
                }
            }
            else
            {
                Draw2DRay(transform.position, Vector2.left * DefDistanceRay, Vector2.left, line3);
            }
        if (Physics2D.Raycast(transform.position, Vector2.right, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(transform.position, Vector2.right, DefDistanceRay, barriers));
                Draw2DRay(transform.position, _hit.point, Vector2.right, line4);

                if(_hit.transform.gameObject.tag == "Player" && laserDamageTime < Time.time){
                    PlayerHealth.TakeDamage(2, direction);
                    laserDamageTime = Time.time + 0.5f;
                }
            }
            else
            {
                Draw2DRay(transform.position, Vector2.right * DefDistanceRay, Vector2.right, line4);
            }
        
            LineObj1.SetActive(true);
            LineObj2.SetActive(true);
            LineObj3.SetActive(true);
            LineObj4.SetActive(true);
        }
    }

// the logic for drrawing rays
    void Draw2DRay(Vector2 startPos, Vector2 endPos, Vector2 direction_, LineRenderer line)
    {
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);        
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, 3.3f);
    }
}