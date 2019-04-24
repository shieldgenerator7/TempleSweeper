using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Settings")]
    public int gridHeight = 16;//how many tiles across
    public int gridWidth = 30;//how many tiles from top to bottom

    [Header("Level Generators")]
    public List<LevelGenerator> levelGenerators;
    public List<LevelGenerator> postStartLevelGenerators;
    public List<LevelGenerator> postRevealLevelGenerators;
}
