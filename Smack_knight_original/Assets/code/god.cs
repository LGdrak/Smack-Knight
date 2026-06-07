using UnityEngine;
using UnityEngine.Events;
using System.Collections;
public class god : MonoBehaviour
{
    private GameObject Player;
    private MoveControler PlayerHealth;

    public float speed = 2f;
    private Vector2 direction;
    private Rigidbody2D rb;
    float DefDistanceRay = 100f;
    LayerMask barriers;

    [SerializeField] GameObject firePoint;
    private float ActionCooldown = -3;
    [SerializeField] GameObject ProjectileHolder;
    public GameObject[] PewAmmo;
    public GameObject BamAmmo;
    Animator godAnim;
    bool stop = true;
    bool shooting = false;
    bool lasering = false;
    public GameObject LineObject;
    LineRenderer line;
    private Vector2 referenceDistance;
    public GameObject refObj;
    public Collider2D FistCol;
    public Collider2D SlashCol;
    public Collider2D col;
    private float laserDamageTime;
    [SerializeField] private AudioSource shoot;
    [SerializeField] private AudioSource Fisting;
    [SerializeField] private AudioSource Slashing;
    [SerializeField] private AudioSource laser;
    [SerializeField] private AudioSource charge;
    private GameObject SpawnHolder;
    public GameObject[] enemyes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //used for finding the plyer object
        barriers = LayerMask.GetMask("walls", "border", "Block", "player");
        Player = GameObject.FindWithTag("Player");
        if(Player != null)
            PlayerHealth = Player.GetComponent<MoveControler>();
        rb = GetComponent<Rigidbody2D>();
        godAnim = GetComponent<Animator>();
        line = LineObject.GetComponent<LineRenderer>();
// sets the ammo holder free so the bulets don't move with the enemy
        ProjectileHolder.transform.SetParent(null, true);
        SpawnHolder = GameObject.FindWithTag("enemyList");
    }

    // Update is called once per frame
    void Update()
    {

        if(Player){
            direction = (Player.transform.position - transform.position).normalized;
        }
        
        float distanceToPlayer = Vector2.Distance(rb.position, Player.transform.position);
        referenceDistance = (Vector2)refObj.transform.position;

     // if the Player is within range and can attack attacks
        
        if(Time.time >= ActionCooldown){
            ActionCooldown = Time.time + 6f;

            if (distanceToPlayer < 4 && Random.Range(0, 3) <= 1)
            {
                ActionCooldown = Time.time + 2f;
                StartCoroutine(Slash());
            }
            else
            {
                if (distanceToPlayer > 15 || Random.Range(0, 10) == 0)
                {
                    col.enabled = false;
                    transform.position = Player.transform.position;
                    StartCoroutine(Fist());
                }
                else if(Random.Range(0, 2) == 0)
                {
                    if(Random.Range(0, 2) == 0)
                    {
                        StartCoroutine(PewPew());
                    }
                    else
                    {
                        ActionCooldown = Time.time + 8f;
                        StartCoroutine(Laser());
                    }
                }
                else
                {
                    ActionCooldown = Time.time + 6f;
                    StartCoroutine(Spawn());
                }
            }
        }
        
        // creating a laser when attacking
        if (lasering == true){
            if (Physics2D.Raycast(firePoint.transform.position, referenceDistance, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(firePoint.transform.position, referenceDistance, DefDistanceRay, barriers));
                Draw2DRay(firePoint.transform.position, _hit.point);

                if(_hit.transform.gameObject.tag == "Player" && laserDamageTime < Time.time){
                    PlayerHealth.TakeDamage(2, direction);
                    laserDamageTime = Time.time + 0.5f;
                }
            }
            else
            {
                Draw2DRay(firePoint.transform.position, referenceDistance * DefDistanceRay);
            }

            transform.Rotate (0f, 0f, -1f * 100 * Time.deltaTime, Space.World);
        }
    }

    void FixedUpdate()
    {
        // moving the enemy
        if(shooting == false)
            rb.linearVelocity = direction * speed;
        else
        {
            // stoping the enmy when attacking
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D other){
        //damidging and pushing the player
        if(other.transform.gameObject.tag == "Player"){
            PlayerHealth.TakeDamage(2, direction);
        }
    }
    private void OnTriggerEnter2D(Collider2D other){
        //damidging and pushing the player
        if(other.transform.gameObject.tag == "Player"){
            PlayerHealth.TakeDamage(2, direction);
        }
    }

    IEnumerator Fist(){

        godAnim.SetBool("Fist", true);
        shooting = true;

        yield return new WaitForSecondsRealtime(0.41666f);

        FistCol.enabled = true;

        yield return new WaitForSecondsRealtime(0.08333f);

        FistCol.enabled = false;

        yield return new WaitForSecondsRealtime(0.333f);

        Fisting.Play();
        godAnim.SetBool("Fist", false);
        shooting = false;
        col.enabled = true;

        if(Random.Range(0, 2) == 0)
        {
            StartCoroutine(Slash());
        }
        else
        {
            StartCoroutine(PewPew());
        }
    }

        IEnumerator Slash(){
// pulling a bullet to the shooting position and setting its direction with IsFacingRight
        col.enabled = false;
        godAnim.SetBool("Slash", true);
        shooting = true;

        yield return new WaitForSecondsRealtime(0.5666f);

        SlashCol.enabled = true;

        yield return new WaitForSecondsRealtime(0.18333f);

        SlashCol.enabled = false;

        Slashing.Play();
        godAnim.SetBool("Slash", false);
        shooting = false;
        col.enabled = true;
    }

    IEnumerator PewPew(){
// pulling a bullet to the shooting position and setting its direction with IsFacingRight
        godAnim.SetBool("PewPew", true);
        shooting = true;

        yield return new WaitForSecondsRealtime(0.38333f);

// pulling a bullet to the shooting position and setting its direction
        PewAmmo[FindPewAmmo()].transform.position = firePoint.transform.position;
        PewAmmo[FindPewAmmo()].GetComponent<SimpleSpitScript>().SetDirection(direction);
        shoot.Play();

        yield return new WaitForSecondsRealtime(0.45f);

// pulling a bullet to the shooting position and setting its direction
        PewAmmo[FindPewAmmo()].transform.position = firePoint.transform.position;
        PewAmmo[FindPewAmmo()].GetComponent<SimpleSpitScript>().SetDirection(direction);
        shoot.Play();

        yield return new WaitForSecondsRealtime(0.1666f);

        godAnim.SetBool("PewPew", false);
        shooting = false;
        StartCoroutine(Bam());
    }

// finds a bullet not being used to fire
    private int FindPewAmmo()
    {
        for (int i = 0; i < PewAmmo.Length; i++)
        {
            if (!PewAmmo[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
        IEnumerator Bam(){
// pulling a bullet to the shooting position and setting its direction with IsFacingRight
        godAnim.SetBool("Bam", true);
        shooting = true;

        yield return new WaitForSecondsRealtime(0.5666f);

// pulling a bullet to the shooting position and setting its direction
        BamAmmo.transform.position = firePoint.transform.position;
        BamAmmo.GetComponent<SimpleSpitScript>().SetDirection(direction);
        shoot.Play();

        yield return new WaitForSecondsRealtime(0.8f);

        godAnim.SetBool("Bam", false);
        shooting = false;
    }

    IEnumerator Spawn(){
// pulling a bullet to the shooting position and setting its direction with IsFacingRight
        godAnim.SetBool("Spawn", true);
        shooting = true;

        yield return new WaitForSecondsRealtime(0.666f);

        for (int i = 0; i < 5; i++)
        {
            Instantiate(enemyes[Random.Range(0, 8)], transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0), Quaternion.identity, SpawnHolder.transform);
        }
        
        yield return new WaitForSecondsRealtime(0.3666f);

        godAnim.SetBool("Spawn", false);
        shooting = false;
    }

    IEnumerator Laser()
    {
        // animation and attack bools
        shooting = true;
        godAnim.SetBool("Laser", true);
        charge.Play();

// charging the laser
        yield return new WaitForSecondsRealtime(1.08333f);

// enabel the collision and draw the laser
        lasering = true;

        if (Physics2D.Raycast(firePoint.transform.position, referenceDistance, DefDistanceRay, barriers))
            {
                RaycastHit2D _hit = (Physics2D.Raycast(firePoint.transform.position, referenceDistance, DefDistanceRay, barriers));
                Draw2DRay(firePoint.transform.position, _hit.point);

                if(_hit.transform.gameObject.tag == "Player" && laserDamageTime < Time.time){
                    PlayerHealth.TakeDamage(2, direction);
                    laserDamageTime = Time.time + 0.5f;
                }
            }
            else
            {
                Draw2DRay(firePoint.transform.position, referenceDistance * DefDistanceRay);
            }
        
        LineObject.SetActive(true);
        laser.Play();

        yield return new WaitForSecondsRealtime(3.6f);

// stop fireing and turn off the bools
        transform.rotation = Quaternion.Euler(0,0,0);
        laser.Stop();
        lasering = false;
        shooting = false;
        godAnim.SetBool("Laser", false);
        LineObject.SetActive(false);
    }
    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);

    }

    public void Stop(bool halt){
        
    }

    public void Die(){
// displays the death animation and stops the function of the enemy
        rb.linearVelocity = Vector2.zero;
        StopAllCoroutines();
        Destroy(ProjectileHolder);
        godAnim.SetBool("Death", true);  
    }
}
