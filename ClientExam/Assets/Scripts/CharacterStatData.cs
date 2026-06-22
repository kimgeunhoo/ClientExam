using UnityEngine;

[CreateAssetMenu(menuName = "Data/Character Stat Data")]
public class CharacterStatData : ScriptableObject
{
    [Header("Base Stats")]
    public float maxHp = 100;
    public float maxStamina = 100;
    public float attack = 10;
    public float defense = 5;
    public float moveSpeed = 5;
    public float attackSpeed = 1;
    public float criticalChance = 0;
}
