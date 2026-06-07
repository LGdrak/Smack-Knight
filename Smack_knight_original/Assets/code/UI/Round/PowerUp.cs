using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUp : MonoBehaviour
{
    public Trader trader_script;
    private GameObject player;
    public Transform equipped;
    SwordStateManager sword;
    public bool selected = false;
    private string name;
    [SerializeField] TMP_Text CostText;
    [SerializeField] int cost;
    public int index;

    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] int health;
        // the amount of healt that is healed with the heal spell
    [SerializeField] int healingtValue;
    [SerializeField] float range;
    [SerializeField] int chance;
    [SerializeField] bool regeneration;
    [SerializeField] bool blocking;
    [SerializeField] bool poison;
    [SerializeField] bool orbital;
    [SerializeField] bool orbitalShoot;
    [SerializeField] bool bonusSouls;
    [SerializeField] float attackRate;
    [SerializeField] int LaserDamageValue;
    [SerializeField] int blastDamage;
    [SerializeField] int MaxSouls;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        sword = player.transform.GetChild(0).gameObject.GetComponent<SwordStateManager>();

        name = gameObject.name;

        CostText.text = cost.ToString();
    }

    void OnTriggerEnter2D(Collider2D other){
        // selling the item for a set amount of souls and destroing it
        if(other.transform.gameObject.tag == "Player" && sword.souls >= cost){
            // Check if he has enough blood to pay
            sword.souls -= cost;
            BuyUpgrade();
        }
    }

    public void BuyUpgrade()
    {
        trader_script.Buy(damage, speed, health, range, chance, regeneration, attackRate, blocking, poison, healingtValue, LaserDamageValue, blastDamage, orbital, orbitalShoot, MaxSouls, bonusSouls, name);
        gameObject.SetActive(false);
        transform.SetParent(equipped);
    }
}
