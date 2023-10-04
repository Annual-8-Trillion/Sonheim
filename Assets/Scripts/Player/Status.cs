using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Status
{

    public event Action<Status> OnStatusChanged;

    [SerializeField] private float curHealth;
    public float CurHealth
    {
        get
        {
            return curHealth;
        }
        set
        {
            float preValue = curHealth;
            curHealth = value;
            if (preValue != value) { CallStatusChangedEvent(); }
        }
    }

    [SerializeField] private float maxHealth;
    public float MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            float preValue = maxHealth;
            maxHealth = value;
            if (preValue != value) { CallStatusChangedEvent(); }
        }
    }

    [SerializeField] private float stamina;
    public float Stamina
    {
        get
        {
            return stamina;
        }
        set
        {
            float preValue = stamina;
            stamina = value;
            if (preValue != value) { CallStatusChangedEvent(); }
        }
    }

    [SerializeField] private float hunger;
    public float Hunger
    {
        get
        {
            return hunger;
        }
        set
        {
            float preValue = hunger;
            hunger = value;
            if (hunger >= 100f) hunger = 100f;
            if (hunger < 0f)
            {
                curHealth += hunger;
                hunger = 0f;
            }
            if (preValue != value) { CallStatusChangedEvent(); }
        }
    }

    [SerializeField] private float thirst;
    public float Thirst
    {
        get
        {
            return thirst;
        }
        set
        {
            float preValue = thirst;
            thirst = value;
            if (thirst >= 100f) thirst = 100f;
            if (thirst < 0f)
            {
                curHealth += thirst;
                thirst = 0f;
            }
            if (preValue != value) { CallStatusChangedEvent(); }
        }
    }

    public Status(int _maxHealth, int _stamina, int _hunger, int _thirst)
    {
        curHealth = _maxHealth;
        maxHealth = _maxHealth;
        stamina = _stamina;
        hunger = _hunger;
        thirst = _thirst;
    }

    public void CallStatusChangedEvent()
    {
        OnStatusChanged?.Invoke(this);
    }
}
