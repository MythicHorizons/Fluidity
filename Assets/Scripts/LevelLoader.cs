using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelLoader
{
    public enum winCondition
    {
        Flood,
        Moves,
        Escort
    }

    public Cell[,] LoadLevel(string fileName)
    {
        return null;
        //Cell[,] result;
        //using (StreamReader reader = new StreamReader(@"Assets\Levels\" + fileName + ".txt"))
        //{
        //    var winConditions = (winCondition)reader.Read();
        //    var sizeX = reader.Read();
        //    var sizeY = reader.Read();
        //    Vector2 curPos = new Vector2(0, sizeY);
        //    result = new Cell[sizeX, sizeY];
        //    do
        //    {
        //        switch(reader.Read())
        //        {
        //            case '#':
        //                result
        //            break;
        //            case '@':
        //            break;
        //            default:
        //            break;
        //        }

        //        //check if we can increment x
        //        if(curPos.y + 1 == sizeY)
        //        {

        //        }

        //        //if x is at the end of row, increment Y and continue.

        //    } while (!reader.EndOfStream);
        //};
    }
}
