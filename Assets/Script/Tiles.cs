using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class Tiles
{
    public enum TileType
    {
        Floor,
        Wall,
        Box,
        Button,
        Button_Actif,
        Oil,
        Arm,
        Arm_Last,
    }

    public enum Direction
    {
        top, bottom, left, right,
    }

    public enum ArmType
    {
        Horizontal,
        Vertical,
        Left_To_Top,
        Right_To_Top,
        Left_To_Bottom,
        Right_To_Bottom,
    }
}

public class Hand
{
    public Vector2Int coord;
    public Tiles.TileType tile;
    public Tiles.ArmType arm;
    public  Hand (Vector2Int BoardCoord, Tiles.TileType HandRecovered, Tiles.ArmType armtype = Tiles.ArmType.Horizontal)
    {
        coord = BoardCoord;
        tile = HandRecovered;
        arm = armtype;
    }
}

public static class HandTiles
{
    public static Tiles.ArmType Coude(Vector2Int Depart,Vector2Int Arriver,Vector2Int coude)
    {


        //regarde les coordonnées arriver et de depart
        //Debug.Log("Depart.x =" + Depart.x + "| Arriver.x =" + Arriver.x);
        //Debug.Log("Depart.y =" + Depart.y + "| Arriver.y =" + Arriver.y);

        if (Depart.x == Arriver.x) // deplacement vertical
        { return Tiles.ArmType.Vertical; }
        else if(Depart.y == Arriver.y) //deplacement horizontal
        { return Tiles.ArmType.Horizontal; }

        else if(coude.x > Depart.x) // on descends
        {
            if(Arriver.y > coude.y) // on va a droite
            {
                return Tiles.ArmType.Left_To_Bottom;
            }
            else 
            {
                return Tiles.ArmType.Left_To_Top;
            }
        }
        else if(coude.x < Depart.x)
        {
            if(Arriver.y > coude.y)
            {
                return Tiles.ArmType.Right_To_Bottom;
            }
            else
            {
                return Tiles.ArmType.Right_To_Top;
            }
        }

        else if(coude.x > Arriver.x)
        {
            if(Depart.y > coude.y)
            {
                return Tiles.ArmType.Left_To_Bottom;
            }
            else
            {
                return Tiles.ArmType.Left_To_Top;
            }
        }

        else
        {
            if (Depart.y > coude.y)
            {
                return Tiles.ArmType.Right_To_Bottom;
            }
            else
            {
                return Tiles.ArmType.Right_To_Top;
            }
        }
    }
}