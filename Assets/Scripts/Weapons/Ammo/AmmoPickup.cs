using UnityEngine;
using System.Collections; // Necesario para usar Coroutines (IEnumerator)

public class AmmoPickup : MonoBehaviour
{
    [Header("Configuración de Munición Aleatoria")]
    [SerializeField] private int minAmmo = 20;
    [SerializeField] private int maxAmmo = 30;

    [Header("Configuración de Cooldown Aleatorio")]
    [SerializeField] private float minCooldown = 20f;
    [SerializeField] private float maxCooldown = 30f;

    // Referencias internas para ocultar la caja sin destruirla
    private MeshRenderer meshRenderer;
    private Collider boxCollider;
    private bool isAvailable = true;

    void Start()
    {
        // Guardamos los componentes automáticamente al iniciar
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Si la caja está en cooldown, no hacemos nada
        if (!isAvailable) return;

        // Buscamos si el objeto que pisó la caja tiene el WeaponManager
        WeaponManager manager = other.GetComponent<WeaponManager>();

        if (manager != null && manager.weaponHolder != null)
        {
            // Buscamos el arma que el jugador tiene activa en ese instante en sus manos
            BaseWeapon activeWeapon = manager.weaponHolder.GetComponentInChildren<BaseWeapon>();

            if (activeWeapon != null)
            {
                // 1. Calculamos una cantidad de munición ALEATORIA entre 20 y 30
                // Nota: maxAmmo + 1 porque Random.Range con enteros excluye el número máximo
                int randomAmmo = Random.Range(minAmmo, maxAmmo + 1);

                // 2. Le inyectamos la munición a la reserva del arma
                activeWeapon.AddAmmo(randomAmmo);
                
                // 3. Activamos el sistema de Cooldown (Desaparece temporalmente)
                StartCoroutine(CooldownCoroutine());
            }
        }
    }

    private IEnumerator CooldownCoroutine()
    {
        isAvailable = false;

        // Desactivamos el colisionador y el renderizado visual (el objeto sigue existiendo, pero es invisible e intangible)
        if (boxCollider != null) boxCollider.enabled = false;
        if (meshRenderer != null) meshRenderer.enabled = false;

        // Calculamos un tiempo de espera ALEATORIO entre 20 y 30 segundos
        float randomCooldown = Random.Range(minCooldown, maxCooldown);
        Debug.Log($"Caja de munición recogida. Reapareciendo en {randomCooldown:F1} segundos...");

        // Esperamos ese tiempo exacto
        yield return new WaitForSeconds(randomCooldown);

        // Volvemos a activar la caja para que el jugador pueda usarla otra vez
        if (boxCollider != null) boxCollider.enabled = true;
        if (meshRenderer != null) meshRenderer.enabled = true;
        
        isAvailable = true;
        Debug.Log("¡La caja de munición ha reaparecido en el suelo!");
    }
}