using UnityEngine;

public class orbitShoot : MonoBehaviour
{

    [SerializeField] GameObject[] SoulBlast;
    Vector2 direction;
    public LayerMask enemyLayers;
    public float attackRange;
    private float distance = 100000f;
    bool shoot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("Cast", 0f, 3f);
    }

    void Cast()
    {
        shoot = true;
        FindEnemy();

// pulling a bullet to the shooting position and setting its direction
        if (shoot == true){

            SoulBlast[FindSoulBlast()].transform.position = transform.position;
            SoulBlast[FindSoulBlast()].GetComponent<Soul_Blast>().SetDirection(direction, 20);
        }
        
    }

    private void FindEnemy()
    {
        Collider2D[] search = Physics2D.OverlapCircleAll(gameObject.transform.position, attackRange, enemyLayers);

        if (search.Length == 0){shoot = false;}
        Debug.Log(shoot);

        foreach(Collider2D foe in search)
        {
            try
            {
                // tries if the enemy can be damidged, if yes, asighns it to direction
                if (Vector2.Distance(foe.gameObject.transform.position, transform.position) < distance)
                {
                    direction = foe.GetComponent<enemy>().gameObject.transform.position - transform.position;
                    direction = direction.normalized;
                    distance = Vector2.Distance(foe.gameObject.transform.position, transform.position);
                }
                
            }
            catch (System.Exception)
            {
                Debug.Log("not a damidgable enemy");
            }
        }
        distance = 100000f;
    }

// finds a bullet not being used to fire
    private int FindSoulBlast()
    {
        for (int i = 0; i < SoulBlast.Length; i++)
        {
            if (!SoulBlast[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, attackRange);
    }
}
