// EquipmentData.cs
using UnityEngine;

public enum EquipmentType { Lethal, Tactical }

[CreateAssetMenu(menuName="FPS/Equipment")]
public class EquipmentData : ScriptableObject
{
    [SerializeField] private string equipmentName; // Evitamos usar 'name'
    [SerializeField] private EquipmentType type;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int maxAmount = 2;

    public string EquipmentName => equipmentName;
    public EquipmentType Type => type;
    public GameObject Prefab => prefab;
    public int MaxAmount => maxAmount;
}