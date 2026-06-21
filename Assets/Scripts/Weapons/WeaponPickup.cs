using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Asigna el ScriptableObject de esta arma aquí")]
    public WeaponData weaponData; 

    void OnTriggerEnter(Collider other)
    {
        WeaponManager manager = other.GetComponent<WeaponManager>();

        // Verificamos que el jugador tenga el Manager y el Loadout asignados
        if (manager != null && manager.loadout != null && weaponData != null)
        {
            // Detectamos si el arma del suelo es Primaria o Secundaria y actualizamos el Loadout
            if (weaponData.Slot == WeaponSlot.Primary)
            {
                manager.loadout.primary = weaponData;
                manager.InitializeWeapons(); // Re-crea los objetos en las manos
                manager.EquipPrimary();       // Te la pone en la mano inmediatamente
            }
            else if (weaponData.Slot == WeaponSlot.Secondary)
            {
                manager.loadout.secondary = weaponData;
                manager.InitializeWeapons();
                manager.EquipSecondary();
            }

            Destroy(gameObject); // Destruye el item del suelo
        }
    }
}