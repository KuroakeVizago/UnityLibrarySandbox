using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ItemObjectData", fileName = "ItemData")]
public class ItemObject : ScriptableObject
{
    [SerializeField] protected ItemData m_itemData;

    public ItemData GetItemData => m_itemData;
    public string ItemName => m_itemData.Name;
    public string ItemDescription => m_itemData.Description;
    public Sprite ItemIcon => m_itemData.Icon;
    public bool IsStackable => m_itemData.IsStackable;
    
    public override bool Equals(object other)
    {
        bool equal;
        if (other is ItemObject otherItem)
        {
            equal = string.Equals(otherItem.ItemName, ItemName, StringComparison.CurrentCultureIgnoreCase);
        }
        else
            equal = false;
        return equal;
    }

    public bool Equals(ItemObject other)
    {
        return base.Equals(other) && m_itemData.Equals(other.m_itemData);
    }
    
    public override int GetHashCode()
    {
        unchecked
        {
            return (base.GetHashCode() * 397) ^ m_itemData.GetHashCode();
        }
    }
}

[System.Serializable]
public struct ItemData
{
    [Header("Basic Information:")]
    [SerializeField] private string m_name;
    [SerializeField] private string m_description;
    [SerializeField] private Sprite m_icon;
    [Space]
    [Header("Config:")]
    [SerializeField] private bool m_stackableItem;

    public string Name => m_name;
    public string Description => m_description;
    public Sprite Icon => m_icon;
    public bool IsStackable => m_stackableItem;


}

