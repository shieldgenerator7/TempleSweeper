﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLineGenerator : LevelGenerator
{
    [Header("Settings")]
    public int mapLineDistMin = 3;
    public int mapLineDistMax = 5;
    [Header("Objects")]
    public GameObject linePrefab;
    public ObjectGenerator mapGenerator;

    private List<Vector2> mapPath;
    private int mapLineSegmentRevealedCount = 0;
    private List<GameObject> drawnLines = new List<GameObject>();

    public override void generate(GameObject[,] tileMap)
    {
        throw new System.NotImplementedException();
    }

    public override void generatePostStart(GameObject[,] tileMap, int posX, int posY)
    {
        drawnLines = new List<GameObject>();
        int curX = posX;
        int curY = posY;
        int prevX = curX - 1;//set to one less to avoid infinite loop
        int prevY = curY - 1;
        //Make new path
        while (true)
        {
            curX = posX;
            curY = posY;
            mapPath = new List<Vector2>();
            mapPath.Add(new Vector2(curX, curY));
            for (int i = 0; i < mapGenerator.amount; i++)
            {
                int newX = curX;
                int newY = curY;
                while (true)
                {
                    newX = curX;
                    newY = curY;
                    int randBool = Random.Range(0, 2);
                    int randDist = Random.Range(mapLineDistMin, mapLineDistMax + 1);
                    randDist *= (Random.Range(0, 2) == 1) ? 1 : -1;
                    if (randBool == 1)
                    {
                        newX += randDist;
                    }
                    else
                    {
                        newY += randDist;
                    }
                    //Check to see if this new point is valid
                    if (
                        (newX != prevX && newY != prevY)
                        && (newX != curX || newY != curY)
                        && inBounds(tileMap, newX, newY)
                        && tileMap[newX, newY] != null
                    )
                    {
                        bool hallClear = true;
                        //Check to make sure the new point is
                        //not in the same column or row as another point of the line
                        for (int j = 0; j < mapPath.Count; j++)
                        {
                            Vector2 point = mapPath[i];
                            if (point.x == curX && point.y == curY)
                            {
                                //Don't test against the current position
                                continue;
                            }
                            //Check to make sure the row and column is clear
                            if (point.x == newX || point.y == newY)
                            {
                                hallClear = false;
                                break;
                            }
                        }
                        if (hallClear)
                        {
                            //Break out of the while loop
                            break;
                        }
                    }
                }
                prevX = curX;
                prevY = curY;
                curX = newX;
                curY = newY;
                mapPath.Add(new Vector2(curX, curY));
            }
            //Test to see if the path is acceptable
            Vector2 theSpot = mapPath[mapPath.Count - 1];
            LevelTile spotTile = tileMap[(int)theSpot.x, (int)theSpot.y]?.GetComponent<LevelTile>();
            //If the spot is on land
            if (spotTile != null
                //And the spot is not a treasure, mine, or map fragment
                && spotTile.Available)
            {
                bool noOverlap = true;
                //Check to make sure the spot is not on another point of the line
                for (int i = 0; i < mapPath.Count - 1; i++)
                {
                    Vector2 point = mapPath[i];
                    if (point == theSpot)
                    {
                        noOverlap = false;
                        break;
                    }
                }

                bool noOverlapInBetween = true;
                for (int i = 0; i < mapPath.Count - 1; i++)
                {
                    Vector2 point = mapPath[i];
                    for (int j = 0; j < i - 1; j++)
                    {
                        Vector2 point2 = mapPath[j];
                        if (point.x == point2.x
                            || point.y == point2.y)
                        {
                            noOverlapInBetween = false;
                            break;
                        }
                    }
                    if (!noOverlapInBetween)
                    {
                        break;
                    }
                }

                bool acceptable = noOverlap && noOverlapInBetween;
                if (acceptable)
                {
                    //Break out of the while loop
                    break;
                }
                else
                {
                    //Continue through to the next random iteration
                    continue;
                }
            }
            else
            {
                continue;
            }
        }

        //Acceptable path generated,
        //reserve the path so no mines get generated on it
        for (int n = 0; n < mapPath.Count - 1; n++)
        {
            int rx = (int)mapPath[n].x;
            int ry = (int)mapPath[n].y;
            int rx2 = (int)mapPath[n + 1].x;
            int ry2 = (int)mapPath[n + 1].y;
            int ix = (int)Mathf.Sign(rx2 - rx);
            int iy = (int)Mathf.Sign(ry2 - ry);
            for (int x = rx; x != rx2 + ix; x += ix)
            {
                for (int y = ry; y != ry2 + iy; y += iy)
                {
                    if (tileMap[x, y])
                    {
                        LevelTile lt = tileMap[x, y].GetComponent<LevelTile>();
                        if (lt.Available)
                        {
                            lt.tileType = LevelTile.TileType.RESERVED;
                        }
                    }
                }
            }
        }

        Managers.Start.SetActive(true);
        Managers.Start.transform.position = LevelManager.getWorldPos(posX, posY);
    }

    public override void generatePostReveal(GameObject[,] tileMap, LevelTile.TileType tileType)
    {
        if (tileType == LevelTile.TileType.MAP)
        {
            GameObject line = Instantiate(linePrefab);
            Vector2 startPos = LevelManager.getWorldPos(mapPath[mapLineSegmentRevealedCount]);
            Vector2 endPos = LevelManager.getWorldPos(mapPath[mapLineSegmentRevealedCount + 1]);
            line.transform.position = startPos;
            line.transform.right = (endPos - startPos);
            line.GetComponent<SpriteRenderer>().size = new Vector2((startPos - endPos).magnitude, 1);
            drawnLines.Add(line);
            mapLineSegmentRevealedCount++;
            if (mapLineSegmentRevealedCount == mapGenerator.amount)
            {
                Managers.End.SetActive(true);
                Managers.End.transform.position = endPos;
            }
        }
    }

    public override void clearGeneratedObjects()
    {
        mapLineSegmentRevealedCount = 0;
        //Clear old path objects
        foreach (GameObject go in drawnLines)
        {
            Destroy(go);
        }
        drawnLines.Clear();

        Managers.Start.SetActive(false);
        Managers.End.SetActive(false);
    }
}
