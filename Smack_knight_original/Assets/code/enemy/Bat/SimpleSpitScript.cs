using UnityEngine;
using System.Collections;

public class SimpleSpitScript : MonoBehaviour
{
    [SerializeField] private float BulletSpeed;
    private CircleCollider2D boxCollider;
    private bool hit;
    private float lifetime;

    private GameObject Player;
    private MoveControler PlayerHealth;
    private Vector2 direction;
    public bool explodes = false;
    public LayerMask enemyLayers;
    public Animator anim;
    private void Awake()
    {
        boxCollider = GetComponent<CircleCollider2D>();

        Player = GameObject.FindWithTag("Player");
        if(Player != null)
            PlayerHealth = Player.GetComponent<MoveControler>();
    }


    // Update is called once per frame
    void Update()
    {
// check if the buleet colided before moving it
        if (hit)
        {
            return;
        } 
//moving the bullet
        transform.Translate(direction * BulletSpeed * Time.deltaTime);
// despavning bullet after certain time
        lifetime += Time.deltaTime;
        if(lifetime > 2.5)
        {
            gameObject.SetActive(false);
        }
    }
// checking for collision and deactivating the bullet
    private void OnCollisionEnter2D(Collision2D other)
    {
//Checking for collision with player and lowering hp

        if(other.transform.gameObject.tag == "Player"){
            PlayerHealth.TakeDamage(1, direction);
        }
        else if (explodes == true)
        {
            StartCoroutine(Explode());
        }
        
        if(explodes == false)
        {
            Deactivate();
        }

    }
    
// setting direction for the bullet and activating it
    public void SetDirection(Vector2 _direction)
    {
        direction = _direction;

        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;
        lifetime = 0;

    }
// deactivating the bullet
    public void Deactivate()
    {
        hit = true;
        boxCollider.enabled = false;
        gameObject.SetActive(false);
    }

    private IEnumerator Explode()
    {
        anim.SetBool("Explode", true);
        hit = true;

        yield return new WaitForSecondsRealtime(0.216f);
        
        //Checking for collision with bulets and lowering hp
//Searching for and hitting enmies affected by the explosion
        Collider2D[] explosion = Physics2D.OverlapCircleAll(gameObject.transform.position, 2.5f, enemyLayers);

        foreach(Collider2D foe in explosion)
        {
//damidging enemies
            if(foe.transform.gameObject.tag == "Player"){
                PlayerHealth.TakeDamage(1, direction);
            }
        }

        yield return new WaitForSecondsRealtime(0.033f);

        anim.SetBool("Explode", false);
        Deactivate();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, 2.5f);
    }
}
