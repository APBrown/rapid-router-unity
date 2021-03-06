﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Road;
using Zenject;


public enum Direction : int
{
    North = 0,
    East = -90,
    South = -180,
    West = -270
};

public class Level
{
    public PathNode[] path;
    public OriginNode origin;

    public int[][] destinations;
    
    public Coordinate[] destinationCoords
    {
        get
        {
            Coordinate[] coordinates = new Coordinate[destinations.Length];
            for (int i = 0; i < destinations.Length; i++)
            {
                int[] dest = destinations[i];
                coordinates[i] = new Coordinate(dest[0], dest[1]);
            }
            return coordinates;
        }
    }
}

public class BoardManager : MonoBehaviour, IInitializable
{
    public float rows;
    public float columns;

    public static Level currentLevel;


    [Inject]
    Installer.Settings.FloorTiles tiles;

    [Inject]
    RoadDrawer roadDrawer;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    [Inject]
    BoardTranslator translator;

    [Inject]
    Installer.Settings.MapSettings mapDimensions;

    [PostInject]
    public void Initialize() {
        gridPositions = new List<Vector3>();
        rows = mapDimensions.rows;
        columns = mapDimensions.columns;
    }

    void InitialiseList()
    {
        gridPositions.Clear();
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    private void SetupLevel(int levelNumber)
    {
        currentLevel = LevelReader.ReadLevelFromFile(levelNumber);
        SetupBoard();
        roadDrawer.SetupOrigin(currentLevel.origin);
        GameObject[] roadObjects = roadDrawer.SetupRoadSegments(currentLevel.path);
        roadDrawer.SetupDestinations(currentLevel.destinationCoords);
        foreach (GameObject roadObject in roadObjects)
        {
            roadObject.isStatic = true;
            roadObject.transform.SetParent(boardHolder);
        }
    }

    private void SetupBoard()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject grassObject = Instantiate(tiles.grassTile, new Vector3(translator.translateRow(x), translator.translateColumn(y), 0f), Quaternion.identity) as GameObject;
                grassObject.isStatic = true;
                grassObject.transform.SetParent(boardHolder);
            }
        }
    }

    public void SetupScene(int level)
    {
        InitialiseList();
        boardHolder = new GameObject("Board").transform;
        SetupLevel(level);
    }

}


