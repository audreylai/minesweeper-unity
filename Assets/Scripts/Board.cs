using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [SerializeField] Tilemap boardMap;
    [SerializeField] Tilemap maskMap;
    [SerializeField] List<Tile> tiles;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int bombCount;
    [SerializeField] GameManager gameManager;
    bool gameEnd = false;
    bool firstClick = true;

    enum TileType
    {
        Bomb = 2,
        Flag = 1,
        Exploded = 3,
        Unknown = 0
    }

    TileType[,] board;

    // Sound Effects
    public AudioClip clickSound;
    public AudioClip flagSound;

    public void setup(int w, int h, int bombs)
    {
        width = w;
        height = h;
        bombCount = bombs;
        board = new TileType[width, height];
        generateBoardMask();
        Camera.main.transform.position = new Vector3(width / 2, height / 2, -10);
    }

    void Start()
    {
        board = new TileType[width, height];
        generateBoardMask();
        Camera.main.transform.position = new Vector3(width / 2, height / 2, -10);
    }

    void Update()
    {
        if (checkWin() && !gameEnd && !firstClick)
        {
            gameEnd = true;
            gameManager.endGame(true);
        }
    }

    void placeBombs(int x, int y)
    {
        for (int i = 0; i < bombCount; i++)
        {
            int randX = Random.Range(0, width - 1);
            int randY = Random.Range(0, height - 1);
            while (board[randX, randY] == TileType.Bomb || randX == x || randY == y)
            {
                randX = Random.Range(0, width - 1);
                randY = Random.Range(0, height - 1);
            }
            board[randX, randY] = TileType.Bomb;
            boardMap.SetTile(new Vector3Int(randX, randY), tiles[(int)TileType.Bomb]);
        }
    }

    void calculateBombCount(int x, int y)
    {
        int[,] adj = { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 } };
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            int adjX = x + adj[i, 0];
            int adjY = y + adj[i, 1];
            if (adjX >= 0 && adjX < width && adjY >= 0 && adjY < height && board[adjX, adjY] == TileType.Bomb)
            {
                count++;
            }
        }
        boardMap.SetTile(new Vector3Int(x, y), tiles[(int)count + 4]);
    }

    void generateBoardBomb()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y] != TileType.Bomb)
                {
                    calculateBombCount(x, y);
                }
            }
        }
    }

    void generateBoardMask()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maskMap.SetTile(new Vector3Int(x, y), tiles[(int)TileType.Unknown]);
                boardMap.SetTile(new Vector3Int(x, y), tiles[4]);
            }
        }
    }

    private void lose()
    {
        gameEnd = true;
        gameManager.endGame(false);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y] == TileType.Bomb)
                {
                    board[x, y] = TileType.Exploded;
                    boardMap.SetTile(new Vector3Int(x, y), tiles[(int)TileType.Exploded]);
                }
                maskMap.SetTile(new Vector3Int(x, y), null);
            }
        }
    }

    private bool checkWin()
    {
        Debug.Log(calculateOpen());
        Debug.Log(maskMap.ContainsTile(tiles[(int)TileType.Unknown]));
        if (!maskMap.ContainsTile(tiles[(int)TileType.Unknown]) || (width*height)-calculateOpen() == bombCount)
        {
            return true;
        }
        return false;
    }

    private void toggleFlag(int x, int y)
    {
        SoundManager.Instance.Play(flagSound);
        if (maskMap.GetTile<Tile>(new Vector3Int(x, y)) == tiles[(int)TileType.Flag])
        {
            maskMap.SetTile(new Vector3Int(x, y), tiles[(int)TileType.Unknown]);
        }
        else if (maskMap.GetTile<Tile>(new Vector3Int(x, y)) == tiles[(int)TileType.Unknown])
        {
            maskMap.SetTile(new Vector3Int(x, y), tiles[(int)TileType.Flag]);
        }
    }

    private void showTile(int x, int y)
    {
        maskMap.SetTile(new Vector3Int(x, y), null);
        if (firstClick)
        {
            placeBombs(x, y);
            generateBoardBomb();
            firstClick = false;
        }
        else if (board[x, y] == TileType.Bomb)
        {
            lose();
        }
        if (boardMap.GetTile<Tile>(new Vector3Int(x, y)) == tiles[4])
        {
            floodTiles(x, y);
        }
    }

    private void chordTile(int x, int y)
    {
        int flagN = tiles.IndexOf(boardMap.GetTile<Tile>(new Vector3Int(x, y))) - 4;
        int flags = 0;
        int[,] adj = { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 } };
        for (int i = 0; i < 8; i++)
        {
            int adjX = x + adj[i, 0];
            int adjY = y + adj[i, 1];
            if (adjX >= 0 && adjX < width && adjY >= 0 && adjY < height && maskMap.GetTile<Tile>(new Vector3Int(adjX, adjY)) == tiles[(int)TileType.Flag])
            {
                flags++;
            }
        }

        if (flagN == flags)
        {
            for (int i = 0; i < 8; i++)
            {
                int adjX = x + adj[i, 0];
                int adjY = y + adj[i, 1];
                if (adjX >= 0 && adjX < width && adjY >= 0 && adjY < height && maskMap.GetTile<Tile>(new Vector3Int(adjX, adjY)) != tiles[(int)TileType.Flag])
                {
                    showTile(adjX, adjY);
                }
            }
        }

    }

    private void floodTiles(int x, int y)
    {
        int[,] adj = { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 } };
        for (int i = 0; i < 8; i++)
        {
            int adjX = x + adj[i, 0];
            int adjY = y + adj[i, 1];
            if (adjX >= 0 && adjX < width && adjY >= 0 && adjY < height && board[adjX, adjY] != TileType.Bomb && maskMap.GetTile<Tile>(new Vector3Int(adjX, adjY)) == tiles[(int)TileType.Unknown])
            {
                showTile(adjX, adjY);
            }
        }
    }

    private void OnMouseOver()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int mouseX = (int)mousePos.x;
        int mouseY = (int)mousePos.y;
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                SoundManager.Instance.Play(clickSound);
                showTile(mouseX, mouseY);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                toggleFlag(mouseX, mouseY);
            }
            else if (Input.GetMouseButtonDown(2))
            {
                SoundManager.Instance.Play(clickSound);
                chordTile(mouseX, mouseY);
            }
        }

    }

    public int calculateOpen()
    {
        int sum = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maskMap.GetTile<Tile>(new Vector3Int(x, y)) == tiles[(int)TileType.Unknown])
                {
                    sum++;
                }
            }
        }
        return sum;
    }

    public void resetBoard()
    {
        firstClick = true;
        gameEnd = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maskMap.SetTile(new Vector3Int(x, y), null);
                boardMap.SetTile(new Vector3Int(x, y), null);
            }
        }
    }

    public int flagCount()
    {
        int flags = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maskMap.GetTile<Tile>(new Vector3Int(x, y)) == tiles[(int)TileType.Flag])
                {
                    flags++;
                }
            }
        }
        return bombCount - flags;
    }

}
