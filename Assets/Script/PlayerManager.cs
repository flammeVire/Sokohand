using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public Vector2Int position { get; private set; }
    private Vector2Int InputDirection = new Vector2Int(0,0);
    [SerializeField] Sprite[] sprites;
    bool isWalking;
    public void SpawnOnMap(Vector2Int startPosition)
    {
        position = startPosition;
        transform.position = new Vector2(position.x, -position.y);
    }

    private void Update()
    {
        if (!isWalking && gameManager.Running)
        {
            Vector2Int desiredPosition = position;
            if (gameManager.board[position.y, position.x] == Tiles.TileType.Oil)
            {
                desiredPosition = new Vector2Int(position.x + InputDirection.x, position.y + InputDirection.y);
                PlayerOnOil(desiredPosition);
                //gameManager.UpdateVisuals();
            }

            #region movement management
            InputDirection = new Vector2Int(0, 0);


            if (Input.GetButtonDown("Vertical"))
            {
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    InputDirection = new Vector2Int(0, -1);
                }
                else if (Input.GetAxisRaw("Vertical") < 0)
                {
                    InputDirection = new Vector2Int(0, +1);
                }
            }
            else if (Input.GetButtonDown("Horizontal"))
            {
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    InputDirection = new Vector2Int(+1, 0);
                }
                else if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    InputDirection = new Vector2Int(-1, 0);
                }
            }


            desiredPosition = new Vector2Int(position.x + InputDirection.x, position.y + InputDirection.y);
            #endregion

            if (desiredPosition != position)
            {

                //si joueur arrivera dans la zone de jeu
                if (desiredPosition.x <= 9 && desiredPosition.y <= 9 && desiredPosition.x >= 0 && desiredPosition.y >= 0)
                {

                    // Cas sol
                    //si joueur  arrivera sur un sol / bouton
                    if (gameManager.board[desiredPosition.y, desiredPosition.x] == Tiles.TileType.Floor 
                        || gameManager.board[desiredPosition.y, desiredPosition.x] == Tiles.TileType.Button)
                    {
                        gameManager.ActualMove++;
                        gameManager.GlobalMove++;
                        Debug.Log("Adding : " + gameManager.board[position.y, position.x]);

                        gameManager.HandManagement(position, gameManager.board[position.y, position.x], true);
                        gameManager.board[position.y, position.x] = Tiles.TileType.Arm_Last;
                        StartCoroutine(WalkAnimation(desiredPosition, true));
                        position = desiredPosition;
                        
                        //RotateHand();


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
                            gameManager.GlobalMove++;
                            Vector2Int lastPos = position;
                            position = desiredPosition;
                            StartCoroutine(WalkAnimation(desiredPosition,true));
                            // Transformation case de départ en last hand

                            if (gameManager.board[desiredPosition.y, desiredPosition.x] == Tiles.TileType.Box)// Boite simple
                            {
                                /*
                                Debug.Log("Adding : " + gameManager.board[position.y, position.x]);
                                
*/
                                if (gameManager.board[lastPos.y,lastPos.x] == Tiles.TileType.Floor)
                                {
                                    gameManager.HandManagement(lastPos, Tiles.TileType.Floor, true);
                                    gameManager.board[lastPos.y, lastPos.x] = Tiles.TileType.Arm_Last;

                                    gameManager.board[desiredPosition.y, desiredPosition.x] = Tiles.TileType.Floor;
                                }
                                else
                                {
                                    gameManager.HandManagement(lastPos, Tiles.TileType.Button, true);
                                    gameManager.board[lastPos.y, lastPos.x] = Tiles.TileType.Arm_Last;

                                    gameManager.board[desiredPosition.y, desiredPosition.x] = Tiles.TileType.Floor;
                                }

                            }
                            else // Boite était sur un switch
                            {
                                gameManager.HandManagement(lastPos, Tiles.TileType.Floor, true);
                                gameManager.board[lastPos.y, lastPos.x] = Tiles.TileType.Arm_Last;
                                gameManager.ButtonActifInGame -= 1;
                                gameManager.board[desiredPosition.y, desiredPosition.x] = Tiles.TileType.Button;
                            }

                            // Transformation case d'arrivée
                            if (desiredBoxTileType == Tiles.TileType.Floor) // Je pousse la boite vers un sol
                            {
                                gameManager.board[desiredBoxPosition.y, desiredBoxPosition.x] = Tiles.TileType.Box;
                            }
                            else // Je pousse la boite vers un switch
                            {
                                gameManager.board[desiredBoxPosition.y, desiredBoxPosition.x] = Tiles.TileType.Button_Actif;
                                gameManager.ButtonActifInGame += 1;
                                gameManager.EndChecker();
                                
                            }
                            StartCoroutine(WalkAnimation(desiredPosition, true));
                        }

                    }
                    //Cas Bras
                    //si joueur arrivera sur son dernier bras
                    else if (gameManager.board[desiredPosition.y, desiredPosition.x] == Tiles.TileType.Arm_Last)
                    {
                        gameManager.ActualMove--;
                        
                        StartCoroutine(WalkAnimation(desiredPosition,false));
                        

                    }

                    //Cas Huile
                    //si joueur arrivera sur de l'huile
                    else if (gameManager.board[desiredPosition.y, desiredPosition.x] == Tiles.TileType.Oil)
                    {
                        PlayerOnOil(desiredPosition);
                    }


                }
                gameManager.UpdateVisuals();


            }
        }
        
    }

    #region hand transform
    IEnumerator WalkAnimation(Vector2Int desiredPosition,bool AutoRotate)
    {
        isWalking = true;
        Vector2 inputmidposition = InputDirection;
        if (InputDirection.x > 0)
        {
            inputmidposition = new Vector2(0.5f, 0);
        }
        else if (InputDirection.x < 0)
        {
            inputmidposition = new Vector2(-0.5f, 0);
        }
        else if (InputDirection.y > 0)
        {
            inputmidposition = new Vector2(0, 0.5f);
        }
        else if (InputDirection.y < 0)
        {
            inputmidposition = new Vector2(0, -0.5f);
        }

        Vector2 midposition = new Vector2(desiredPosition.x - inputmidposition.x, desiredPosition.y - inputmidposition.y);


        this.GetComponent<SpriteRenderer>().sprite = sprites[1];
        transform.position = new Vector2(midposition.x, -midposition.y);
        
        if (AutoRotate)
        {
            RotateHand();
        }
        //gameManager.UpdateVisuals();
        
        yield return new WaitForSeconds(0.1f);

        if (!AutoRotate)
        {
            position = desiredPosition;
            HandAttachToArm();
            gameManager.UpdateVisuals();
        }

        transform.position = new Vector2(desiredPosition.x, -desiredPosition.y);
        
        this.GetComponent<SpriteRenderer>().sprite = sprites[0];
        isWalking = false;
    }

    void RotateHand()
    {
        
        if (InputDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);

        }
        else if (InputDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (InputDirection.y > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (InputDirection.y < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

    }

    public void HandAttachToArm()
    {
        if (gameManager.HandCoord[gameManager.GetLastArmIndex(0)].arm == Tiles.ArmType.Right_To_Top) 
        {
            if (InputDirection.x != 0)
            {
                
                transform.rotation = Quaternion.Euler(0, 0, 180);

            }
            else if (InputDirection.y != 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 90);
                
            }
        }
        else if (gameManager.HandCoord[gameManager.GetLastArmIndex(0)].arm == Tiles.ArmType.Right_To_Bottom) 
        {

            if (InputDirection.x != 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                
            }
            else if (InputDirection.y != 0)
            {
                
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
        }
        else if (gameManager.HandCoord[gameManager.GetLastArmIndex(0)].arm == Tiles.ArmType.Left_To_Top) 
        {

            if (InputDirection.x != 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 180);
                
            }
            else if (InputDirection.y != 0)
            {
                
                transform.rotation = Quaternion.Euler(0, 0, 270);
            }
        }
        else if (gameManager.HandCoord[gameManager.GetLastArmIndex(0)].arm == Tiles.ArmType.Left_To_Bottom) 
        {

            if (InputDirection.x != 0)
            {
                
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (InputDirection.y != 0)
            {
                
                transform.rotation = Quaternion.Euler(0, 0, 270);
            }
        }

        gameManager.HandManagement(position, gameManager.board[position.y, position.x], false);  //retire la main et l'ancienne tuile
    }

    #endregion
    private void PlayerOnOil(Vector2Int desiredPosition)
    {
        Debug.Log("player is on oil");
        gameManager.ActualMove++;
        gameManager.GlobalMove++;
        gameManager.HandManagement(position, gameManager.board[position.y, position.x], true);  //ajoute la main et l'ancienne tuile
        gameManager.board[position.y, position.x] = Tiles.TileType.Arm_Last;
        position = desiredPosition;
        StartCoroutine(WalkAnimation(desiredPosition,true));
        RotateHand();


    }
}
