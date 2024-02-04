using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager s_Instance = null;

    [SerializeField]
    private Ship m_Ship = null;

    [SerializeField]
    private GameCamera m_Camera = null;


    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public Ship GetShip()
    {
        return m_Ship;
    }

    public GameCamera GetCamera()
    {
        return m_Camera;
    }

    public static GameManager Get()
    {
        if (s_Instance == null)
        {
            s_Instance = FindObjectOfType<GameManager>();
        }

        return s_Instance;
    }
}