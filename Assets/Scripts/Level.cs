using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level/Level")]
public class Level : ScriptableObject
{
    [Header("Settings")]
    public int gridHeight = 16;//how many tiles across
    public int gridWidth = 30;//how many tiles from top to bottom

    [Header("Level Generators")]
    public List<LevelGenerator> levelGenerators;
    public List<LevelGenerator> postStartLevelGenerators;
    public List<LevelGenerator> postRevealLevelGenerators;
}
