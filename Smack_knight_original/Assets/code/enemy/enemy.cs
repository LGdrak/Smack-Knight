using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;

// a general script for all enemyes to control their health
public class enemy : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;
    [SerializeField] int knockbackSpeed = 300;
    [SerializeField] float knockbackTime = 0.1f;
    float deathTime = 0.66f;
    public Vector2 direction;
    Rigidbody2D rb;
    SwordStateManager sword;
    GameObject sw;
    [SerializeField] int Soul;
    public bool halfed = false;
    private bool poisoned = false;

    public UnityEvent<bool> stopEvent;
    public UnityEvent<bool> dieEvent;
    [SerializeField] private AudioSource hit;
    public bool IsBuffed = false;
    public bool imortal = false;
    public ParticleSystem system;
    public bool boss = false;
    private Slider bossBar;
    private Image bossHit;
    private GameObject bossObj;
    Color newCol;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {//setting starting health
        system.Stop();
        currentHealth = maxHealth;
        rb = gameObject.GetComponent<Rigidbody2D>();

        sw = GameObject.FindWithTag("sword");
        sword = sw.GetComponent<SwordStateManager>();

        if (boss)
        {
            bossObj = GameObject.FindWithTag("bossBar");
            bossObj.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            Debug.Log(bossObj);
            bossBar = bossObj.gameObject.transform.GetChild(0).gameObject.GetComponent<Slider>();
            bossHit = bossObj.gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Image>();
            Debug.Log(bossHit);
        }

    }

    public void TakeDamage(int damage, Vector2 _direction, bool meele){
        if (imortal == false)
        {
            //taking damage, bool meele decides whether the enemy was killed with a sword
            currentHealth -= damage;
            if (bossBar != null)
            {
                StartCoroutine(Boss());
            }
            direction = _direction;
            hit.Play();

            if(currentHealth <= 0){
                // kills the enmy
                StartCoroutine(Die(meele));
                return;
            }
// aplies knockback to a enemy
            StartCoroutine(Knockback(direction));
        }
        else
        {
            StartCoroutine(Knockback(direction));
        }

    }

    IEnumerator Boss()
    { 
        if (ColorUtility.TryParseHtmlString("#ffffff", out newCol))
            bossHit.color  = newCol;
    
        yield return new WaitForSecondsRealtime(0.1f);

        if (ColorUtility.TryParseHtmlString("#E01E22", out newCol))
            bossHit.color  = newCol;
        bossBar.value = currentHealth;
    }

    public IEnumerator Die(bool meele){
//killing the enmy
// invokes a event that is linkt to the enemys other script to stop movement and play animations
        dieEvent.Invoke(true);

        yield return new WaitForSecondsRealtime(deathTime);
        
        if (meele)
        {
            if (halfed)
            {
                if (Random.Range(0, 2) == 0)
                {
                    sword.SoulExtraction(Soul);
                }
            }
            else
            {
                sword.SoulExtraction(Soul);
            }
        }

        if (bossBar != null)
        {
            bossBar.gameObject.SetActive(false);
            Destroy(gameObject.transform.parent.gameObject);
        }
        Destroy(gameObject);
    }

    IEnumerator Knockback(Vector2 _direction) {

// the knocback event is linked with the enemys other script to prevent overriding directions
        stopEvent.Invoke(false);
        rb.linearVelocity = new Vector2(_direction.x * knockbackSpeed * Time.fixedDeltaTime, _direction.y * knockbackSpeed * Time.fixedDeltaTime);

        yield return new WaitForSecondsRealtime(knockbackTime);
        stopEvent.Invoke(true);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // stoping the enemy on contact
		rb.linearVelocity = Vector2.zero;
    }

    public void Poison()
    {
        if (poisoned == false){
            InvokeRepeating("Poisoned", 0f, 5f);
            poisoned = true;
        }
    }

    private void Poisoned()
    {
        if (imortal == false)
        {
                //taking damage, bool meele decides whether the enemy was killed with a sword
            currentHealth -= 5;

            if(currentHealth <= 0){
                // kills the enmy
                StartCoroutine(Die(false));
                return;
            }
// aplies knockback to a enemy
            StartCoroutine(Knockback(direction));
        }
        else
        {
            StartCoroutine(Knockback(direction));
        }

    }
    public void Buffed()
    {
        imortal = true;
        IsBuffed = true;
        system.Play();
    }
    public void Cancel()
    {
        imortal = false;
        IsBuffed = false;
        if (system != null)
        {
            system.Stop();
        }
    }

}
