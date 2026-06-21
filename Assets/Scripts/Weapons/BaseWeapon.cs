using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    // Mantenemos oculta la data del ScriptableObject, pero accesible para herencia
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
    public GameObject bulletPrefab;

    // Variables de estado dinámicas (Viven en la instancia, no en el ScriptableObject)
    protected float damage;
    protected float fireRate;
    protected int magazineSize;
    protected int currentAmmo;

    protected float nextShoot;
    protected bool isAiming;
    private Quaternion originalRotation;

    protected virtual void Start()
    {
        // Al arrancar, si tenemos un ScriptableObject asignado por el manager, jalamos sus datos base
        if (data != null)
        {
            damage = data.Damage;
            fireRate = data.FireRate;
            magazineSize = data.MagazineSize;
        }

        currentAmmo = magazineSize;
        originalRotation = transform.localRotation;
    }

    // EN BASEWEASON.CS

    // El veterano usa SetADS porque define perfectamente la mecánica de un FPS
    public void SetADS(bool isAiming)
    {
        // Si usas el código optimizado, esto activa la transición de la mira (Lerp)
        this.isAiming = isAiming; 
    }   

    protected virtual void Update()
    {
        HandleADSVisual();
        HandleRecoilRecovery();
    }

    // Gestiona el movimiento físico de la mira en pantalla
    private void HandleADSVisual()
    {
        Vector3 targetPosition = isAiming ? adsPosition : hipPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * adsSpeed);
    }

    // Hace que el arma vuelva a su posición original gradualmente tras disparar
    private void HandleRecoilRecovery()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, Time.deltaTime * recoilReturnSpeed);
    }

    public virtual void Shoot()
    {
        if (Time.time < nextShoot || currentAmmo <= 0) return;

        nextShoot = Time.time + fireRate;
        currentAmmo--;

        // 1. Cálculo de dispersión profesional (Multiplicando por los ejes locales del cañón)
        float currentSpread = isAiming ? adsSpread : hipSpread;
        Vector3 spreadOffset = (muzzle.right * Random.Range(-currentSpread, currentSpread)) + 
                               (muzzle.up * Random.Range(-currentSpread, currentSpread));
        
        Vector3 finalDirection = (muzzle.forward + spreadOffset).normalized;

        // 2. Instanciar proyectil
        if (bulletPrefab != null && muzzle != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(finalDirection));
            
            // Sincronización con tu script 'Projectile' que ya tenías creado
            Projectile projectileScript = bullet.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                // Pasamos: Daño, Velocidad, Gravedad, Tiempo de vida (ej: 3 segundos)
                projectileScript.Initialize(damage, 120f, 1f, 3f);
            }
        }

        // 3. Aplicar retroceso visual al arma
        ApplyVisualRecoil();
    }

    private void ApplyVisualRecoil()
    {
        // Rompemos levemente la rotación hacia arriba y un lado aleatorio
        float pitch = -recoilForce;
        float yaw = Random.Range(-recoilForce * 0.3f, recoilForce * 0.3f);

        transform.localRotation *= Quaternion.Euler(pitch, yaw, 0);
    }

    public virtual void StartReload()
    {
        Debug.Log($"Recargando {gameObject.name}...");
        currentAmmo = magazineSize; // Lógica básica por ahora
    }
}