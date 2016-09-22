using UnityEngine;
using System.Collections;
using System;

public class Game : MonoBehaviour {

    class Item
    {
        SpriteRenderer sprite;

    }

    [Header("Map")]
	public SpriteRenderer sprite;	
	public int xCubes;
	public int yCubes;

    [Header("Game")]
    public int aa;


    void Start () {
            CreateMap();
        }

//    void Update();

    private void CreateMap()
    { 
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        width *= 0.5f;
        height *= 0.5f;

        var startX = Camera.main.transform.position.x - width;
        var startyY = Camera.main.transform.position.y - height;

        startX += (sprite.bounds.size.x / 2);
        startyY += (sprite.bounds.size.y / 2);

        int d = 0;

        for (int z = 0; z < yCubes; z++)
        {
            for (int x = 0; x < xCubes; x++)
            {
                d++;
                Instantiate(sprite, new Vector3(startX + (x * sprite.bounds.size.x), startyY + (z * sprite.bounds.size.y), 0), Quaternion.identity);
            }
        }
        Debug.Log(d);
    }





}
