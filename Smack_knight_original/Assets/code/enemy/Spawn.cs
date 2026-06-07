using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour
{
    private GameObject Player;
    private MoveControler PlayerHealth;
    private Vector2 direction;
    private float SpitCooldown = -3;
    public Animator ShootAnim;
    bool stop = true;
    bool spiting = false;
    private IEnumerator coroutine;
    private Collider2D col;
    [SerializeField] private AudioSource shoot;
    public GameObject Anim;
    public GameObject[] enemyes;
    private int pick;
    private int MAX = 6;
    private RoundControler RC;
    private Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //used for finding the plyer object
        Player = GameObject.FindWithTag("Player");
        if(Player != null)
            PlayerHealth = Player.GetComponent<MoveControler>();
        col = GetComponent<Collider2D>();

        RC = GameObject.FindWithTag("RC").transform.GetComponent<RoundControler>();

        if (RC.round > 7){MAX = 7;}
        if (RC.round > 10){MAX = 10;}

    }

    // Update is called once per frame
    void Update()
    {
        if(Player){
            direction = (Player.transform.position - Anim.transform.position).normalized;
        }
        
        // if can attack attacks
        if(Time.time >= SpitCooldown){
            SpitCooldown = Time.time + 20f;
            coroutine = Spit();
            StartCoroutine(coroutine);
        }
    }
    private void OnCollisionEnter2D(Collision2D other){
        //damidging and pushing the player
        if(other.transform.gameObject.tag == "Player"){
            PlayerHealth.TakeDamage(1, direction);
        }
    }

    IEnumerator Spit(){
// pulling a bullet to the shooting position and setting its direction with IsFacingRight
        ShootAnim.SetBool("Shooting", true);
        spiting = true;

        yield return new WaitForSecondsRealtime(1f);

        // pulling a bullet to the shooting position and setting its direction
        Spawning();
        shoot.Play();

        yield return new WaitForSecondsRealtime(0.25f);
        ShootAnim.SetBool("Shooting", false);
        spiting = false;
    }

    private void Spawning()
    {
        pick = Random.Range(0, MAX);
        offset = new Vector3(Random.Range(1f, 4f), Random.Range(1f, 4f), 0);
        Instantiate(enemyes[pick], transform.position + offset, Quaternion.identity, gameObject.transform.parent.gameObject.transform);
    }

    public void Stop(bool halt){
        stop = halt;
        ShootAnim.SetBool("Hit", stop);
        // stops the enemy from moving so he can be knockbacked and interupts the attack (if atacking)
        if (stop == false && spiting == true)
            {
                ShootAnim.SetBool("Shooting", false);
                spiting = false;
                SpitCooldown = Time.time + 5f;
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
