using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    [HideInInspector]
    public Vector2 directionEnemy = new Vector3(1, 1, 0);
    [HideInInspector]
    public float speedEnemy = 20;
    [HideInInspector]
    public Vector2 LastPoz = Vector2.zero;
    public byte LastSumbl = 100;
    [HideInInspector]
    public int xCur = 0;
    [HideInInspector]
    public int yCur = 0;
    [HideInInspector]
    public int MAxSpeed = 0;
    [HideInInspector]
    public int MinSpeed = 0;

    void OnEnable()
    {
        speedEnemy = Random.Range(MinSpeed, MAxSpeed);
    }
}
