using System;
using UnityEditor;
using UnityEngine;

public class LevelLoader : EditorWindow
{
    private Maps selectedLevel;

    private int maxmove;
    private string Level;

    private bool StringIsChange;
    private string[] tiles = {"F", "W", "B", "#", "O","S"};
    private char[,] actualLevelTiles = new char[10,10];
    private bool[,] isDropdownOpen = new bool[10, 10];

    private char TilesSelected;
    #region editor fonction
    [MenuItem("Window/LevelLoader")]
    public static void OpenWindow()
    {
        LevelLoader window = GetWindow<LevelLoader>();
        window.titleContent = new GUIContent("Level Loader");

    }

    private void OnEnable() // quand actif
    {
        selectedLevel = FindObjectOfType<GameManager>().LevelToLoad;
        Level = selectedLevel.Level1;
    }

    private void OnGUI() // s'actualise regulierement
    {
        GUILayout.Label("Level Loader",EditorStyles.boldLabel);
        LevelLoading();
        GUILayout.Label("");
        TileSelector();
        GUILayout.Label("");
        AcceptLevelModif();
        GUILayout.Label("Actual Level");
        GetStringInfo();
        LevelGUI();
        
        
    }
    #endregion
    private void LevelLoading()
    {
        selectedLevel = (Maps)EditorGUILayout.ObjectField(selectedLevel, typeof(Maps));
        if (GUILayout.Button("LoadLevel"))
        {
            FindObjectOfType<GameManager>().LoadLevel(selectedLevel);
            Level = selectedLevel.Level1;
        }
    }

    private void GetStringInfo()
    {
        string[] levelLines = Level.Split('\n');
        
        // Boucle sur les lignes (max 10 ou le nombre de lignes disponibles)
        for (int row = 0; row < 10 && row < levelLines.Length; row++)
        {
            // Boucle sur les colonnes (max 10 ou la longueur de la ligne)
            for (int col = 0; col < 10 && col < levelLines[row].Length; col++)
            {
                // Détermine quel caractère est dans la ligne et la colonne actuelles, et le stocke dans actualLevelTiles
                char tileChar = levelLines[row][col];

                switch (tileChar)
                {
                    case 'F':
                        actualLevelTiles[row, col] = 'F';
                        break;
                    case 'B':
                        actualLevelTiles[row, col] = 'B';
                        break;
                    case 'W':
                        actualLevelTiles[row, col] = 'W';
                        break;
                    case 'S':
                        actualLevelTiles[row, col] = 'S';
                        break;
                    case '#':
                        actualLevelTiles[row, col] = '#';
                        break;
                    case 'O':
                        actualLevelTiles[row, col] = 'O';
                        break;
                    case 'A':
                        actualLevelTiles[row, col] = 'A';
                        break;
                    default:
                        // Si un caractère inattendu est rencontré, on peut le marquer comme vide ou le signaler
                        actualLevelTiles[row, col] = ' '; // Par exemple, ' ' pour un espace vide
                        Debug.LogWarning($"Caractère inconnu '{tileChar}' à la position [{row}, {col}]");
                        break;
                }
            }
        }
    }

    private void LevelGUI()
    {

        for (int y = 0; y < 10; y++)
        {
            GUILayout.BeginHorizontal(); // Démarre une nouvelle ligne pour chaque rangée de y

            for (int x = 0; x < 10; x++)
            {
                // Vérifie si actualLevelTiles[x, y] contient une valeur et l'affiche
                string tile = actualLevelTiles[y, x].ToString();
                switch (tile)
                {
                    case "F":
                        GUI.color = Color.green;
                        break;
                    case "W":
                        GUI.color = Color.black;
                        break;
                    case "B":
                        GUI.color = Color.blue;
                        break;
                    case "#":
                        GUI.color = Color.red;
                        break;
                    case "O":
                        GUI.color = Color.yellow;
                        break;
                    case "S":
                        GUI.color = Color.magenta;
                        break;
                    case "A":
                        GUI.color = Color.cyan;
                        break;
                }
                DropBoxLevel(tile,y,x);
            }
            GUILayout.EndHorizontal(); // Terminer la ligne après avoir ajouté les 10 cases pour x
        }

           
        GUI.color = Color.clear;
    }

    void DropBoxLevel(string tile,int y,int x)
    {
        if (GUILayout.Button(tile, GUILayout.Width(30), GUILayout.Height(30)))
        {
            actualLevelTiles[y, x] = TilesSelected; // Change le caractère à chaque clic

            UpdateString();
        }
    }

    void UpdateString()
    {
        Level = "";
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                Level += actualLevelTiles[y, x];
            }
            Level += "\n"; // Retour à la ligne après chaque ligne du tableau
        }
        
    }

    void AcceptLevelModif()
    {
        if(GUILayout.Button("Accept Modification"))
        {
            selectedLevel.Level1 = Level;
        }
    }

    void TileSelector()
    {
        GUILayout.BeginHorizontal();
        GUI.color = Color.green;
        if (GUILayout.Button("F", GUILayout.Width(30), GUILayout.Height(30)))
        {
            TilesSelected = 'F';
        }
        GUI.color = Color.clear;
        GUILayout.FlexibleSpace();
        GUI.color = Color.black;
        if (GUILayout.Button("W", GUILayout.Width(30), GUILayout.Height(30)))
        {
            TilesSelected = 'W';
        }
        GUI.color = Color.clear;
        GUILayout.FlexibleSpace();
        GUI.color = Color.blue;
        if (GUILayout.Button("B", GUILayout.Width(30), GUILayout.Height(30)))
        {
            TilesSelected = 'B';
        }
        GUI.color = Color.clear;
        GUILayout.FlexibleSpace();
        GUI.color = Color.magenta;
        if (GUILayout.Button("S", GUILayout.Width(30), GUILayout.Height(30)))
        {
            TilesSelected = 'S';
        }
        GUI.color = Color.clear;
        GUILayout.FlexibleSpace();
        GUI.color = Color.red;
        if (GUILayout.Button("#", GUILayout.Width(30), GUILayout.Height(30)))
        {
            TilesSelected = '#';
        }
        GUI.color = Color.clear;
        GUILayout.FlexibleSpace();
        GUI.color = Color.yellow;
        if (GUILayout.Button("O", GUILayout.Width(30), GUILayout.Height(30)))
        {
            TilesSelected = 'O';
        }
        GUI.color = Color.clear;
        GUILayout.FlexibleSpace();
        GUI.color = Color.cyan;
        if (GUILayout.Button("A", GUILayout.Width(30), GUILayout.Height(30)))
        {
            TilesSelected = 'A';
        }
        GUI.color = Color.white;
        GUILayout.EndHorizontal();
    }




}
