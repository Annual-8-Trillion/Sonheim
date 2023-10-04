using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public Status status = new Status(100, 100, 100, 100);

    public Weapon weapon;
    public GameObject weaponPoint;  
    private GameObject curEquipedWeapon;

    public float hpChangeDelay;
    public float staminaChangeDelay;
    public float hungerChangeDelay;
    public float thirstChangeDelay;

    private bool isHpChangable;
    private bool isStaminaChangable;
    private bool isHungerChangable;
    private bool isThirstChangable;

    private float timeFromLastHpChanged;
    private float timeFromLastHungerChanged;
    private float timeFromLastStaminaChanged;
    private float timeFromLastThirstChanged;

    private MeshRenderer[] meshRenderers;

    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    private void Start()
    {
        isHpChangable = true;
        isStaminaChangable = true;
        isHungerChangable = true;
        isThirstChangable = true;
        timeFromLastHpChanged = 0;
        timeFromLastStaminaChanged = 0;
        timeFromLastHungerChanged = 0;
        timeFromLastThirstChanged = 0;
    }

    private void Update()
    {
        if (!isHpChangable)
        {
            timeFromLastHpChanged += Time.deltaTime;
            if (timeFromLastHpChanged > hpChangeDelay )
            {
                isHpChangable=true;
                timeFromLastHpChanged = 0;
            }
        }
        if (!isStaminaChangable)
        {
            timeFromLastStaminaChanged += Time.deltaTime;
            if (timeFromLastStaminaChanged > staminaChangeDelay)
            {
                isStaminaChangable = true;
                timeFromLastStaminaChanged = 0;
            }
        }
        if (!isHungerChangable)
        {
            timeFromLastHungerChanged += Time.deltaTime;
            if (timeFromLastHungerChanged > hungerChangeDelay)
            {
                isHungerChangable = true;
                timeFromLastHungerChanged = 0;
            }
        }
        if (!isThirstChangable)
        {
            timeFromLastThirstChanged += Time.deltaTime;
            if (timeFromLastThirstChanged > thirstChangeDelay)
            {
                isThirstChangable = true;
                timeFromLastThirstChanged = 0;
            }
        }

        if (isHungerChangable) AddHunger(-1f);
        if (isThirstChangable) AddThirst(-1f);
    }

    public void EquipWeapon(GameObject weaponPrefab)
    {
        GameObject _weapon = Instantiate(weaponPrefab) as GameObject;
        _weapon.transform.SetParent(weaponPoint.transform, false);
        curEquipedWeapon = _weapon;

        weapon = _weapon.GetComponent<Weapon>();
    }

    public void UnEquipWeapon(GameObject weaponPrefab)
    {
        Destroy(curEquipedWeapon);
        curEquipedWeapon = null;
    }
    public void AddStamina(float value)
    {
        if (isStaminaChangable)
        {
            status.Stamina += value;
            timeFromLastStaminaChanged = 0f;
            isStaminaChangable = false;
        }
    }
    public void AddStamina(float value, bool hasDelay)
    {
        if (hasDelay) { AddStamina(value); }
        else
        {
            status.Stamina += value;
        }
    }
    public void AddHp(float value)
    {
        if (isHpChangable)
        {
            if (value < 0f)
            {
                StartCoroutine("DamageFlash");
            }
            status.CurHealth += value;
            timeFromLastHpChanged = 0f;
            isHpChangable = false;
        }
    }
    public void AddHp(float value, bool hasDelay)
    {
        if (hasDelay) { AddHp(value); }
        else
        {
            if (value < 0f)
            {
                StartCoroutine("DamageFlash");
            }
            status.CurHealth += value;
        }
    }
    public void AddHunger(float value)
    {
        if (isHungerChangable)
        {
            status.Hunger += value;
            timeFromLastHungerChanged = 0f;
            isHungerChangable = false;
        }
    }
    public void AddHunger(float value, bool hasDelay)
    {
        if (hasDelay) { AddHunger(value); }
        else
        {
            status.Hunger += value;
        }
    }
    public void AddThirst(float value)
    {
        if (isThirstChangable)
        {
            status.Thirst += value;
            timeFromLastThirstChanged = 0f;
            isThirstChangable = false;
        }
    }
    public void AddThirst(float value, bool hasDelay)
    {
        if (hasDelay) { AddThirst(value); }
        else
        {
            status.Thirst += value;
        }
    }
    public void AddMaxHp(float value)
    {
        status.MaxHealth += value;
    }

    IEnumerator DamageFlash()
    {
        for (int x = 0; x < meshRenderers.Length; x++)
            meshRenderers[x].material.color = new Color(1.0f, 0.6f, 0.6f);

        yield return new WaitForSeconds(0.1f);
        for (int x = 0; x < meshRenderers.Length; x++)
            meshRenderers[x].material.color = Color.white;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy Attack")
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            AddHp((float)enemy.damage * -1f);
            Debug.Log(status.CurHealth);
        }
    }
}
