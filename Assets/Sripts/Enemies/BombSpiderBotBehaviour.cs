using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombSpiderBotBehaviour : MonoBehaviour
{
    private GameObject player;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] private EnemyHealth eh;
    [SerializeField] private CapsuleCollider enemyColl;
    [SerializeField] private SkinnedMeshRenderer smr1;
    [SerializeField] private MeshRenderer smr2;

    [SerializeField] private ParticleSystem explosionVfx;
    [SerializeField] private GameObject explosionColl;
    [SerializeField] private GameObject explosionRange;

    public float timeUntilExplosion;

    public int bombDamage;


    private bool alreadyExploding = false;

    private GameObject currentTarget;

    [Space]
    public AudioClip tictac, boom;
    private AudioSource audioSource;

    // Upgrade
    bool upgraded = false;

    [SerializeField] private LayerMask m_LayerMask;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = gameObject.GetComponent<AudioSource>();

        currentTarget = player;

        eh.onDeath += Death;
    }

    private void Update()
    {
        if (agent.enabled)
            agent.SetDestination(currentTarget.transform.position);

    }

    public void SetTarget(GameObject go)
    {
        currentTarget = go;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            if (!alreadyExploding)
            {
                alreadyExploding = true;
                agent.speed *= 1.35f;

                Invoke(nameof(Death), timeUntilExplosion * 2.5f);

                explosionRange.SetActive(true);

                //Playing 'tictac'
                audioSource.clip = tictac;
                audioSource.loop = true;
                audioSource.Play();
            }

        }
    }

    private void Death()
    {
        if (!alreadyExploding)
        {
            alreadyExploding = true;
            eh.DropItems(); 
            TriggerExplosion();
        }
        else
        {
            CancelInvoke();
            agent.enabled = false;
            eh.DropItems();
            eh.enabled = false;
            Explode();
        }
    }

    private void TriggerExplosion()
    {
        agent.enabled = false;
        eh.enabled = false;
        explosionRange.SetActive(true);

        //Playing 'tictac'
        audioSource.clip = tictac;
        audioSource.loop = true;
        audioSource.Play();

        Invoke("Explode", timeUntilExplosion*=0.5f);
        enabled = false;
    }

    private void Explode()
    {
        GetComponent<CapsuleCollider>().enabled = false;
            Collider[] hitColliders = Physics.OverlapSphere(explosionColl.transform.position, explosionColl.GetComponent<SphereCollider>().radius * explosionColl.transform.localScale.x * transform.localScale.x, m_LayerMask, QueryTriggerInteraction.Ignore);
            foreach (var hc in hitColliders)
            {
                if (hc.tag == "Enemy")
                {
                    hc.GetComponent<EnemyHealth>().TakeDamage(bombDamage);
                }
                else if (hc.tag == "Player")
                {
                    hc.GetComponent<Health>().TakeDamage(bombDamage);
                }
            }
            explosionRange.SetActive(false);

            //Playing Booom
            audioSource.clip = boom;
            audioSource.loop = false;
            audioSource.Play();

            explosionVfx.Play();
            smr1.enabled = false;
            smr2.enabled = false;
            Destroy(gameObject, explosionVfx.main.duration);
    }

    public void UpgradeAttackSpeed()
    {
        if (!upgraded)
        {
            // TODO: Modify variable values to get enhanced version.
            upgraded = true;
        }
    }
}
