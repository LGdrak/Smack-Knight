using UnityEngine;
using UnityEngine.Events;
using System.Collections;
public class TurretScript : MonoBehaviour
{
    private GameObject Player;
    private MoveControler PlayerHealth;

    private Vector2 direction;
    private Rigidbody2D rb;

    [SerializeField] GameObject firePoint;
    private float SpitCooldown = -3;
    public Animator ShootAnim;
    bool stop = true;
    bool spiting = false;
    private IEnumerator coroutine;
    private Collider2D col;
    [SerializeField] private AudioSource shoot;
    public GameObject[] TurretAmmo;
    public GameObject Anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //used for finding the plyer object
        Player = GameObject.FindWithTag("Player");
        if(Player != null)
            PlayerHealth = Player.GetComponent<MoveControler>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Player){
            direction = (Player.transform.position - Anim.transform.position).normalized;
        }

        float distanceToPlayer = Vector2.Distance(rb.position, Player.transform.position);
        
        // if the Player is within range and can attack attacks
        if(distanceToPlayer <= 10f){
            if(Time.time >= SpitCooldown){
                SpitCooldown = Time.time + 3f;
                coroutine = Spit(direction);
                StartCoroutine(coroutine);
            }
        }

    }
    private void OnCollisionEnter2D(Collision2D other){
        //damidging and pushing the player
        if(other.transform.gameObject.tag == "Player"){
            PlayerHealth.TakeDamage(1, direction);
        }
    }

    IEnumerator Spit(Vector2 _direction){
// pulling a bullet to the shooting position and setting its direction with IsFacingRight
        ShootAnim.SetBool("Shooting", true);
        spiting = true;

        yield return new WaitForSecondsRealtime(0.916f);

        // pulling a bullet to the shooting position and setting its direction
        TurretAmmo[FindTurretAmmo()].transform.position = firePoint.transform.position;
        TurretAmmo[FindTurretAmmo()].GetComponent<SimpleSpitScript>().SetDirection(direction);
        shoot.Play();

        yield return new WaitForSecondsRealtime(2.83f);
        ShootAnim.SetBool("Shooting", false);
        spiting = false;
    }
// finds a bullet not being used to fire
    private int FindTurretAmmo()
    {
        for (int i = 0; i < TurretAmmo.Length; i++)
        {
            if (!TurretAmmo[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

    public void Stop(bool halt){
        stop = halt;
        ShootAnim.SetBool("Hit", stop);
        // stops the enemy from moving so he can be knockbacked and interupts the attack (if atacking)
        if (stop == false && spiting == true)
            {
                ShootAnim.SetBool("Shooting", false);
                spiting = false;
                SpitCooldown = Time.time + 3f;
                StopCoroutine(coroutine);
            }
    }

    public void Die(){
// displays the death animation and stops the function of the enemy
        Destroy(gameObject.GetComponent<SpriteRenderer>());
        ShootAnim.SetBool("Death", true);
        col.enabled = false;
        stop = false;
        if (spiting == true){
            StopCoroutine(coroutine);
        }
    }
}
