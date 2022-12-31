using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grenade : MonoBehaviour
{

    public Vector3 target;
    public Vector3 origin;
    public float timeUntilExplosion = 1f;
    public int damage = 40;
    private float anim;

    private float speed = 0.6f;
    private bool animEnded = false;

    [SerializeField] ParticleSystem vfx;
    [SerializeField] GameObject explosionRange;
    [SerializeField] GameObject bombMesh;
    [SerializeField] GameObject Light;

    [Header("SFX")]

    Collider[] enemiesAware;

    public AudioClip tictac;
    public AudioClip boom;
    private AudioSource audioSource;

    [SerializeField] private LayerMask layerMask;

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        origin = transform.position;
        target = new Vector3(target.x, target.y + 0.1f, target.z);
        audioSource = gameObject.GetComponent<AudioSource>();
        Destroy(gameObject, 5);
    }

    void Update()
    {
        if (animEnded)
            return;
        anim += Time.deltaTime;
        transform.position = MathParabola.Parabola(origin, target, 3.8f, anim / speed);

        if (transform.position.y <= target.y + 0.1f)
        {
            //Debug.Log("reached");
            animEnded = true;
            transform.position = new Vector3(transform.position.x, 0.08f, transform.position.z);
            TriggerExplosion();

        }
    }
    private void TriggerExplosion()
    {
        explosionRange.GetComponent<MeshRenderer>().enabled = true;
        
        //Playing 'tictac'
        audioSource.clip = tictac;
        audioSource.loop = true;
        audioSource.Play();
        enemiesAware = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius * transform.localScale.x, layerMask, QueryTriggerInteraction.Ignore); ;
       

        Invoke("Explode", timeUntilExplosion);
        enabled = false;
    }

    private void Explode()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius  * transform.localScale.x, layerMask, QueryTriggerInteraction.Ignore); ;
        foreach (var hc in hitColliders)
        {

            if (hc.tag == "Enemy")
            {
                hc.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
            else if (hc.tag == "Player")
            {
                hc.GetComponent<Health>().TakeDamage(damage);
            }

        }
        explosionRange.SetActive(false);

        bombMesh.SetActive(false);
        Light.SetActive(false);
        
        //Playing Booom
        audioSource.clip = boom;
        audioSource.loop = false;
        audioSource.Play();

        vfx.Play();
       
        Destroy(gameObject, vfx.main.duration);
    }
    public void setExplosionRatio(float r)
    {
        transform.localScale *= r;
    }
}
