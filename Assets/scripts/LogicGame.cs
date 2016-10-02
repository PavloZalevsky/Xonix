﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LogicGame : MonoBehaviour
{
    [Header("Settigs Game")]
    [Range(10f, 85)]
    public float percentWin = 85;




    public int DensityPixels = 10;
    public Transform Player;
    public Camera cameraOther;

    public Transform target;
    // public int texSize = 128;
    private Texture2D tex;
    private byte[][] map;

    public List<Enemy> PoolEnemy = new List<Enemy>();
    private List<Enemy> enemies = new List<Enemy>();


    private float speed = 25.0f;


    private int xSize = 0;
    private int ySize = 0;



    private Vector3 pos;
    private Vector3 gridpos;
    private Vector3 oldgridpos;

    private Vector3 startPos;

    private bool load = false;

    private int allPixel = 0;
    private int paintedPixels = 0;
    private int paintedPixelsBorder = 0;
    private float percentpainted = 0;



    void OnEnable()
    {
        CameraSettings();
    }

    void Start()
    {
        map = new byte[xSize][];
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = new byte[ySize];
        }
        tex = new Texture2D(xSize, ySize);

        target.GetComponent<Renderer>().material.mainTexture = tex;
        target.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                map[x][y] = 0;
                tex.SetPixel(x, y, Color.blue);

                if (x == 0 || y == 0 || x == xSize - 1 || y == ySize - 1)
                {
                    map[x][y] = 33; // border
                    tex.SetPixel(x, y, Color.gray);
                    paintedPixelsBorder++;
                }
            }
        }

        tex.Apply();
        allPixel =( xSize * ySize) - paintedPixelsBorder;
        paintedPixels = 0;
        pos = new Vector3(xSize / 2, 0, 0);
        gridpos = pos;
        oldgridpos = Vector3.up;
        transform.position = pos;
        Player.position = new Vector3(pos.x + 0.5f, pos.y);

        direction = Vector2.zero;
        myPoins.Clear();
        points.Clear();
        points.Add(new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0));
        percentpainted = 0;

        SpawnEnemies();
        load = true;
    }

    private void SpawnEnemies()
    {
     // return;
        //TODO//
        foreach (var item in PoolEnemy)
        {
            item.gameObject.SetActive(false);
        }
        enemies.Clear();

        int countEnemy = UnityEngine.Random.Range(2, 5);

        for (int i = 0; i < countEnemy; i++)
        {
            PoolEnemy[i].transform.position = new Vector3(UnityEngine.Random.Range(10, xSize - 10), UnityEngine.Random.Range(10, ySize - 10), 0);
            PoolEnemy[i].gameObject.SetActive(true);
            enemies.Add(PoolEnemy[i]);
        }
    }

    void CameraSettings()
    {
        float heightMainCamera = Camera.main.orthographicSize * 2f;
        float widtMainCamerh = heightMainCamera * Screen.width / Screen.height;
        target.transform.localScale = new Vector3(widtMainCamerh, heightMainCamera, 0.1f);

        xSize = Mathf.RoundToInt(widtMainCamerh * DensityPixels);
        ySize = Mathf.RoundToInt(heightMainCamera * DensityPixels);

        cameraOther.orthographicSize = cameraOther.orthographicSize * DensityPixels;
        float heightOtherCamera = 2f * cameraOther.orthographicSize;
        float widthOtherCamera = heightOtherCamera * cameraOther.aspect;
        cameraOther.transform.position = new Vector3(cameraOther.transform.position.x + widthOtherCamera / 2, cameraOther.transform.position.y + heightOtherCamera / 2, -10);

     

        Debug.Log(xSize + "   " + ySize + "  AllPixel " + allPixel);
    }


    private Vector2 direction;

    private void Restart()
    {
        load = false;
        Start();
    }

    private List<Vector3> points = new List<Vector3>();

    private List<Vector2> myPoins = new List<Vector2>();

    void Update()
    {
       // MoveEnemy();
        if (Input.GetKeyDown(KeyCode.U))
        {
            AutoFloodFill();
        }

        float moveX = Input.GetAxisRaw("Horizontal") * speed;
        float moveY = Input.GetAxisRaw("Vertical") * speed;

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

        gridpos = new Vector3(Mathf.RoundToInt(transform.position.x + moveX * Time.deltaTime), Mathf.RoundToInt(transform.position.y + moveY * Time.deltaTime), 0);

        if (gridpos.x < 0 || gridpos.x > xSize - 1 || gridpos.y < 0 || gridpos.y > ySize - 1)
            return;

        transform.Translate(new Vector3(moveX, moveY, 0) * Time.deltaTime, Space.World);
        Player.position = new Vector3(gridpos.x + 0.5f, gridpos.y);

        if (direction.x != 0 && moveX == 0 || direction.x == 0 && moveX != 0 || direction.y != 0 && moveY == 0 || direction.y == 0 && moveY != 0)
        {
            tex.SetPixel(Mathf.RoundToInt(gridpos.x), Mathf.RoundToInt(gridpos.y), Color.black);
            tex.Apply();

            points.Add(new Vector3(gridpos.x, gridpos.y));
        }

        if (oldgridpos.x > 0 && oldgridpos.x < xSize - 1 && oldgridpos.y > 0 && oldgridpos.y < ySize - 1)
        {
            try
            {
                if (map[(int)oldgridpos.x][(int)oldgridpos.y] == 33 &&
                    ((map[(int)gridpos.x + 1][(int)gridpos.y] == 0 && map[(int)gridpos.x - 1][(int)gridpos.y] == 0)
                     || (map[(int)gridpos.x][(int)gridpos.y + 1] == 0 && map[(int)gridpos.x][(int)gridpos.y - 1] == 0)))
                {
                    tex.SetPixel(Mathf.RoundToInt(oldgridpos.x), Mathf.RoundToInt(oldgridpos.y), Color.black);
                    points.Add(new Vector3(oldgridpos.x, oldgridpos.y));
                }
            }
            catch(Exception e)
            {
                Debug.LogError("!!! " + (int)oldgridpos.x + " " + (int)oldgridpos.y);
            }
        }


        if (oldgridpos != gridpos)
        {
            direction = new Vector2(moveX, moveY);

            int x_x = (Mathf.RoundToInt(gridpos.x));
            int y_y = (Mathf.RoundToInt(gridpos.y));
            byte cur = map[x_x][y_y];

            if (cur == 1) /*ми*/
            {
                Restart();
            }

            if (cur == 33)/*забор*/
            {
                if (myPoins.Count != 0)
                {
                    AutoFloodFill();
                }
            }
            if (cur != 33 && cur != 1)
            {
                map[x_x][y_y] = 1;
                myPoins.Add(new Vector2(x_x, y_y));
                tex.SetPixel(Mathf.RoundToInt(gridpos.x), Mathf.RoundToInt(gridpos.y), Color.green);
                tex.Apply();
                paintedPixels++;
            }
            oldgridpos = gridpos;

         
        }
    }

    void MoveEnemy()
    {
        //TODO//
        if (!load) return;
        

        foreach (var ene in enemies)
        {
            if (!ene.gameObject.activeSelf) continue;

            int x_x = (Mathf.RoundToInt(ene.transform.position.x));
            int y_y = (Mathf.RoundToInt(ene.transform.position.y));

            if (x_x < 0)
            {
                x_x = 0;
            }
            if (x_x > xSize - 1)
            {
                x_x = xSize - 1;
            }
            if (y_y < 0)
            {
                y_y = 0;
            }
            if (y_y > ySize - 1)
            {
                y_y = ySize - 1;
            }

            if (map[x_x][y_y] == 33)
            {
                if ((y_y + 1 < ySize - 1 && map[x_x][y_y + 1] == 33) && (y_y - 1 > 0 && map[x_x][y_y - 1] == 33))
                {
                    ene.directionEnemy = new Vector2(ene.directionEnemy.x * -1, ene.directionEnemy.y);
                }
                else if ((x_x + 1 < xSize - 1 && map[x_x + 1][y_y] == 33) && (x_x - 1 > 0 && map[x_x - 1][y_y] == 33))
                {
                    ene.directionEnemy = new Vector2(ene.directionEnemy.x, ene.directionEnemy.y * -1);
                }
                else
                {
                   // Debug.Log(x_x + " : " + y_y);
                    tex.SetPixel(x_x, y_y, Color.red);
                    tex.Apply();
                    ene.directionEnemy = new Vector2(ene.directionEnemy.x * -1, ene.directionEnemy.y * -1);
                }
                ene.transform.Translate(ene.directionEnemy * Time.deltaTime * ene.speedEnemy, Space.World);

            }
            //else if(map[x_x][y_y] == 33)
            //{
            //    ene.gameObject.SetActive(false);
            //}

            if (map[x_x][y_y] == 1)
            {
                Restart();
                break;
            }

            if (map[x_x][y_y] != 33 && map[x_x][y_y] != 1)
            {
                map[x_x][y_y] = 2;
                ene.xCur = x_x;
                ene.yCur = y_y;

                if (ene.LastPoz != Vector2.zero)
                {

                    map[(int)ene.LastPoz.x][(int)ene.LastPoz.y] = ene.LastSumbl;
                }

                ene.LastPoz = new Vector2(x_x, y_y);
                ene.LastSumbl = map[x_x][y_y];
            }

            ene.transform.Translate(ene.directionEnemy * Time.deltaTime * ene.speedEnemy, Space.World);
        }
    }

    void AutoFloodFill()
    {
       // Debug.Log("AutoFloodFill");
        points.Add(new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0));

        CheckPoins();
        CreateMewBorder();
        direction = Vector2.zero;
        // 
    }

    private void CheckPoins()
    {
        var lastPoint = points.Last();
        var preLastPoint = points[points.Count - 2];

        var difference = lastPoint - preLastPoint;
        var angle = Mathf.Atan2(difference.y, difference.x);
        var middlePoint = (lastPoint + preLastPoint) / 2;
        var middleLeft = middlePoint + new Vector3(Mathf.Cos(angle - Mathf.PI / 2) * 1.5f, Mathf.Sin(angle - Mathf.PI / 2) * 1.5f, 0f);
        var middleRight =  middlePoint + new Vector3(Mathf.Cos(angle + Mathf.PI / 2) * 1.5f, Mathf.Sin(angle + Mathf.PI / 2) * 1.5f, 0f);

       


        Debug.Log(middleLeft);
        Debug.Log(middleRight);

        tex.SetPixel(Mathf.RoundToInt(middleLeft.x), Mathf.RoundToInt(middleLeft.y), Color.red);
        tex.SetPixel(Mathf.RoundToInt(middleRight.x), Mathf.RoundToInt(middleRight.y), Color.yellow);
        tex.Apply();


        var countLeft = TryToFill(Mathf.RoundToInt(middleLeft.x), Mathf.RoundToInt(middleLeft.y));
        var countRight = TryToFill(Mathf.RoundToInt(middleRight.x), Mathf.RoundToInt(middleRight.y));

        if(countLeft == 0 && countRight == 0)
        {

        }

        ClearFill(Mathf.RoundToInt(middleLeft.x), Mathf.RoundToInt(middleLeft.y));
        ClearFill(Mathf.RoundToInt(middleRight.x), Mathf.RoundToInt(middleRight.y));

        Debug.Log(countLeft + " : " + countRight);
        Debug.Log("--------------------------------------");

        if (countLeft <= countRight)
            paintedPixels += countLeft;
        else
            paintedPixels += countRight;



        percentpainted = paintedPixels / (allPixel / 100f);

        if (percentpainted > percentWin)
            Debug.Log("WIN!!!!!!!!!!!!!!");

       // Debug.Log(percentpainted);

        StartCoroutine(FloodFillCorot(
                countLeft <= countRight ? Mathf.RoundToInt(middleLeft.x) : Mathf.RoundToInt(middleRight.x),
                countLeft <= countRight ? Mathf.RoundToInt(middleLeft.y) : Mathf.RoundToInt(middleRight.y)
                ));

        
        tex.Apply(); 
    }
 

    private int TryToFill(int x, int y)
    {
        if (x <= 0 || x >= xSize - 1 || y <= 0 || y >= ySize - 1 || map[x][y] == 33 || map[x][y] == 1 || map[x][y] == 8)
            return 0;

        map[x][y] = 8;

        return 1 + TryToFill(x + 1, y) + TryToFill(x - 1, y) + TryToFill(x, y + 1) + TryToFill(x, y - 1);
    }

    private void ClearFill(int x, int y)
    {
        if (x <= 0 || x >= xSize - 1 || y <= 0 || y >= ySize - 1)
            return;
        var color = map[x][y];
        if (color != 8)
            return;

        if (color == 8)
        {
            map[x][y] = 0;
        }

        ClearFill(x + 1, y);
        ClearFill(x - 1, y);
        ClearFill(x, y + 1);
        ClearFill(x, y - 1);
    }

    private IEnumerator FloodFillCorot(int x, int y)
    {
        var color = tex.GetPixel(x, y);

        if (x >= 0  && x <= xSize - 1 && y >= 0 && y <= ySize - 1 && map[x][y] == 2)
        {
            foreach (var ene in enemies)
            {
                if (ene.xCur == x && ene.yCur == y)
                {
                    ene.gameObject.SetActive(false);
                }
            }
        }

        if (color == Color.green || color == Color.magenta || color == Color.black || x < 1 || x >= xSize - 1 || y < 1 || y >= ySize - 1)
            yield break;

        map[x][y] = 33;
        tex.SetPixel(x, y, Color.green);
        tex.Apply();

        yield return new WaitForSeconds(0.025f);
        StartCoroutine(FloodFillCorot(x + 1, y));
        StartCoroutine(FloodFillCorot(x - 1, y));
        StartCoroutine(FloodFillCorot(x, y + 1));
        StartCoroutine(FloodFillCorot(x, y - 1));
    }

    public void FloodFill(int x, int y)
    {
        var color = tex.GetPixel(x, y);
        //if (map[x][y] == 33)
        //return;
        //map[x][y
        if(map[x][y] == 2)
        {
            Debug.Log("!!");
        }

        if (color == Color.green)
            return;
        tex.SetPixel(x, y, Color.green);
        tex.Apply();
        FloodFill(x + 1, y);
        FloodFill(x - 1, y);
        FloodFill(x, y + 1);
        FloodFill(x, y - 1);
    }

    private void CreateMewBorder()
    {
        foreach (var item in myPoins)
        {
            map[(int)item.x][(int)item.y] = 33;
        }

        myPoins.Clear();
    }
}
