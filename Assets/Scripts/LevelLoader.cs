using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public enum LevelLoaderType
    {
        LoadFromFile,
        RandomGrid
    }

    private string _filePath;
    private string _fileName;

    public LevelLoaderType LevelLoaderMode;
    public Object LevelDataAsFile;
    public int MapWidth = 10;
    public int MapHeight = 10;

    public Cell startingPrefab = new Cell();
    public Cell[] Prefabs = new Cell[0];
    public Cell WallPrefab = new Cell();
    public Cell[,] CellMap { get; private set; }
    public List<Cell> Cells { get; private set; }
    public Material StartingCellMaterial { get; private set; }

    public bool AutoLoadLevelOnStart = true;
    public int RandomSeed = 0; //TODO: When using Random use Unity.Random for its range and its pre-included in package so deploy is smaller.
    private Dictionary<string, System.Func<Cell>> mapCellGenFuncs;

    public string GetFilePath()
    {
        if (string.IsNullOrWhiteSpace(_filePath))
        {
            //$"{Application.dataPath}/Levels/{fileName}.txt"
            _filePath = AssetDatabase.GetAssetPath(LevelDataAsFile);
        }

        return _filePath;
    }

    public string GetFileName()
    {
        if (string.IsNullOrWhiteSpace(_fileName))
        {
            _fileName = Path.GetFileName(GetFilePath());
        }

        return _fileName;
    }

    public static class CellType
    {
        public const string StartPos = "@";
        public const string RandomColor = ".";
        public const string Color0 = "0";
        public const string Color1 = "1";
        public const string Color2 = "2";
        public const string Color3 = "3";
        public const string Color4 = "4";
        public const string Color5 = "5";
        public const string Color6 = "6";
        public const string Color7 = "7";
        public const string Color8 = "8";
        public const string Color9 = "9";
        public const string Wall = "#";
    }

    public void Awake()
    {
        mapCellGenFuncs = new Dictionary<string, System.Func<Cell>>()
        {
            {CellType.StartPos, GenerateStartingCell},
            {CellType.RandomColor, GenerateRandomColorCell},
            {CellType.Color0, GenerateColor0Cell},
            {CellType.Color1, GenerateColor1Cell},
            {CellType.Color2, GenerateColor2Cell},
            {CellType.Color3, GenerateColor3Cell},
            {CellType.Color4, GenerateColor4Cell},
            {CellType.Color5, GenerateColor5Cell},
            {CellType.Color6, GenerateColor6Cell},
            {CellType.Color7, GenerateColor7Cell},
            {CellType.Color8, GenerateColor8Cell},
            {CellType.Color9, GenerateColor9Cell},
            {CellType.Wall, GenerateWallCell}
        };

        Cells = new List<Cell>();
    }

    public void Start()
    {
        if (AutoLoadLevelOnStart)
        {
            switch (LevelLoaderMode)
            {
                case LevelLoaderType.LoadFromFile:
                    CellMap = GenerateLevelFromFile();
                    break;
                default:
                    CellMap = GenerateRandomGrid();
                    break;
            }
        }
    }

    public virtual Cell GenerateRandomColorCell() { return GenerateColorCell(Random.Range(0, Prefabs.Length)); }
    public virtual Cell GenerateColor0Cell() { return GenerateColorCell(0); }
    public virtual Cell GenerateColor1Cell() { return GenerateColorCell(1); }
    public virtual Cell GenerateColor2Cell() { return GenerateColorCell(2); }
    public virtual Cell GenerateColor3Cell() { return GenerateColorCell(3); }
    public virtual Cell GenerateColor4Cell() { return GenerateColorCell(4); }
    public virtual Cell GenerateColor5Cell() { return GenerateColorCell(5); }
    public virtual Cell GenerateColor6Cell() { return GenerateColorCell(6); }
    public virtual Cell GenerateColor7Cell() { return GenerateColorCell(7); }
    public virtual Cell GenerateColor8Cell() { return GenerateColorCell(8); }
    public virtual Cell GenerateColor9Cell() { return GenerateColorCell(9); }
    public virtual Cell GenerateColorCell(int prefabIndex)
    {
        if (prefabIndex < 0) prefabIndex = 0;
        if (prefabIndex >= Prefabs.Length) prefabIndex = Prefabs.Length - 1;

        GameObject prefab = Prefabs[prefabIndex].gameObject;
        var newObj = Instantiate(prefab);
        var newCell = newObj.GetComponent<Cell>();
        return newCell;
    }

    public virtual Cell GenerateStartingCell()
    {
        GameObject prefab = startingPrefab.gameObject;
        var newObj = Instantiate(prefab);
        var newCell = newObj.GetComponent<Cell>();
        newCell.Capture(false);
        return newCell;
    }

    public virtual Cell GenerateWallCell()
    {
        var newObj = Instantiate(WallPrefab.gameObject);
        var newCell = newObj.GetComponent<Cell>();
        newCell.canCapture = false;
        return newCell;
    }

    public Cell TryGetNorthCell(Cell cell)
    {
        if (cell.cellPos.y <= 0) return null;
        return GetCellAt((int)cell.cellPos.x, (int)cell.cellPos.y - 1);
    }

    public Cell TryGetSouthCell(Cell cell)
    {
        if (cell.cellPos.y >= MapHeight-1) return null;
        return GetCellAt((int)cell.cellPos.x, (int)cell.cellPos.y + 1);
    }

    public Cell TryGetEastCell(Cell cell)
    {
        if (cell.cellPos.x >= MapWidth-1) return null;
        return GetCellAt((int)cell.cellPos.x + 1, (int)cell.cellPos.y);
    }

    public Cell TryGetWestCell(Cell cell)
    {
        if (cell.cellPos.x <= 0) return null;
        return GetCellAt((int)cell.cellPos.x - 1, (int)cell.cellPos.y);
    }

    public Cell[] GetNeighborCells(Cell cell)
    {
        var result = new List<Cell>();
        var northCell = TryGetNorthCell(cell);
        var southCell = TryGetSouthCell(cell);
        var eastCell = TryGetEastCell(cell);
        var westCell = TryGetWestCell(cell);

        if(northCell != null) result.Add(northCell);
        if (southCell != null) result.Add(southCell);
        if (eastCell != null) result.Add(eastCell);
        if (westCell != null) result.Add(westCell);
        return result.ToArray();
    }

    public Cell GetCellAt(int x, int y)
    {
        return CellMap[y, x];
    }

    public Cell GetCellAt(Vector2 cellPos)
    {
        return GetCellAt((int)cellPos.x, (int)cellPos.y);
    }

    public Cell[,] GenerateLevelFromFile() { return GenerateLevelFromFile(null); }

    public Cell[,] GenerateLevelFromFile(System.Random rng)
    {
        Cell[,] result = null;
        bool isMap = false;

        //TODO Random Seed needs to work with GenerateRandomColorCell();
        ////Initialize using seed
        //if (rng == null && RandomSeed <= 0) rng = new System.Random();
        //else if (rng == null) rng = new System.Random(RandomSeed);

        ////If not initialize, use time-based Random
        //if (rng == null) rng = new System.Random();

        string input = File.ReadAllText(GetFilePath());
        string[] f = input.Split(new string[] { "\n", "\r", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        if (f[0].Length > 0 && f[1].Length > 0)
        {
            int width, height = 0;
            int.TryParse(f[0], out width);
            int.TryParse(f[1], out height);
            MapWidth = width;
            MapHeight = height;
        }

        if (MapWidth > 0 && MapHeight > 0)
        {
            result = new Cell[MapHeight, MapWidth];
            int y = 0, x = 0;
            int cellIndex = 0;
            StartingCellMaterial = Prefabs[0].GetComponent<CellColor>().targetMaterial;
            foreach (var row in input.Split('\n'))
            {
                x = 0;
                foreach (var symbol in row.Trim().Split(' '))
                {
                    System.Func<Cell> cellGenFunc;
                    var isValueFound = mapCellGenFuncs.TryGetValue(symbol, out cellGenFunc);
                    if (isValueFound)
                    {
                        var newCell = cellGenFunc();
                        newCell.SetCellPos(x, y);
                        newCell.cellIndex = cellIndex;
                        cellIndex++;
                        newCell.transform.SetPositionAndRotation(new Vector3(x - MapWidth / 2, MapHeight - (y - MapHeight / 2), 0),
                                                Quaternion.Euler(0, 0, 0));

                        Cells.Add(newCell);
                        result[y, x] = newCell;

                        x++;
                        isMap = true;
                    }
                    else { isMap = false; }
                }
                if (isMap == true) { y++; }
            }

            CellMap = result;
        }

        return result;
    }

    public Cell[,] GenerateRandomGrid()
    {
        var result = new Cell[MapWidth, MapHeight];
        var cellIndex = 0;
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapWidth; y++)
            {
                Cell newCell = null;
                if (y == 0 && x == 0)
                    newCell = GenerateStartingCell();
                else
                    newCell = GenerateRandomColorCell();

                newCell.SetCellPos(x, y);
                newCell.cellIndex = cellIndex;
                cellIndex++;
                newCell.transform.SetPositionAndRotation(new Vector3(x-MapWidth/2,y-MapHeight/2, 0),
                                        Quaternion.Euler(0, 0, 0));
                Cells.Add(newCell);
                result[y, x] = newCell;
            }
        }

        CellMap = result;
        return result;
    }
}
