using UnityEngine;

public class PlayerLoadout : MonoBehaviour
{
    [Header("Weapon Slots")]
    public WeaponData primary;
    public WeaponData secondary;

    [Header("Equipment Slots")]
    public EquipmentData lethal;
    public EquipmentData tactical;
}