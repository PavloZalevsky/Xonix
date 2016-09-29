using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    [HideInInspector]
    public Vector2 directionEnemy = new Vector3(1, 1, 0);
    [HideInInspector]
    public float speedEnemy = 20;

    //public Enemy(Vector3 position)
    //{
    //    transform.position = position;
    //}
    void OnEnable()
    {
        speedEnemy = Random.Range(10, 40);
    }
}
