using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    [SerializeField] GameObject[] tilesprefab;
    public Maps[] LevelScriptable;
    public Tiles.TileType[,] board { get; private set; }
    public Maps LevelToLoad;
    [SerializeField] GameObject[] armprefab;

    public int MaxMove;
    public int ActualMove;
    private Coroutine countMove;
    public List<Hand> HandCoord { get; private set; } = new List<Hand>();

    #region gestion de la partie
    public void EndChecker()
    {
        for (int row = 0; row < 10; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                if (board[row, col] == Tiles.TileType.Button)
                {
                    return;
                }
            }
        }
        Debug.Log("FINI");
        StopCoroutine(countMove);
    }


    private void Start()
    {
        LoadLevel(LevelToLoad);
    }

    IEnumerator CountMove(Maps LoadedLevel) 
    {
        yield return new WaitUntil(() => ActualMove > MaxMove);
        Debug.Log("NO MORE MOVE");

        LoadLevel(LoadedLevel);
    }
    #endregion
    #region boardManagement
    public void LoadLevel(Maps loadedLevel)
    {
        countMove = StartCoroutine(CountMove(loadedLevel));
        MaxMove = loadedLevel.MaxMovement;
        HandCoord.Clear();
        
        ActualMove = 0;
        board = new Tiles.TileType[10, 10];
        string level = loadedLevel.Level1;
        string[] levelLines = level.Split('\n');

        for (int row = 0; row < 10; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                //Debug.Log("actual row :" + row + "actual col : " + col);
                switch (levelLines[row][col])
                {
                    case 'F':
                        board[row, col] = Tiles.TileType.Floor;
                        break;
                    case 'B':
                        board[row, col] = Tiles.TileType.Box;
                        break;
                    case 'W':
                        board[row, col] = Tiles.TileType.Wall;
                        break;
                    case 'S':
                        board[row, col] = Tiles.TileType.Floor;
                        Debug.Log("actual row :" + row + "actual col : " + col);
                        player.GetComponent<PlayerManager>().SpawnOnMap(new Vector2Int(col, row));
                        break;
                    case '#':
                        board[row, col] = Tiles.TileType.Button;
                        break;
                    case 'O':
                        board[row,col] = Tiles.TileType.Oil;
                        break;
                    case 'A':
                        board[row,col] = Tiles.TileType.Button_Actif;
                        break;  
                }
            }
        }


        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        foreach (Transform childTransform in transform)
            Destroy(childTransform.gameObject);

        for (int row = 0; row < 10; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                if (board[row, col] == Tiles.TileType.Wall) //mur
                    Instantiate(tilesprefab[0],
                        new Vector2(col, -row),
                        Quaternion.identity,
                        transform);
                else if (board[row, col] == Tiles.TileType.Box) //boite
                    Instantiate(tilesprefab[2],
                        new Vector2(col, -row),
                        Quaternion.identity,
                        transform);
                else if (board[row, col] == Tiles.TileType.Button)  //bouton
                {
                    Instantiate(tilesprefab[3],
                    new Vector2(col, -row),
                    Quaternion.identity,
                    transform);
                }
                else if (board[row, col] == Tiles.TileType.Button_Actif)    // boite sur bouton
                {
                    Instantiate(tilesprefab[4],
                    new Vector2(col, -row),
                    Quaternion.identity,
                    transform);

                }

                else if (board[row,col] == Tiles.TileType.Oil)  //huile
                {
                    Instantiate(tilesprefab[7],
                    new Vector2(col, -row),
                    Quaternion.identity,
                    transform);

                }

                else if (board[row, col] == Tiles.TileType.Arm)    // bras
                {
                    Vector2Int coord = new Vector2Int(col, row);
                    //Debug.Log("coord hand index " + GetArmByCoord(coord) +" ==" + coord);
                    
                    Instantiate(
                        ArmVisuals(HandCoord[GetArmByCoord(coord)].arm),
                    new Vector2(col, -row),
                    Quaternion.identity,
                    transform);


                }

                else if (board[row, col] == Tiles.TileType.Arm_Last)    // dernier bras posé
                {
                    Tiles.ArmType last_ArmType = HandTiles.Coude(HandCoord[GetLastArmIndex(-1)].coord, player.GetComponent<PlayerManager>().position, HandCoord[GetLastArmIndex(0)].coord);
                    
                    Instantiate(

                        ArmVisuals(last_ArmType)



                        , new Vector2(col,-row)
                        , Quaternion.identity
                        , transform);
                    
                    Last_HandAddTiles(last_ArmType,GetLastArmIndex(0));
                }

                else        // sol
                {
                    Instantiate(tilesprefab[1],
                    new Vector2(col, -row),
                    Quaternion.identity,
                    transform);
                }
                   
            }
        }
    }

    #endregion
    #region Hand

    public GameObject ArmVisuals(Tiles.ArmType Arm)
    {
        if (Arm == Tiles.ArmType.Vertical)
        {
            //0
            return armprefab[0];
        }
        else if (Arm == Tiles.ArmType.Horizontal)
        {
            //1
            return armprefab[1];
        }
        else if (Arm == Tiles.ArmType.Left_To_Top)
        {
            //2
            return armprefab[2];
        }
        else if (Arm == Tiles.ArmType.Left_To_Bottom)
        {
            //3
            return armprefab[3];
        }
        
        else if (Arm == Tiles.ArmType.Right_To_Top )
        {
            //4
            return armprefab[4];
        }
        else if (Arm == Tiles.ArmType.Right_To_Bottom)
        {
            //5
            return armprefab[5];
        }
        else
        {
            Debug.Log("ARM DIR OBJECT NOT FOUND");
            return tilesprefab[5];
        }
    }


    #region listManagement
    public void HandManagement(Vector2Int BoardCoord,Tiles.TileType TileRecovered ,bool Adding)
    {
        if (Adding)
        {
            HandCoord.Add(new Hand(BoardCoord,TileRecovered));
        }
        else
        {
            foreach(Hand hand in HandCoord)
            {
                if(hand.coord == BoardCoord)
                {
                    Debug.Log("HAND REMOVE FIND");
                    board[hand.coord.y, hand.coord.x] = hand.tile;
                    HandCoord.Remove(hand);
                    break;
                }
            }
        }


        for (int i = 0; i < HandCoord.Count; i++) 
        {
            if (HandCoord[i] == HandCoord.Last<Hand>())
            {
                board[HandCoord[i].coord.y, HandCoord[i].coord.x] = Tiles.TileType.Arm_Last;
            }
            else 
            {
                if (board[HandCoord[i].coord.y, HandCoord[i].coord.x] != Tiles.TileType.Arm)
                {
                    board[HandCoord[i].coord.y, HandCoord[i].coord.x] = Tiles.TileType.Arm;
                }
            }
        }
        
    }

    public void Last_HandAddTiles(Tiles.ArmType armType,int index) 
    {
        HandCoord[index].arm = armType;
    }
    #endregion


    #region get Index
    public int GetLastArmIndex(int IndexWanted)
    {
        int index = -1;
        //recupere le dernier index
        for (int i = 0; i < HandCoord.Count; i++)
        {
            if (HandCoord[i] == HandCoord.Last<Hand>())
            {
                index = i;
                break;
            }
        }

        if (index +IndexWanted >= 0)
        {
            index += IndexWanted;
        }
        return index;
    }

    public int GetArmByCoord(Vector2Int Coord)
    {
        int index = 0;
        for (int i = 0; i < HandCoord.Count; i++)
        {
            if (HandCoord[i].coord == Coord)
            {
                //Debug.Log("Index Found By Coord");
                index = i;
                break;
            }
        }
        //Debug.Log ("Index wanted by coord ==" + index + "Coord == " +Coord);
        return index;
    }
    #endregion
    #endregion
}