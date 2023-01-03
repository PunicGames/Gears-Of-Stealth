using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public int scoreValue = 10;

    public int minScoreValue;
    public int maxScoreValue;

    [SerializeField]
    private GameObject coin;

    CapsuleCollider collider;
    public bool isDead;
    private Animator animator;
    private float timeUntilDestroy = 2f;

    private PopUp popup;
    [SerializeField] private Transform popupPosition;

    public delegate void TakeDamageDel();

    public TakeDamageDel takeDamage;

    //Items Consumibles
    [SerializeField] private float itemsRatio;
    [SerializeField] private GameObject[] items;

    // Enemy Type
    public enum EnemyType { WORKER, GUNSLINGER, GUNNER, EXPLOSIVE_SPIDERBOT, WORKER_SPIDERBOT, ATTACK_SPIDERBOT, FOREMAN }
    public EnemyType enemyType;
    public EnemySoundManager enemySoundManager;

    // Player reference
    private GameObject playerRef;

    //Delegates
    public delegate void OnDeath();
    public event OnDeath onDeath;

    // Particles
    [SerializeField] private ParticleSystem Cure;

    //private int timeAnimationDead = 1;
    private void Awake()
    {
        collider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        currentHealth = startingHealth;
        enemySoundManager = GetComponent<EnemySoundManager>();
        //Debug.Log("Health: " + currentHealth);

        popup = GetComponent<PopUp>();
    }
    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        playerRef.GetComponent<Player>().PlayHitMarker();

        takeDamage?.Invoke();

        popup.Create(popupPosition.position, amount, PopUp.TypePopUp.DAMAGE, false, 0.5f);

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        
        enemySoundManager.PauseSound("walk");
        enemySoundManager.PlaySound("death");

        enemySoundManager.UnSuscribe();
        isDead = true;
        collider.enabled = false;

        if (enemyType == EnemyType.EXPLOSIVE_SPIDERBOT)
        {
            
            CountStats();
            onDeath?.Invoke();
            return;
        }
        else if (enemyType == EnemyType.GUNSLINGER)
        {
            onDeath?.Invoke();
        }
        

        NavMeshAgent navMov = GetComponent<NavMeshAgent>();
        if (navMov != null)
            navMov.enabled = false;

        //Look for a box collider in case its a melee enemy and deactivate
        BoxCollider coll = GetComponent<BoxCollider>();
        if (coll)
            coll.enabled = false;


        //NavMeshAgent navMov = GetComponent<NavMeshAgent>();
        EnemyMovement eM = GetComponent<EnemyMovement>();
        WorkerBehavior mE = GetComponent<WorkerBehavior>();
        GunnerBehaviour gb = GetComponent<GunnerBehaviour>();
        RangedEnemy rE = GetComponent<RangedEnemy>();
        ProspectorBehaviour pb = GetComponent<ProspectorBehaviour>();
        if (eM != null)
            eM.enabled = false;
        if (rE != null)
            rE.enabled = false;
        if (mE != null)
            mE.enabled = false;
        //if (navMov != null)
        //   navMov.enabled = false;
        if (pb != null)
            pb.enabled = false;
        if (gb)
        {

            gb.enabled = false;

        }

        // Faltan sonidos y animaciones de muerte etc etc

        int r = Random.Range(0, 2);
        //Randomly choose death animation type
        switch (r)
        {
            case 0:
                animator.SetFloat("death_type", 0);
                break;
            case 1:
                animator.SetFloat("death_type", .5f);
                break;
            case 2:
                animator.SetFloat("death_type", 1);
                break;

        }
        animator.SetTrigger("death");


        DropItems();
        CountStats();

    }
    //Autamitacally call when death animation ended
    public void DestroyCallback()
    {
        Destroy(gameObject, timeUntilDestroy);
    }

    public void Heal(int hp)
    {
        int hpHealed = (hp > startingHealth - currentHealth) ? startingHealth - currentHealth : hp;
        popup.Create(popupPosition.position, hpHealed, PopUp.TypePopUp.LIFE, true, 0.5f);
        currentHealth += hpHealed;
        if (Cure != null)
        {
            Cure.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // En caso de error si el enemigo aparece dentro de una tienda o un objeto. No deber�a de ocurrir igualmente.
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Shop")
        {
            Debug.Log("ENEMIGO APARECE DENTRO DE OBJETO. ELIMINADO");
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "DeathPlane")
        {
            Debug.Log("ENEMIGO CAE AL VAC�O. ELIMINADO");
            Destroy(gameObject);
        }
    }

    public void DropItems()
    {
        // Suelta moneda
        //GameObject moneda = Instantiate(coin, transform.position, Quaternion.identity);
        //moneda.gameObject.GetComponent<Moneda>().value = scoreValue;
        //moneda.gameObject.GetComponent<Moneda>().value = Random.Range(minScoreValue, maxScoreValue);

        //Soltar items consumibles

        float aux = Random.Range(0f, 1f);
        if (aux <= itemsRatio)
        {
            int index = Random.Range(0, items.Length);
            //GameObject item = Instantiate(items[index], transform.position + new Vector3(0.8f, 0f, -0.8f), Quaternion.identity);
        }
    }

    private void CountStats()
    {
        // Add stats to player
        playerRef.GetComponent<PlayerStats>().numDefeatedEnemies++;
    }

}
