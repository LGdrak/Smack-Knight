using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SwordStateManager : MonoBehaviour
{
    public SwordBaseState currentState;
    public SwordState swordState = new SwordState();
    public CastState castState = new CastState();
    public HealState healState = new HealState();
    public LaserState laserState = new LaserState();

    public int souls;
    public int maxSouls = 25;
    public TextMeshProUGUI SoulText;
    public Slider slider;
    public GameObject[] _SoulBlast;

    public int spells = 0;
    public MoveControler move;
    SpellsUI uiSpells;
    GameObject Spell;
    int spellNum;
    bool BonusSouls = false;
    public Animator playerAnim;
    public GameObject cooldownCastObj;
    public GameObject cooldownHealObj;
    public GameObject cooldownLaserObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Spell = GameObject.FindWithTag("Spell");
        uiSpells = Spell.GetComponent<SpellsUI>();
        move = gameObject.transform.parent.GetComponent<MoveControler>();
        //starting state
        currentState = swordState;
// this = this exact script
        currentState.EnterState(this, _SoulBlast);
    }

    // Update is called once per frame
    void Update()
    {
        // manages soul UI and updates the update method
        slider.value = souls;
        SoulText.text = "Souls: " + souls.ToString();
        // manages cooldovn UI for the fire spell
        if(Time.time >= castState.NextCastTime){
            cooldownCastObj.SetActive(false);
        }
        if(Time.time >= healState.NextCastTime){
            cooldownHealObj.SetActive(false);
        }
        if(Time.time >= laserState.NextCastTime){
            cooldownLaserObj.SetActive(false);
        }
        currentState.UpdateState(this);
    }

// switches states
    public void SwitchState(SwordBaseState state){
        currentState = state;
        // manages UI
        ActiveSpellStateSend(state);
        state.EnterState(this, _SoulBlast);
    }

// apllies the stats from the trader script
    public void Damage(int _damage, float range, float attackRate, bool blocking, bool poison, int healtValue, int LaserDamageValue, int blastDamage, int MaxSouls, bool bonusSouls){
        // increases damage
        swordState.damage += _damage;

        //increases attack range
        swordState.attackRange += range;
        swordState.book.transform.localScale = new Vector3(swordState.attackRange, swordState.attackRange, swordState.attackRange);

        // incriases attack rate and limits it to a maximum of 0,1 s
        swordState.AttackRate -= attackRate;
        if (swordState.AttackRate < 0.1){swordState.AttackRate = 0.1f;}
        playerAnim.SetFloat("AttackSpeed", 1 / swordState.AttackRate); 

        // adds the blocking efect and makes sure it doesent get reset
        if (swordState.blocking != true){swordState.blocking = blocking;}

        // adds the poison efect and makes sure it doesent get reset
        if (swordState.poison != true){swordState.poison = poison;}

        // increases the amount of healing the healing spell will heal for
        healState.healtValue += healtValue;

        // increases laser damage
        laserState.LaserDamageValue += LaserDamageValue;

        // increases the damage of soul blast
        castState.blastDamage += blastDamage;

        // increases max souls capacity and translates it to UI
        maxSouls += MaxSouls;
        slider.maxValue = maxSouls;

        // adds the bonus souls efect and makes sure it doesent get reset
        if (BonusSouls != true){BonusSouls = bonusSouls;}
    }

// reciving souls from other scripts
    public void SoulExtraction(int soul){
        if (souls + soul <= maxSouls){
            if (BonusSouls)
            {
                souls += Random.Range(0,2);
            }
            souls += soul;
        }
        else{
            souls = maxSouls;
            // make some kind of animation so that the player knows that he cant have more souls
            }
    }

// a temporari way of unlocking spells
    public void EnableSpellSend(){
        if(spells < 4){
            spells ++;
            uiSpells.EnableSpell(spells);
        }
    }

// manages the UI of spells
    void ActiveSpellStateSend(SwordBaseState state){
        if(state == swordState){spellNum = 0;}
        else if(state == castState){spellNum = 2;}
        else if(state == healState){spellNum = 1;}
        else if(state == laserState){spellNum = 3;}

        uiSpells.ActiveSpellState(spellNum);
    }

// sends the collision for the laser to the laser state
    public void LaserSend(Collider2D laser){
        laserState.LaserDamage(laser);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, swordState.attackRange);
    }
}
