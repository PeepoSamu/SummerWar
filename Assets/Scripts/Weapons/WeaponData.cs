using UnityEngine;

public enum WeaponSlot
{
    Primary,
    Secondary
}

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "FPS/Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{
    [Header("Base Info")]
    [SerializeField] private string weaponName;
    [SerializeField] private WeaponSlot slot;
    [SerializeField] private GameObject weaponPrefab; // El modelo 3D del arma
    
    [Header("Shooting Settings")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float spread = 0.05f;

    [Header("Ammo & Reloading")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private float reloadTime = 2.0f;

    // --- PROPIEDADES PÚBLICAS ---
    public string WeaponName => weaponName;
    public WeaponSlot Slot => slot;
    public GameObject WeaponPrefab => weaponPrefab;
    public float Damage => damage;
    public float FireRate => fireRate;
    public float Spread => spread;
    public int MagazineSize => magazineSize;
    public float ReloadTime => reloadTime;
}