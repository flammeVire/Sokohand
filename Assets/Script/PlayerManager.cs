using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public Vector2Int position { get; private set; }
    private Vector2Int InputDirection = new Vector2Int(0,0);
    public void SpawnOnMap(Vector2Int startPosition)
    {
        position = startPosition;
        transform.position = new Vector2(position.x, -position.y);
    }

    private void Update()
    {
        Vector2Int desiredPosition = position;
        if (gameManager.board[position.y, position.x] == Tiles.TileType.Oil)
        {
            desiredPosition = new Vector2Int(position.x + InputDirection.x, position.y + InputDirection.y);
            PlayerOnOil(desiredPosition);
        }

        
        InputDirection = new Vector2Int(0, 0);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            InputDirection = new Vector2Int(0, - 1);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            InputDirection = new Vector2Int(0, + 1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            InputDirection = new Vector2Int(-1,0);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            InputDirection = new Vector2Int( + 1, 0);

        desiredPosition = new Vector2Int (position.x + InputDirection.x, position.y+InputDirection.y);
        if (desiredPosition != position)
        {
            //si joueur arrivera dans la zone de jeu
            if (desiredPosition.x <= 9 && desiredPosition.y <= 9 && desiredPosition.x >= 0 && desiredPosition.y >= 0)
            {

                // Cas sol
                //si joueur  arrivera sur un sol / bouton
                if (gameManager.board[desiredPosition.y, desiredPosition.x] == Tiles.TileType.Floor || gameManager.board[desiredPosition.y, desiredPosition.x] == Tiles.TileType.Button)
                {
                    gameManager.ActualMove++;
                    gameManager.HandManagement(position, gameManager.board[position.y, position.x], true);  //ajoute la main et l'ancienne tuile
                    gameManager.board[position.y, position.x] = Tiles.TileType.Arm_Last;
                    position = desiredPosition;
                    transform.position = new Vector2(position.x, -position.y);
                }

                // Cas boite
                //si joueur arrivera sur une boite // bouton avec boite
                else if (gameManager.board[desiredPosition.y, desiredPosition.x] == Tiles.TileType.Box
                  || gameManager.board[desiredPosition.y, desiredPosition.x] == Tiles.TileType.Button_Actif)
                {
                    
                    Vector2Int desiredBoxPosition = desiredPosition + (desiredPosition - position);
                    Tiles.TileType desiredBoxTileType = gameManager.board[desiredBoxPosition.y, desiredBoxPosition.x];

                    if (desiredBoxPosition.x <= 9 && desiredBoxPosition.y <= 9 && desiredBoxPosition.x >= 0 && desiredBoxPosition.y >= 0
                        && (desiredBoxTileType == Tiles.TileType.Floor || desiredBoxTileType == Tiles.TileType.Button))
                    {
                        gameManager.ActualMove++;
                        Vector2Int lastPos = position;
                        position = desiredPosition;
                        transform.position = new Vector2(position.x, -position.y);
                        gameManager.HandManagement(lastPos, Tiles.TileType.Floor, true);  //ajoute la main et l'ancienne tuile
                        gameManager.board[lastPos.y, lastPos.x] = Tiles.TileType.Arm_Last;
                        // Transformation case de d�part en last hand
                        if (gameManager.board[desiredPosition.y, desiredPosition.x] == Tiles.TileType.Box)// Boite simple
                        {
                            

                            gameManager.board[desiredPosition.y, desiredPosition.x] = Tiles.TileType.Floor;
                        }
                        else // Boite �tait sur un switch
                        {
                            gameManager.board[desiredPosition.y, desiredPosition.x] = Tiles.TileType.Button;
                        }
                        // Transformation case d'arriv�e
                        if (desiredBoxTileType == Tiles.TileType.Floor) // Je pousse la boite vers un sol
                        {
                            gameManager.board[desiredBoxPosition.y, desiredBoxPosition.x] = Tiles.TileType.Box;
                        }
                        else // Je pousse la boite vers un switch
                        {
                            gameManager.board[desiredBoxPosition.y, desiredBoxPosition.x] = Tiles.TileType.Button_Actif;
                            gameManager.EndChecker();
                        }

                    }

                }
                //Cas Bras
                //si joueur arrivera sur son dernier bras
                else if (gameManager.board[desiredPosition.y, desiredPosition.x] == Tiles.TileType.Arm_Last)
                {
                    //enleve un mouvement
                    gameManager.ActualMove--;

                    //dernier bras ajouter = ancien bras
                    position = desiredPosition;
                    transform.position = new Vector2(position.x, -position.y);
                    gameManager.HandManagement(position, gameManager.board[position.x, position.y], false);  //ajoute la main et l'ancienne tuile

                    //actuel ancien bras = ancienne tiles


                }

                //Cas Huile
                //si joueur arrivera sur de l'huile
                else if(gameManager.board[desiredPosition.y,desiredPosition.x] == Tiles.TileType.Oil)
                {
                    PlayerOnOil(desiredPosition);
                }

                gameManager.UpdateVisuals();
            }
        }

        
    }

    private void PlayerOnOil(Vector2Int desiredPosition)
    {
        Debug.Log("player is on oil");
        gameManager.ActualMove++;
        gameManager.HandManagement(position, gameManager.board[position.y, position.x], true);  //ajoute la main et l'ancienne tuile
        gameManager.board[position.y, position.x] = Tiles.TileType.Arm_Last;
        position = desiredPosition;
        transform.position = new Vector2(position.x, -position.y);
        

    }
}
