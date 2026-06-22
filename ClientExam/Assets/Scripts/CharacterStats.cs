using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private CharacterStatData baseStatData;

    private Dictionary<StatType, float> bonusStats = new();
    private void Awake()
    {
        foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
        {
            bonusStats[statType] = 0;
        }
    }
    public float MaxHp => GetStat(StatType.MaxHp);
    public float MaxStamina => GetStat(StatType.MaxStamina);
    public float Attack => GetStat(StatType.Attack);
    public float Defense => GetStat(StatType.Defense);
    public float MoveSpeed => GetStat(StatType.MoveSpeed);
    public float AttackSpeed => GetStat(StatType.AttackSpeed);
    public float CriticalChance => GetStat(StatType.CriticalChance);

    private float GetStat(StatType statType)
    {
        if (baseStatData == null)
        {
            Debug.LogError("CharacterStatData가 연결되지 않았습니다.");
            return 0;
        }

        if (!bonusStats.ContainsKey(statType))
            bonusStats.Add(statType, 0f);

        float baseValue = statType switch
        {
            StatType.MaxHp => baseStatData.maxHp,
            StatType.MaxStamina => baseStatData.maxStamina,
            StatType.Attack => baseStatData.attack,
            StatType.Defense => baseStatData.defense,
            StatType.MoveSpeed => baseStatData.moveSpeed,
            StatType.AttackSpeed => baseStatData.attackSpeed,
            StatType.CriticalChance => baseStatData.criticalChance,
            _ => 0
        };

        return baseValue + bonusStats[statType];
    }
    public void AddModifier(StatModifierData modifier)
    {
        if (!bonusStats.ContainsKey(modifier.statType))
            bonusStats[modifier.statType] = 0f;
        bonusStats[modifier.statType] += modifier.value;
    }

    public void RemoveModifier(StatModifierData modifier)
    {
        if (!bonusStats.ContainsKey(modifier.statType))
            bonusStats[modifier.statType] = 0f;
        bonusStats[modifier.statType] -= modifier.value;
    }
}
