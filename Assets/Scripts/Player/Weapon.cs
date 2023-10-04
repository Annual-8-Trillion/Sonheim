using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Pickaxe, Axe, Club };
    public Type type;
    public int damage;
    public BoxCollider attackArea;

    public void Use()
    {
        StartCoroutine("Swing");
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.2f);
        attackArea.enabled = true;

        yield return new WaitForSeconds(0.3f);
        attackArea.enabled = false;
    }
}
