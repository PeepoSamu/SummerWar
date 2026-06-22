using UnityEngine;
using System.Collections; // Necesario para usar IEnumerator (la recarga con tiempo)

public abstract class BaseWeapon : MonoBehaviour
{
    [HideInInspector] public WeaponData data;

    [Header("Precision & ADS Settings")]
    [SerializeField] protected float hipSpread = 0.08f;
    [SerializeField] protected float adsSpread = 0.01f;
    [SerializeField] protected Vector3 hipPosition;
    [SerializeField] protected Vector3 adsPosition;
    [SerializeField] protected float adsSpeed = 12f;

    [Header("Recoil Settings")]
    [SerializeField] protected float recoilForce = 2f;
    [SerializeField] protected float recoilReturnSpeed = 5f;

    [Header("References")]
    public Transform muzzle;
    public GameObject bulletPrefab; // ¡Aquí asignarás tu bolita de pintura!

    // Variables de estado dinámicas
    protected float damage;
    protected float fireRate;
    protected int magazineSize;
    
    // --- NUEVAS VARIABLES DE MUNICIÓN ---
    [Header("Ammo Status (Sólo lectura)")]
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected int reserveAmmo;
    [SerializeField] protected int maxReserveAmmo;
    protected bool isReloading = false;

    protected float nextShoot;
    protected bool isAiming;
    private Quaternion originalRotation;

    protected virtual void Start()
    {
        if (data != null)
        {
            damage = data.Damage;
            fireRate = data.FireRate;
            magazineSize = data.MagazineSize;
        }

        // Configuración inicial de la munición (Cargador x 3 en reserva)
        currentAmmo = magazineSize;
        maxReserveAmmo = magazineSize * 3; 
        reserveAmmo = maxReserveAmmo; 

        originalRotation = transform.localRotation;
    }

    public void SetADS(bool isAiming)
    {
        this.isAiming = isAiming; 
    }   

    protected virtual void Update()
    {
        HandleADSVisual();
        HandleRecoilRecovery();
    }

    private void HandleADSVisual()
    {
        Vector3 targetPosition = isAiming ? adsPosition : hipPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * adsSpeed);
    }

    private void HandleRecoilRecovery()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, Time.deltaTime * recoilReturnSpeed);
    }

    public virtual void Shoot()
    {
        // CORRECCIÓN: Si está recargando o no tiene balas, bloqueamos el disparo
        if (isReloading || Time.time < nextShoot || currentAmmo <= 0) return;

        nextShoot = Time.time + fireRate;
        currentAmmo--;

        float currentSpread = isAiming ? adsSpread : hipSpread;
        Vector3 spreadOffset = (muzzle.right * Random.Range(-currentSpread, currentSpread)) + 
                               (muzzle.up * Random.Range(-currentSpread, currentSpread));
        
        Vector3 finalDirection = (muzzle.forward + spreadOffset).normalized;

        if (bulletPrefab != null && muzzle != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(finalDirection));
            
            Projectile projectileScript = bullet.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                // MODIFICACIÓN: Bajamos la velocidad de 120f a 35f para que parezca una bola de pintura real
                // Al bajar la velocidad, verás de forma espectacular cómo la bola cae por la gravedad física
                projectileScript.Initialize(damage, 35f, 1f, 4f);
            }
        }

        ApplyVisualRecoil();
    }

    private void ApplyVisualRecoil()
    {
        float pitch = -recoilForce;
        float yaw = Random.Range(-recoilForce * 0.3f, recoilForce * 0.3f);
        transform.localRotation *= Quaternion.Euler(pitch, yaw, 0);
    }

    // --- NUEVA LÓGICA DE RECARGA REAL ---
    public virtual void StartReload()
    {
        // Si ya está recargando, si el cargador está lleno, o si no hay reservas, no hace nada
        if (isReloading || currentAmmo == magazineSize || reserveAmmo <= 0) return;

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log($"Recargando {gameObject.name}... Esperando {data.ReloadTime} segundos.");

        // Espera el tiempo de recarga configurado en tu WeaponData ScriptableObject
        yield return new WaitForSeconds(data != null ? data.ReloadTime : 2.0f);

        int ammoNeeded = magazineSize - currentAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        isReloading = false;
        Debug.Log($"Recarga completa. Balas: {currentAmmo} | Reserva restante: {reserveAmmo}");
    }

    // --- MÉTODO PARA RECOGER MUNICIÓN DEL SUELO ---
    public void AddAmmo(int amount)
    {
        // Añade balas sin pasarse del límite de cargador x3
        reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
        Debug.Log($"¡Munición añadida! Reserva actual: {reserveAmmo}/{maxReserveAmmo}");
    }
}