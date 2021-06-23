using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    /// <summary>
    /// The cells that should be created using this generator.
    /// </summary>
    public Cell[] prefabs = new Cell[0];
    public Cell wallPrefab = new Cell();
    public bool autoLoadLevelOnStart = true;
    public int RandomSeed = 0;
    private Dictionary<string, System.Func<Cell>> mapCellGenFuncs;
    private Cell RunCellGenFunc(System.Func<Cell> cellGenFunc) => cellGenFunc();

    public void Start()
    {
        mapCellGenFuncs = new Dictionary<string, System.Func<Cell>>()
        {
            {"@", GenerateStartingCell},
            {".", GenerateRandomColorCell},
            {"#", GenerateWallCell}
        };

        if(autoLoadLevelOnStart)
        {
            GenerateLevelFromFile("Level1");
        }
    }


    public virtual Cell GenerateRandomColorCell()
    {
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)].gameObject;
        var newObj = Instantiate(prefab);
        return newObj.GetComponent<ColorCell>();
    }

    public virtual Cell GenerateStartingCell()
    {
        var newCell = GenerateRandomColorCell();
        newCell.captured = true;
        return new Cell();
    }

    public virtual Cell GenerateWallCell()
    {
        var newObj = Instantiate(wallPrefab.gameObject);
        var newCell = newObj.GetComponent<Cell>();
        newCell.canCapture = false;
        return new Cell();
    }

    public Cell[,] GenerateLevelFromFile(string fileName) => GenerateLevelFromFile(fileName, null);

    public Cell[,] GenerateLevelFromFile(string fileName, System.Random rng)
    {
        int mapHeight = 0;
        int mapWidth = 0;
        Cell[,] result = null;
        bool isMap = false;

        //If not initialize, use time-based Random
        if (rng == null) rng = new System.Random();

        string input = File.ReadAllText($"{Application.dataPath}/Levels/{fileName}.txt");
        string[] f = input.Split(new string[] { "\n", "\r", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        if (f[0].Length > 0 && f[1].Length > 0)
        {
            int.TryParse(f[0], out mapHeight);
            int.TryParse(f[1], out mapWidth);
        }

        if (mapWidth > 0 && mapHeight > 0)
        {
            result = new Cell[mapHeight, mapWidth];
            int y = 0, x = 0;
            foreach (var row in input.Split('\n'))
            {
                x = 0;
                foreach (var symbol in row.Trim().Split(' '))
                {
                    System.Func<Cell> cellGenFunc;
                    var isValueFound = mapCellGenFuncs.TryGetValue(symbol,out cellGenFunc);
                    if (isValueFound)
                    {
                        var newCell = RunCellGenFunc(cellGenFunc);
                        result[y, x] = newCell;
                        GameObject.Instantiate(newCell, new Vector3(x, mapHeight - y, 0),
                                                Quaternion.Euler(0, 0, 0));
                        x++;
                        isMap = true;
                    }
                    else { isMap = false; }
                }
                if (isMap == true) { y++; }
            }
        }

        return result;
    }
}
