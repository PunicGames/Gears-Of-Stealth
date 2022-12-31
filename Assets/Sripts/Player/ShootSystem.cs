using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShootSystem : MonoBehaviour
{
    // Weapon personalization
    [SerializeField] private GameObject[] weapon_meshes;
    [SerializeField] private Transform[] weapon_origins;
    [SerializeField] public PlayerRigBehaviour rig;



    // Guns
    [HideInInspector] public PlayerGuns guns;
    [SerializeField] private AudioClip[] reloadAudioClip;
    [HideInInspector] public int selectedGun = 0;
    public bool[] availableGuns;

    [SerializeField] private Sprite[] weaponSprites;

    // Guns in Order: pistol, subfusil, rifle, sniper, shotgun


    // Bullet
    public GameObject bullet;
    public GameObject laserBullet;

    //LaserBullet
    public bool canBeStoppedByWalls = true;
    public int numberOfCollisions = 1;



    // Action control
    public bool shooting;
    bool readyToShoot, reloading;

    // Shoot Controller
    public bool allowInvoke = true;

    // Audio
    AudioManager audioManager;

    // Display
    private TextMeshProUGUI ammunitionDisplay;
    private GameObject rechargingDisplay;
    private Image weaponDisplay;

    //Perks Modifies
    //[HideInInspector]
    public bool laserShot;
    [HideInInspector]
    public bool bigShot;
    [HideInInspector]
    public float scaleFactor = 1f;

    // Animator reference
    private Animator anim;

    // Cursor sprites
    [SerializeField] private Texture2D[] cursorSprites;
    private Vector2 cursorHotSpot;

    // Platform control
    private bool desktop;

    // Bullet colors
    [SerializeField] private Color albedo;
    [SerializeField] private Color emissive;

    // Delegates
    public delegate void OnShootWeapon(bool t);
    public OnShootWeapon onShootWeapon;

    public delegate void OnSwapWeapon(Vector3 p);
    public OnSwapWeapon onSwapWeapon;

    // Player Statistics
    [SerializeField] private PlayerStats playerStats;

    //Perks
    [SerializeField] private PerksManager perks;

    private void Awake()
    {
        // Platform
        if (Application.isMobilePlatform)
        {
            desktop = false;
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            desktop = true;
        }

        //desktop = false;

        // Guns initialization
        guns = new PlayerGuns();
        availableGuns = new bool[guns.getGuns().Length];
        // La pistola, que ocupa la primera posición, siempre podrá ser accesible.
        availableGuns[0] = true;
       

    }

    private void Start()
    {

        // Inicializacion de variables
        readyToShoot = true;
        laserShot = false;

        // Inicializacion de componentes
        audioManager = transform.GetComponent<AudioManager>();
        anim = GetComponentInParent<Animator>();

        // Display initialization
        ammunitionDisplay = GameObject.Find("Municion").GetComponent<TextMeshProUGUI>();
        rechargingDisplay = GameObject.Find("Recargando");
        rechargingDisplay.SetActive(false);

        weaponDisplay = GameObject.Find("CurrentWeapon").GetComponent<Image>();
        weaponDisplay.sprite = weaponSprites[0];

        // Display cursor
        //if (desktop) { 
        //    cursorHotSpot = new Vector2(cursorSprites[selectedGun].width / 2, cursorSprites[selectedGun].height / 2);
        //    Cursor.SetCursor(cursorSprites[selectedGun], cursorHotSpot, CursorMode.ForceSoftware);
        //}

        // Init guns in mobile
        if (!desktop)
        {
            InitGunsMobile();
        }
       

    }

    void Update()
    {
        // Recarga automáticamente si no quedan balas
        if (readyToShoot && shooting && !reloading && guns.getGuns()[selectedGun].bulletsLeftInMagazine <= 0) Reload();

        if (ammunitionDisplay != null)
        {
            if (selectedGun != 0)
                ammunitionDisplay.text = (guns.getGuns()[selectedGun].bulletsLeftInMagazine + "/" + guns.getGuns()[selectedGun].totalBullets);
            else // En caso de ser la pistola
                ammunitionDisplay.text = guns.getGuns()[selectedGun].bulletsLeftInMagazine + "/9999";

        }
    }

    public void Shooting()
    {
        // Comprueba si se puede disparar
        if (readyToShoot && shooting && !reloading && (guns.getGuns()[selectedGun].bulletsLeftInMagazine) > 0)
        {
            guns.getGuns()[selectedGun].bulletsShot = 0;
            anim.SetTrigger("shoot");
            Shoot();
        }
    }

    private void Shoot()
    {
        if (shooting)
        { // Shooting ayuda para controlar las balas de las armas automáticas de la función Invoke del final de este método. Evita que se disparen balas indeseadas
            readyToShoot = false;


            // Se calcula la dirección y origen del disparo
            Vector3 origin = weapon_origins[selectedGun].position;
            //Vector3 direction = (directionAim - origin).normalized;// weapon_origins[selectedGun].forward;
            Vector3 direction = weapon_origins[selectedGun].forward;

            if (selectedGun != 4)
            {
                ShootBullet(origin, direction.normalized);
            }
            else
            {
                //Escopeta
                int numBulletsAtTime = 8;

                float startAngle = 60 * 0.5f;
                float partialAngle = 60 * 0.14f; //equal as divide by 3 but faster

                for (int i = 0; i < numBulletsAtTime; i++)
                {
                    Vector3 directionWithSpread = Quaternion.AngleAxis(startAngle, transform.up) * transform.forward;
                    ShootBullet(origin, directionWithSpread.normalized);
                    startAngle -= partialAngle;
                }
            }
            guns.getGuns()[selectedGun].bulletsLeftInMagazine--;
            guns.getGuns()[selectedGun].bulletsShot++;
            anim.SetTrigger("shoot");


            if (allowInvoke)
            {
                Invoke("ResetShot", guns.getGuns()[selectedGun].timeBetweenShooting); // Llama a la función después de timeBetweenShooting segundos
                allowInvoke = false;
            }


            if ((guns.getGuns()[selectedGun].automaticGun && (guns.getGuns()[selectedGun].bulletsLeftInMagazine > 0)))
            { // Si es un arma automática, sigue disparando
               
                Invoke("Shoot", guns.getGuns()[selectedGun].timeBetweenShots);
            }
        }
    }

    public void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    public void Reload()
    { // Llamar función cuando jugador pulsa R
        //Debug.Log("Intenta recargar");
        if ((guns.getGuns()[selectedGun].bulletsLeftInMagazine < guns.getGuns()[selectedGun].magazineSize) && !reloading && guns.getGuns()[selectedGun].totalBullets > 0)
        {
            shooting = false;
            audioManager.PlaySecundary(selectedGun);
            reloading = true;
            rechargingDisplay.SetActive(true);

            rig.ActivateRRig(false);
            rig.ActivateLRig(false);
            if (selectedGun == 0)
                anim.SetBool("isPistol", true);
            else anim.SetBool("isPistol", false);
            anim.SetTrigger("Reload");

            Invoke("ReloadFinished", guns.getGuns()[selectedGun].reloadTime);
        }
    }

    private void ReloadFinished()
    {

        int bulletsInMagazine = guns.getGuns()[selectedGun].bulletsLeftInMagazine;
        if (bulletsInMagazine + guns.getGuns()[selectedGun].totalBullets >= guns.getGuns()[selectedGun].magazineSize)
        {
            guns.getGuns()[selectedGun].bulletsLeftInMagazine = guns.getGuns()[selectedGun].magazineSize;
            guns.getGuns()[selectedGun].totalBullets -= (guns.getGuns()[selectedGun].magazineSize - bulletsInMagazine);
        }
        else
        {
            guns.getGuns()[selectedGun].bulletsLeftInMagazine = guns.getGuns()[selectedGun].totalBullets + bulletsInMagazine;
            guns.getGuns()[selectedGun].totalBullets = 0;
        }
        if (selectedGun > 1)
        {
            rig.ActivateLRig(true, 1);
            rig.ActivateRRig(true, 0.8f, 1);
        }
        else
            rig.ActivateRRig(true, 0.4f, 0.6f);
        reloading = false;
        rechargingDisplay.SetActive(false);
        Shooting(); // Llamamos a esta funcion en caso de que el jugador siga con el click de ratón pulsado, empiece a disparar
    }

    public void SwapGun()
    {
        // Explora las opciones de armas en orden. Hay dos bucles ya que podriamos estar posicionados en el arma número 1,
        // por lo que explora primero hacia arriba y luego da la vuelta y explora las que jerárquicamente están por debajo

        for (int i = selectedGun + 1; i < availableGuns.Length; i++)
        {
            if (availableGuns[i])
            {
                weapon_meshes[selectedGun].SetActive(false);
                weapon_meshes[i].SetActive(true);


                selectedGun = i;
                if (weaponDisplay != null)
                {
                    weaponDisplay.sprite = weaponSprites[selectedGun];
                }

                //if (desktop)
                //    Cursor.SetCursor(cursorSprites[selectedGun], cursorHotSpot, CursorMode.ForceSoftware);

                // Se resetea el disparo para quitar el enfriamiento del anterior arma y poder disparar de inmediato (aunque debería llamarse mejor al finalizar la animación)
                ResetShot();
                // AQUI VA LA ANIMACIÓN DE CAMBIO DE ARMA (TENER EN CUENTA LO QUE PONE 2 LINEAS MÁS ARRIBA)
                if (selectedGun > 1)
                {
                    anim.SetBool("isRifle", true);
                    rig.SetRWeight(1);
                    rig.ChangeRightTargetRigPos(1);
                    rig.ActivateLRig(true);
                }
                else
                {
                    anim.SetBool("isRifle", false);
                    rig.SetRWeight(0.6f);
                    rig.ChangeRightTargetRigPos(0);
                    rig.ActivateLRig(false);

                }

                onSwapWeapon.Invoke(weapon_origins[selectedGun].localPosition);

                return;
            }
        }

        for (int i = 0; i < selectedGun; i++)
        {
            if (availableGuns[i])
            {
                weapon_meshes[selectedGun].SetActive(false);
                weapon_meshes[i].SetActive(true);
                selectedGun = i;

                if (weaponDisplay != null)
                {
                    weaponDisplay.sprite = weaponSprites[selectedGun];
                }

                //if (desktop)
                //    Cursor.SetCursor(cursorSprites[selectedGun], cursorHotSpot, CursorMode.ForceSoftware);

                ResetShot();
                // AQUI VA LA ANIMACIÓN DE CAMBIO DE ARMA
                if (selectedGun > 1)
                {
                    anim.SetBool("isRifle", true);
                    rig.SetRWeight(1);
                    rig.ChangeRightTargetRigPos(1);
                    rig.ActivateLRig(true);
                }
                else
                {
                    anim.SetBool("isRifle", false);
                    rig.SetRWeight(0.6f);
                    rig.ChangeRightTargetRigPos(0);
                    rig.ActivateLRig(false);
                }

                onSwapWeapon.Invoke(weapon_origins[selectedGun].localPosition);
                return;
            }
        }

    }

    public void ActivateGun(int idx)
    {
        availableGuns[idx] = true;
    }

    public void DeactivateGun(int idx)
    {
        availableGuns[idx] = false;
    }

    public void ChangeCursorBack()
    {
        //if (desktop)
        //    Cursor.SetCursor(cursorSprites[selectedGun], cursorHotSpot, CursorMode.ForceSoftware);
    }

    private void InitGunsMobile()
    {
        for (int i = 0; i < guns.getGuns().Length; i++)
        {
            guns.getGuns()[i].automaticGun = false;
        }
    }

    public void addAmmoToWeapons()
    {
        for (int i = 0; i < guns.getGuns().Length; i++)
        {
            var sg = guns.getGuns()[i];

            if (availableGuns[i])
            {
                sg.totalBullets += sg.magazineSize * 2;

            }
        }
    }

    private void ShootBullet(Vector3 origin, Vector3 dir)
    {
        if (laserShot)
        {
            GameObject currentBullet = Instantiate(laserBullet, origin, Quaternion.identity);
            currentBullet.transform.forward = dir;
            Bullet bulletParams = currentBullet.GetComponent<Bullet>();
            bulletParams.SetForce(dir, guns.getGuns()[selectedGun].shootForce);
            bulletParams.SetDamage(guns.getGuns()[selectedGun].bulletDamage);
            bulletParams.SetLaser(true);
            bulletParams.SetPlayerStats(playerStats);
            bulletParams.numberOfCollisions = perks.perkLevels[1] + 2;
            if (perks.perkLevels[1] == 4) bulletParams.stoppedByWalls = false;
            bulletParams.timeToDestroy = guns.getGuns()[selectedGun].bulletLifeTime;
            bulletParams.owner = Bullet.BulletOwner.PLAYER;
            //bulletParams.SetBulletColors(albedo, emissive);
            currentBullet.transform.localScale *= scaleFactor;
            audioManager.PlayLaser(selectedGun);
            onShootWeapon.Invoke(true);
        }
        else
        {
            GameObject currentBullet = Instantiate(bullet, origin, Quaternion.identity);
            currentBullet.transform.forward = dir;
            Bullet bulletParams = currentBullet.GetComponent<Bullet>();
            bulletParams.SetForce(dir, guns.getGuns()[selectedGun].shootForce);
            bulletParams.SetDamage(guns.getGuns()[selectedGun].bulletDamage);
            bulletParams.SetLaser(false);
            bulletParams.SetPlayerStats(playerStats);
            bulletParams.timeToDestroy = guns.getGuns()[selectedGun].bulletLifeTime;
            bulletParams.owner = Bullet.BulletOwner.PLAYER;
            bulletParams.SetBulletColors(albedo, emissive);
            currentBullet.transform.localScale *= scaleFactor;
            audioManager.Play(selectedGun);
            onShootWeapon.Invoke(false);
        }

    }
}
