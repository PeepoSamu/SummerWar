using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform weaponHolder;
    public PlayerLoadout loadout;

    private BaseWeapon primaryInstance;
    private BaseWeapon secondaryInstance;
    private BaseWeapon currentWeapon;

    void Start()
    {
        InitializeWeapons();
        EquipPrimary();
    }

    // Cambiado a public para que WeaponPickup pueda reinicializar las armas si recoges una nueva
    public void InitializeWeapons()
    {
        // Limpieza de armas previas antes de spawnear las nuevas (evita duplicados al recoger del suelo)
        foreach (Transform child in weaponHolder)
        {
            Destroy(child.gameObject);
        }

        primaryInstance = null;
        secondaryInstance = null;

        // Spawneamos la primaria si existe y tiene un prefab asignado
        if (loadout != null && loadout.primary != null && loadout.primary.WeaponPrefab != null)
        {
            GameObject wp = Instantiate(loadout.primary.WeaponPrefab, weaponHolder);
            primaryInstance = wp.GetComponent<BaseWeapon>();
            primaryInstance.data = loadout.primary; // Inyección de datos
            wp.SetActive(false);
        }

        // CORREGIDO: Añadimos protección extra "loadout.secondary != null" para evitar el NullReferenceException si dejas la casilla vacía
        if (loadout != null && loadout.secondary != null && loadout.secondary.WeaponPrefab != null)
        {
            GameObject ws = Instantiate(loadout.secondary.WeaponPrefab, weaponHolder);
            secondaryInstance = ws.GetComponent<BaseWeapon>();
            secondaryInstance.data = loadout.secondary; // Inyección de datos
            ws.SetActive(false);
        }
    }

    public void EquipPrimary()
    {
        if (primaryInstance == null) return;
        ChangeWeapon(primaryInstance);
    }

    public void EquipSecondary()
    {
        if (secondaryInstance == null) return;
        ChangeWeapon(secondaryInstance);
    }

    private void ChangeWeapon(BaseWeapon newWeapon)
    {
        if (currentWeapon == newWeapon) return;

        if (currentWeapon != null)
            currentWeapon.gameObject.SetActive(false);

        currentWeapon = newWeapon;
        currentWeapon.gameObject.SetActive(true);
    }

    void Update()
    {
        // Obtenemos las referencias del teclado y mouse actuales del nuevo sistema
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        var mouse = UnityEngine.InputSystem.Mouse.current;

        // Si por alguna razón no se detectan, no hacemos nada para evitar errores
        if (keyboard == null || mouse == null) return;

        // 1. Cambios de arma (Teclas 1 y 2)
        if (keyboard.digit1Key.wasPressedThisFrame) EquipPrimary();
        if (keyboard.digit2Key.wasPressedThisFrame) EquipSecondary();

        if (currentWeapon != null)
        {
            // 2. Disparar (Mantener Clic Izquierdo)
            if (mouse.leftButton.isPressed)
                currentWeapon.Shoot();

            // 3. Recargar (Tecla R)
            if (keyboard.rKey.wasPressedThisFrame)
                currentWeapon.StartReload();

            // 4. Apuntar / ADS (Mantener Clic Derecho)
            currentWeapon.SetADS(mouse.rightButton.isPressed);
        }
    }
}