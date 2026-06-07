using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
public class Buffer : MonoBehaviour
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
    private GameObject enemyList;
    private int buffCount;
    private enemy enemyScript;
    private List<enemy> enemyMemory = new List<enemy>();
    private int pick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //used for finding the plyer object
        Player = GameObject.FindWithTag("Player");
        if(Player != null)
            PlayerHealth = Player.GetComponent<MoveControler>();
        col = GetComponent<Collider2D>();
        enemyList = GameObject.FindWithTag("enemyList");
    }

    // Update is called once per frame
    void Update()
    {
        if(Player){
            direction = (Player.transform.position - Anim.transform.position).normalized;
        }
        
        // if can attack attacks
        if(Time.time >= SpitCooldown){
            SpitCooldown = Time.time + 15f;
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
        Buff();
        shoot.Play();

        yield return new WaitForSecondsRealtime(0.1f);
        ShootAnim.SetBool("Shooting", false);
        spiting = false;
    }

    private void Buff()
    {
        pick = Random.Range(1, enemyList.transform.childCount - 1);
        enemyScript = enemyList.transform.GetChild(pick).GetComponent<enemy>();
        if (enemyScript.IsBuffed == false)
        {
            enemyScript.Buffed();
            enemyMemory.Add(enemyScript);
            buffCount = 0;
        }
        else
        {
            buffCount ++;
            if (buffCount < 11)
            {
                Buff();
            }
            else
            {
                buffCount = 0;
            }
        }
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

        foreach (enemy item in enemyMemory)
        {
            try
            {
                item.Cancel();
            }
            catch (System.Exception)
            {
                Debug.Log("HowWWWWWWWWWWWWWWWWWWWWWW?");
            }
            
        }
    }
}
