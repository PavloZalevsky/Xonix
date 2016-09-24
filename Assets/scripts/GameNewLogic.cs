using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameNewLogic : MonoBehaviour
{


    public Transform target;
    public int texSize = 128;
    private Texture2D tex;
    private byte[][] map;

    private float speed = 25.0f;

    private Vector3 pos;
    private Vector3 oldpos;
    private Vector3 gridpos;

    private Vector3 startPos;

    void Start()
    {
        map = new byte[texSize][];
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = new byte[texSize];
        }
        tex = new Texture2D(texSize, texSize);

        target.GetComponent<Renderer>().material.mainTexture = tex;
        target.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;

        for (int y = 0; y < texSize; y++)
        {
            for (int x = 0; x < texSize; x++)
            {
                map[x][y] = 0;
                tex.SetPixel(x, y, Color.blue);

                if (x == 0 || y == 0 || x == texSize - 1 || y == texSize - 1)
                {
                    map[x][y] = 33; // border
                    tex.SetPixel(x, y, Color.gray);
                }
            }
        }

        tex.Apply();
        pos = transform.position;
        gridpos = pos;
        oldpos = -pos;
        direction = Vector2.zero;
        transform.position = Vector3.zero;
        points.Clear();
        points.Add(new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0));
    }

    bool start = false;

    private Vector2 direction;

    private void Restart()
    {
        Start();
    }

    private List<Vector3> points = new List<Vector3>();
    private List<Vector3> pointsTMP = new List<Vector3>();

    private List<Vector2> myPoins = new List<Vector2>();

    private bool isHorizontal = true;

    private bool first = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(AutiFloodFill());
        }

        bool needPoint = false;
        float moveX = Input.GetAxisRaw("Horizontal") * speed * -1f;
        float moveY = Input.GetAxisRaw("Vertical") * speed * -1f;

        if (moveX != 0f && moveX == direction.x * -1 || moveY != 0f && moveY == direction.y * -1f)
        {
            moveX = 0f;
            moveY = 0f;
        }

        // auto-movement
        var somethingPressed = moveX != 0f || moveY != 0f;
        moveX = !somethingPressed ? direction.x : moveX;
        moveY = !somethingPressed ? direction.y : moveY;

        if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
            moveY = 0.0f;
        else
            moveX = 0.0f;

        if (moveX == 0f && moveY == 0f)
            return;

        if (direction.x != 0 && moveX == 0 || direction.x == 0 && moveX != 0 || direction.y != 0 && moveY == 0 || direction.y == 0 && moveY != 0)
        {
            tex.SetPixel(Mathf.RoundToInt(gridpos.x), Mathf.RoundToInt(gridpos.y), Color.black);
            points.Add(new Vector3(gridpos.x, gridpos.y));
        }
        gridpos = new Vector3(Mathf.RoundToInt(transform.position.x + moveX * Time.deltaTime), Mathf.RoundToInt(transform.position.y + moveY * Time.deltaTime), 0);
        direction = new Vector2(moveX, moveY);

        //if (gridpos == oldpos)
        //return;
        if (gridpos.x < 0 || gridpos.x >= texSize || gridpos.y < 0 || gridpos.y >= texSize)
            return;
        transform.Translate(new Vector3(moveX, moveY, 0) * Time.deltaTime, Space.World);
        gridpos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);

        if (gridpos != oldpos)
        {
            int x_x = (Mathf.RoundToInt(transform.position.x));
            int y_y = (Mathf.RoundToInt(transform.position.y));
            byte cur = map[x_x][y_y];

            if (cur == 33) // В ЗАБОР
            {
             //   Debug.Log("33");
                //if(!start)
                // StartCoroutine(AutiFloodFill());
            }
            if (cur != 33) // тут ми були
            {
                map[x_x][y_y] = 1;
                myPoins.Add(new Vector2(x_x, y_y));
            }
            tex.SetPixel(Mathf.RoundToInt(gridpos.x), Mathf.RoundToInt(gridpos.y), Color.green);
            tex.Apply();

            if (cur == 1) // самі в себе
            {
              //  Restart();
            }
        }
        oldpos = gridpos;
    }

    IEnumerator AutiFloodFill()
    {
        points.Add(new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0));
        CheckForBorders();
        for (int y = 0; y < texSize; y++)
            for (int x = 0; x < texSize; x++)
            {
                if (polyCheck(points.ToArray(), new Vector3(x, y, 0)))
                {
                    tex.SetPixel(x, y, Color.green);
                }
            }
        tex.Apply();

        CreateMewBorder();

        yield return null;
    }

    private void CreateMewBorder()
    {
        foreach (var item in myPoins)
        {
            map[(int)item.x][(int)item.y] = 33;
        }

        myPoins.Clear();
    }

    private void CheckForBorders()
    {
        var firstPoint = points.First();
        var lastPoint = points.Last();

        if (map[(int)lastPoint.x][(int)lastPoint.y] == 33)
        {
            var tmpY = lastPoint.y;

            if (lastPoint.x == texSize - 1 || lastPoint.x == 0)
            {
                //   FIX Y
                if (lastPoint.y < firstPoint.y || lastPoint.y > firstPoint.y)
                {

                    var dir = firstPoint.y - lastPoint.y > 0 ? 1 : -1;
                    while (tmpY > 0 && tmpY < texSize || map[(int)lastPoint.x][(int)tmpY] != 33)
                    {
                        tmpY += dir;
                        tex.SetPixel((int)lastPoint.x, (int)tmpY, Color.green);
                        if (map[(int)tmpY][(int)lastPoint.x] != 33)
                            if (map[(int)lastPoint.x][(int)tmpY] != 33)
                            {
                                points.Add(new Vector3(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(tmpY)));
                                break;
                            }
                    }
                    points.Add(new Vector3(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(tmpY)));
                }
            }
            if (lastPoint.y == texSize - 1 || lastPoint.y == 0)
            {

                //  FIX X
                if (lastPoint.x < firstPoint.x || lastPoint.x > firstPoint.x)
                {
                    var tmpX = lastPoint.x;
                    var dir = firstPoint.x - lastPoint.x > 0 ? 1 : -1;
                    while (tmpX > 0 && tmpX < texSize || map[(int)tmpX][(int)lastPoint.y] != 33)
                    {
                        tmpX += dir;
                        tex.SetPixel((int)tmpX, (int)lastPoint.y, Color.green);
                        if (map[(int)tmpY][(int)lastPoint.x] != 33)
                            if (map[(int)lastPoint.x][(int)tmpY] != 33)
                            {
                                points.Add(new Vector3(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(tmpY)));
                                break;
                            }
                    }
                    points.Add(new Vector3(Mathf.RoundToInt(tmpX), Mathf.RoundToInt(lastPoint.y)));
                }
            }
        }
    }


    public bool polyCheck(Vector3[] p, Vector3 v)
    {
        int j = p.Length - 1;
        bool c = false;
        for (int i = 0; i < p.Length; j = i++)
            c ^= p[i].y > v.y ^ p[j].y > v.y && v.x < (p[j].x - p[i].x) * (v.y - p[i].y) / (p[j].y - p[i].y) + p[i].x;
        return c;
    }
}

