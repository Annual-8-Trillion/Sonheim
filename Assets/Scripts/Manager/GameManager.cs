using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            return instance == null ? null : instance;
        }
    }

    [SerializeField] private Player player;
    public Player Player
    {
        get
        {
            return player == null ? null : player;
        }
    }

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
