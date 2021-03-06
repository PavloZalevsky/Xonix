﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameLogic : MonoBehaviour
{
    [Header("Game Logic element")]
    [Range(10f, 85)]
    public float percentWin = 85;
    public float PlayerSpeed = 20.0f;

    public Transform Player;
    public Camera cameraOther;
    public Transform target;

    private int DensityPixels = 4;

    private Texture2D tex;
    private byte[][] map;

    public List<Enemy> PoolEnemy = new List<Enemy>();
    private List<Enemy> enemies = new List<Enemy>();


    private int xSize = 0;
    private int ySize = 0;

    private Vector3 pos;
    private Vector3 gridpos;
    private Vector3 oldgridpos;

    private bool load = false;

    private Vector2 direction;
    private List<Vector3> points = new List<Vector3>();
    private List<Vector2> myPoins = new List<Vector2>();

    private int allPixel = 0;
    private int paintedPixels = 0;
    private int paintedPixelsBorder = 0;
    private float percentpainted = 0;

    private int CountLife = 3;
    private int CurrentLife = 0;

    private int level = 1;

    private float time = 0;

    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;

    public virtual void OnEnable()
    {
        CameraSettings();
    }

    public virtual void StartGame(bool nextLevel = false)
    {
        map = new byte[xSize][];
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = new byte[ySize];
        }
        tex = new Texture2D(xSize, ySize);

        target.GetComponent<Renderer>().material.mainTexture = tex;
        target.GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point;
        paintedPixelsBorder = 0;
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
        allPixel = (xSize * ySize) - paintedPixelsBorder;
        pos = new Vector3(xSize / 2, 0, 0);
        gridpos = pos;
        oldgridpos = Vector3.up;
        transform.position = pos;
        Player.position = new Vector3(pos.x + 0.5f, pos.y);
        direction = Vector2.zero;
        myPoins.Clear();
        points.Clear();
        points.Add(new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0));
        paintedPixels = 0;
        percentpainted = 0;

        if (nextLevel)
        {
            level++;
        }
        else
        {
            level = 1;
            CurrentLife = CountLife;
        }
        time = LevelConfig.instance.levels[level - 1].Time;
        ShowPercent(percentpainted, percentWin);
        ShowHeart(CurrentLife);
        ShowLevel(level);

        SpawnEnemies(level);
        Invoke("Loading", 0.5f);
        start = false;
        Timer(time);
    }

    bool start = false;

    void Loading()
    {
        load = true;
    }


    private void Restart()
    {
        pos = new Vector3(xSize / 2, 0, 0);
        gridpos = pos;
        oldgridpos = Vector2.zero;
        transform.position = pos;
        Player.position = new Vector3(pos.x + 0.5f, pos.y);
        direction = Vector2.zero;
        foreach (var item in myPoins)
        {
            map[(int)item.x][(int)item.y] = 0;
            tex.SetPixel((int)item.x, (int)item.y, Color.blue);
        }
        tex.Apply();
        myPoins.Clear();
        points.Clear();
        time = LevelConfig.instance.levels[level - 1].Time;

        if (--CurrentLife == 0)
        {
            GameOver();
        }
        ShowHeart(CurrentLife);
        Timer(time);
        start = false;
    }


    private void SpawnEnemies(int level)
    {
        foreach (var item in PoolEnemy)
        {
            item.gameObject.SetActive(false);
        }
        enemies.Clear();

        int countEnemy = LevelConfig.instance.levels[level - 1].CountEnemy;

        for (int i = 0; i < countEnemy; i++)
        {
            PoolEnemy[i].transform.position = new Vector3(UnityEngine.Random.Range(10, xSize - 10), UnityEngine.Random.Range(10, ySize - 10), 0);
            PoolEnemy[i].MaxSpeed = LevelConfig.instance.levels[level - 1].MaxSpeedEnemy;
            PoolEnemy[i].MinSpeed = LevelConfig.instance.levels[level - 1].MinSpeedEnemy
                ;
            PoolEnemy[i].gameObject.SetActive(true);
            enemies.Add(PoolEnemy[i]);
        }
    }

    void CameraSettings()
    {
        float heightMainCamera = Camera.main.orthographicSize * 2f;
        float widtMainCamerh = heightMainCamera * Screen.width / Screen.height;

        target.transform.localScale = new Vector3(widtMainCamerh, heightMainCamera - 1, 0.1f);
        target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y - 0.5f, target.transform.position.z);

        xSize = Mathf.RoundToInt(widtMainCamerh * DensityPixels);
        ySize = Mathf.RoundToInt((heightMainCamera - 1) * DensityPixels);

        cameraOther.orthographicSize = cameraOther.orthographicSize * DensityPixels;
        float heightOtherCamera = 2f * cameraOther.orthographicSize;
        float widthOtherCamera = heightOtherCamera * cameraOther.aspect;
        cameraOther.transform.position = new Vector3(cameraOther.transform.position.x + widthOtherCamera / 2, cameraOther.transform.position.y + heightOtherCamera / 2, -10);
    }

    void UpdateTimer()
    {
        if (!start) return;

        time -= Time.deltaTime;
        if (time <= 0)
            Restart();

        Timer(time);
    }

    float wid = 0.8f;
    public void SwipeTouch()
    {
        moveX = 0f;
        moveY = 0f;
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                start = true;
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }
            if (t.phase == TouchPhase.Moved)
            {
                secondPressPos = new Vector2(t.position.x, t.position.y);
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                currentSwipe.Normalize();

                if (currentSwipe.y > 0 && currentSwipe.x > wid * -1 && currentSwipe.x < wid) //up
                {
                    moveY = 1 * PlayerSpeed;
                }
                if (currentSwipe.y < 0 && currentSwipe.x > wid * -1f && currentSwipe.x < wid)// down
                {
                    moveY = -1 * PlayerSpeed;
                }
                if (currentSwipe.x < 0 && currentSwipe.y > wid * -1 && currentSwipe.y < wid)//left
                {
                    moveX = -1 * PlayerSpeed;
                }
                if (currentSwipe.x > 0 && currentSwipe.y > wid * -1 && currentSwipe.y < wid)//right
                {
                    moveX = 1 * PlayerSpeed;
                }
            }
        }
    }

    float moveX = 0f;
    float moveY = 0f;

    void Update()
    {
        if (!load) return;

          MoveEnemy();
        UpdateTimer();
        MovePlayer();
    }

    private void MovePlayer()
    {
#if UNITY_EDITOR
        moveX = Input.GetAxisRaw("Horizontal") * PlayerSpeed;
        moveY = Input.GetAxisRaw("Vertical") * PlayerSpeed;
#else
        SwipeTouch();
#endif
        if (moveX != 0 || moveY != 0)
            start = true;

        if (!start) return;

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

        int x = (Mathf.RoundToInt(gridpos.x));
        int y = (Mathf.RoundToInt(gridpos.y));

        if (gridpos.x < 0 || gridpos.x > xSize - 1 || gridpos.y < 0 || gridpos.y > ySize - 1)
            return;

        transform.Translate(new Vector3(moveX, moveY, 0) * Time.deltaTime, Space.World);
        Player.position = new Vector3(gridpos.x + 0.5f, gridpos.y);

        direction = new Vector2(moveX, moveY);

        if (oldgridpos != gridpos)
        {
            byte cur = map[x][y];

            if (cur == 1) /*ми*/
            {
                Restart();
            }

            if (cur == 33)/*забор*/
            {
                if (myPoins.Count != 0 && map[(int)oldgridpos.x][(int)oldgridpos.y] != 33)
                {
                    AutoFloodFill();
                }
            }

            if (cur != 33 && cur != 1)
            {
                map[x][y] = 1;
                myPoins.Add(new Vector2(x, y));
                tex.SetPixel(Mathf.RoundToInt(gridpos.x), Mathf.RoundToInt(gridpos.y), Color.green);
                tex.Apply();
                paintedPixels++;
            }


            int Ox = (Mathf.RoundToInt(oldgridpos.x));
            int Oy = (Mathf.RoundToInt(oldgridpos.y));

            if (oldgridpos.x > 0 && oldgridpos.x < xSize - 1 && oldgridpos.y > 0 && oldgridpos.y < ySize - 1)
            {
                if (map[Ox][Oy] != 33 && (((map[Ox + 1][Oy] == 1 && map[Ox - 1][Oy] == 0) || (map[Ox - 1][Oy] == 1 && map[Ox + 1][Oy] == 0))
                && ((map[Ox][Oy + 1] == 1 && map[Ox][Oy - 1] == 0) || (map[Ox][Oy - 1] == 1 && map[Ox][Oy + 1] == 0))))
                {
                   points.Add(new Vector3(Ox, Oy));
                   flag = false;
                }
                else if (map[Ox][Oy] != 33 && (map[Ox + 1][Oy] == 1 && map[Ox - 1][Oy] == 0) || (map[Ox - 1][Oy] == 33 && map[Ox + 1][Oy] == 1) || (map[Ox - 1][Oy] == 1 && map[Ox + 1][Oy] == 33) || (map[Ox - 1][Oy] == 1 && map[Ox + 1][Oy] == 0)
                                          && (map[Ox][Oy + 1] == 1 && map[Ox][Oy - 1] == 0) || (map[Ox][Oy - 1] == 33 && map[Ox][Oy + 1] == 1) || (map[Ox][Oy - 1] == 1 && map[Ox][Oy + 1] == 33) || (map[Ox][Oy - 1] == 1 && map[Ox][Oy + 1] == 0))
                {
                    points.Add(new Vector3(Ox, Oy));
                }


                else if ((map[Ox + 1][Oy] == 1 && map[Ox - 1][Oy] == 33 && map[Ox][Oy + 1] == 33 && map[Ox][Oy - 1] == 33)
                    || (map[Ox + 1][Oy] == 33 && map[Ox - 1][Oy] == 1 && map[Ox][Oy + 1] == 33 && map[Ox][Oy - 1] == 33)
                    || (map[Ox + 1][Oy] == 33 && map[Ox - 1][Oy] == 33 && map[Ox][Oy + 1] == 1 && map[Ox][Oy - 1] == 33)
                    || (map[Ox + 1][Oy] == 33 && map[Ox - 1][Oy] == 33 && map[Ox][Oy + 1] == 33 && map[Ox][Oy - 1] == 1))
                {
                    points.Add(new Vector3(Ox, Oy));
                    flag = false;
                }
                else if (map[Ox][Oy] == 1
                && (map[Ox + 1][Oy] == 33 && map[Ox - 1][Oy] == 0)
                || (map[Ox - 1][Oy] == 33 && map[Ox + 1][Oy] == 0)
                || (map[Ox][Oy + 1] == 33 && map[Ox][Oy - 1] == 0)
                || (map[Ox][Oy - 1] == 33 && map[Ox][Oy + 1] == 0))
                {
                    if (!flag)
                    {
                        if (points.Count > 1)
                        {
                            AutoFloodFill();
                            flag = true;
                        }
                    }
                }
            }

            oldgridpos = gridpos;
        }

    }

    bool flag = false;

    void MoveEnemy()
    {
        if (!load) return;

        foreach (var ene in enemies)
        {
            if (!ene.gameObject.activeSelf) continue;

            int x = (Mathf.RoundToInt(ene.transform.position.x));
            int y = (Mathf.RoundToInt(ene.transform.position.y));

            if (x < 0)
            {
                x = 0;
            }
            if (x > xSize - 1)
            {
                x = xSize - 1;
            }
            if (y < 0)
            {
                y = 0;
            }
            if (y > ySize - 1)
            {
                y = ySize - 1;
            }

            if (map[x][y] == 33)
            {
                if ((y + 1 < ySize - 1 && map[x][y + 1] == 33) && (y - 1 > 0 && map[x][y - 1] == 33))
                {
                    ene.directionEnemy = new Vector2(ene.directionEnemy.x * -1, ene.directionEnemy.y);
                }
                else if ((x + 1 < xSize - 1 && map[x + 1][y] == 33) && (x - 1 > 0 && map[x - 1][y] == 33))
                {
                    ene.directionEnemy = new Vector2(ene.directionEnemy.x, ene.directionEnemy.y * -1);
                }
                else
                {
                    ene.directionEnemy = new Vector2(ene.directionEnemy.x * -1, ene.directionEnemy.y * -1);
                }
                ene.transform.Translate(ene.directionEnemy * Time.deltaTime * ene.speedEnemy, Space.World);

            }

            if (map[x][y] == 1)
            {
                Restart();
            }

            if (x > 0 && x < xSize - 1 && y > 0 && y < ySize - 1)
            {
                if (map[x][y] == 33 && map[x][y + 1] == 33 && map[x][y - 1] == 33 && map[x + 1][y] == 33 && map[x - 1][y] == 33)
                {
                    ene.gameObject.SetActive(false);
                }
            }

            ene.transform.Translate(ene.directionEnemy * Time.deltaTime * ene.speedEnemy, Space.World);
        }
    }

    void AutoFloodFill(bool check = true)
    {
        points.Add(new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0));

        CheckPoins(check);
        CreateMewBorder();
        points.Clear();
        direction = Vector2.zero;
    }
    int c = 0;

    private void CheckPoins(bool check = true)
    {
        c = 0;
        bool found = false;
        List<Vector3> poinsToCheck = new List<Vector3>();
        int fi = 0;
        if (check)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i].x <= 0 || points[i].x >= xSize - 1 || points[i].y <= 0 || points[i].y >= ySize - 1)
                    continue;

                int x = (int)points[i].x;
                int y = (int)points[i].y;

                if ((map[x + 1][y] == 1 && map[x - 1][y] == 1 && (map[x][y + 1] == 1 && map[x][y - 1] == 0 || map[x][y + 1] == 0 && map[x][y - 1] == 1))
                   || (map[x][y + 1] == 1 && map[x][y - 1] == 1 && (map[x + 1][y] == 1 && map[x - 1][y] == 0 || map[x + 1][y] == 0 && map[x - 1][y] == 1)))
                {
                    if (i - fi == 1)
                        continue;
                    fi = i;

                    poinsToCheck.Add(points[i]);
                    poinsToCheck.Add(points[i - 1]);
                    found = true;
                }      

                if ((map[x + 1][y] == 33 && map[x - 1][y] == 1) || (map[x - 1][y] == 33 && map[x + 1][y] == 1)
                || (map[x][y + 1] == 33 && map[x][y - 1] == 1) || (map[x][y - 1] == 33 && map[x][y + 1] == 1))
                {
                    if (++c == 0) continue;

                    if (i > 1)
                    {
                        poinsToCheck.Add(points[i - 1]);
                        poinsToCheck.Add(points[i]);
                        found = true;
                    }
                }

            }
        }

        CheckPoi(points.Last(), points[points.Count - 2]);

        if (found)
        {
            for (int i = 0; i < poinsToCheck.Count; i++)
            {
                if (i % 2 != 0)
                {
                    CheckPoi(poinsToCheck[i - 1], poinsToCheck[i]);
                }
            }
        }
    }

    private void CheckPoi(Vector3 p1, Vector3 p2)
    {
        var lastPoint = p1;
        var preLastPoint = p2;

        var difference = lastPoint - preLastPoint;
        var angle = Mathf.Atan2(difference.y, difference.x);
        var middlePoint = (lastPoint + preLastPoint) / 2;
        var middleLeft = middlePoint + new Vector3(Mathf.Cos(angle - Mathf.PI / 2) * 1.05f, Mathf.Sin(angle - Mathf.PI / 2) * 1.05f, 0f);
        var middleRight = middlePoint + new Vector3(Mathf.Cos(angle + Mathf.PI / 2) * 1.05f, Mathf.Sin(angle + Mathf.PI / 2) * 1.05f, 0f);


        var countLeft = TryToFill(Mathf.RoundToInt(middleLeft.x), Mathf.RoundToInt(middleLeft.y));
        var countRight = TryToFill(Mathf.RoundToInt(middleRight.x), Mathf.RoundToInt(middleRight.y));


        ClearFill(Mathf.RoundToInt(middleLeft.x), Mathf.RoundToInt(middleLeft.y));
        ClearFill(Mathf.RoundToInt(middleRight.x), Mathf.RoundToInt(middleRight.y));

        if (countLeft <= countRight)
            paintedPixels += countLeft;
        else
            paintedPixels += countRight;


        percentpainted = paintedPixels / (allPixel / 100f);
        ShowPercent(percentpainted, percentWin);


        if (percentpainted >= percentWin)
        {
            GameWin();
        }

        FloodFill(
                countLeft <= countRight ? Mathf.RoundToInt(middleLeft.x) : Mathf.RoundToInt(middleRight.x),
                countLeft <= countRight ? Mathf.RoundToInt(middleLeft.y) : Mathf.RoundToInt(middleRight.y)
                );



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

    public void FloodFill(int x, int y)
    {
        var color = tex.GetPixel(x, y);

        if (color == Color.green || color == Color.magenta || color == Color.black || x < 1 || x >= xSize - 1 || y < 1 || y >= ySize - 1)
            return;

        map[x][y] = 33;
        tex.SetPixel(x, y, Color.green);

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

    public virtual void ShowPercent(float CurrentPercent, float AllPercent) { }

    public virtual void GameOver() { }

    public virtual void ShowHeart(int count) { }

    public virtual void GameWin() { }

    public virtual void ShowLevel(int Level) { }

    public virtual void Timer(float time) { }
}
