using UnityEngine;
using System.Collections;

public class Soul_Blast : MonoBehaviour
{
    [SerializeField] private float BulletSpeed;
    [SerializeField] public int blastDamage = 50;
    private CircleCollider2D boxCollider;
    private bool hit;
    private float lifetime;
    
    private Vector2 direction;

    public float attackRange;
    public LayerMask enemyLayers;
    public Animator anim;
    public bool animated = true;

    private void Awake()
    {
        boxCollider = GetComponent<CircleCollider2D>();
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
        if(lifetime > 5)
        {
            gameObject.SetActive(false);
        }
    }
// checking for collision and deactivating the bullet
    private void OnCollisionEnter2D(Collision2D other)
    {
        hit = true;
        boxCollider.enabled = false;
        StartCoroutine(Explode());
    }
    
// setting direction for the bullet and activating it
    public void SetDirection(Vector2 _direction, int damage_)
    {
        direction = _direction;
        blastDamage = damage_;

        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;
        lifetime = 0;

    }
// deactivating the bullet
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
    private IEnumerator Explode()
    {
        if (animated)
        {
            anim.SetBool("Explode", true);
        }
        
        yield return new WaitForSecondsRealtime(0.216f);

//Checking for collision with bulets and lowering hp
//Searching for and hitting enmies affected by the explosion
        Collider2D[] explosion = Physics2D.OverlapCircleAll(gameObject.transform.position, attackRange, enemyLayers);

        foreach(Collider2D foe in explosion)
        {
//damidging enemies
            try
            {
                foe.GetComponent<enemy>().TakeDamage(blastDamage, direction, false);
            }
            catch (System.Exception)
            {
                Debug.Log("not a damidgable enemy");
            }
        }

        yield return new WaitForSecondsRealtime(0.033f);

        if (animated)
        {
            anim.SetBool("Explode", false);
        }
        Deactivate();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, attackRange);
    }
}
