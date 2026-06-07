using UnityEngine;

public class MoveControler : MonoBehaviour
{
    public Vector2 currentDirection;
    public int currentDamage;
    public Vector2 direction;
    private bool active;
    public bool knockbackNoHitBool;

    public UIHeart uiHeart;

    public GameObject dashObj;

    public AudioSource Hurt;
    private bool regen = false;

// declaring all if the states
    MoveDefaultState currentState;
    public WalkState walkState = new WalkState();
    public HurtState hurtState = new HurtState();
    public DashState dashState = new DashState();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // seting knockback bool up
        knockbackNoHitBool = false;
        //starting state
        currentState = walkState;
// this = this exact script
        currentState.EnterState(this, currentDirection);
    }

    // Update is called once per frame
    void Update()
    {
        // updates the update method of the current state
        currentState.UpdateState(this);
    }

    void FixedUpdate()
    {
        // updates the FIxed update method of the current state
        currentState.FixedUpdateState(this);

// constrains the player inside the arena so he can't get out
        if (active)
        {
            if (transform.position.y <= -7.8f)
            {
                transform.position = new Vector2(transform.position.x, -7.8f);
            }
            else if (transform.position.y >= 51.584f)
            {
                transform.position = new Vector2(transform.position.x, 51.584f);
            }

            if (transform.position.x <= -59.565f)
            {
                transform.position = new Vector2(-59.565f, transform.position.y);
            }
            else if (transform.position.x >= 60f)
            {
                transform.position = new Vector2(60f, transform.position.y);
            }
            
        }
    }

// switches states
    public void SwitchState(MoveDefaultState state){
        currentDirection = currentState.ExitState(this);
        currentState = state;
        state.EnterState(this, currentDirection);
    }
    
    // damages the player
    public void TakeDamage(int damage, Vector2 _direction){
        if (knockbackNoHitBool == false)
        {
            currentDamage = damage;
            direction = _direction;
            SwitchState(hurtState);
        }
    }

// applais the stats send by the trader script
    public void Stats(float _speed, int _health){
        walkState.MoveSpeed += _speed;
        if(walkState.MoveSpeed < 1){walkState.MoveSpeed = 1;}
        hurtState.currentHealth += _health;
        uiHeart.HealthUp(_health, _health);
    }

// heals the player and manages UI heart
    public void HealControl(int _health){
        hurtState.currentHealth += _health;

        if(hurtState.currentHealth <= 0){
            SwitchState(hurtState);
        }

        if(hurtState.currentHealth > uiHeart.maxHealth){
            hurtState.currentHealth = uiHeart.maxHealth;
        }
 
        uiHeart.Heal(_health);
    }

// activates the boundaries of the arena (defined in the fixed update)
    public void Boundaries(bool set)
    {
        active = set;
    }

    public void Regeneration()
    {
        InvokeRepeating("Regenerate", 0f, 30f);
        regen = true;
    }

    private void Regenerate()
    {
        hurtState.currentHealth += 1;
         
        if(hurtState.currentHealth > uiHeart.maxHealth){
            hurtState.currentHealth = uiHeart.maxHealth;
        }
 
        uiHeart.Heal(1);
    }

    public void EndRoundRegen()
    {
        if (regen)
        {
            uiHeart.Heal(uiHeart.maxHealth - hurtState.currentHealth);
            hurtState.currentHealth = uiHeart.maxHealth;
        }
    }
}
