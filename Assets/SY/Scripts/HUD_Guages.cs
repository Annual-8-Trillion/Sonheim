using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Guages : MonoBehaviour
{
    public RectTransform hpGuageFill;
    public RectTransform hungerGuageFill;
    public RectTransform staminaGuageFill;
    public RectTransform thirstyGuageFill;


    private void Start()
    {
        GameManager.Instance.Player.status.OnStatusChanged += OnStatusChanged;
    }

    public void OnStatusChanged(Status newStatus)
    {
        Vector2 hp = hpGuageFill.sizeDelta;
        Vector2 stamina = staminaGuageFill.sizeDelta;
        Vector2 hunger = hungerGuageFill.sizeDelta;
        Vector2 thirst = thirstyGuageFill.sizeDelta;

        hp.x = (newStatus.CurHealth / newStatus.MaxHealth) * 300f;
        hpGuageFill.sizeDelta = hp;
        stamina.x = newStatus.Stamina * 3f;
        staminaGuageFill.sizeDelta = stamina;
        hunger.x = newStatus.Hunger * 3f;
        hungerGuageFill.sizeDelta = hunger;
        thirst.x = newStatus.Thirst * 3f;
        thirstyGuageFill.sizeDelta = thirst;

    }
}
