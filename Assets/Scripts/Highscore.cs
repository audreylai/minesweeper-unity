using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Highscore", order = 1)]
public class Highscore : ScriptableObject
{
    public float easy;
    public float medium;
    public float hard;

    public float getScore(GameManager.LevelType difficulty)
    {
        switch ((GameManager.LevelType)difficulty)
        {
            case GameManager.LevelType.Easy:
                return easy;
            case GameManager.LevelType.Medium:
                return medium;
            case GameManager.LevelType.Hard:
                return hard;
        }
        return 0;
    }

    public void setScore(GameManager.LevelType difficulty, float score)
    {
        switch ((GameManager.LevelType)difficulty)
        {
            case GameManager.LevelType.Easy:
                easy = score;
                break;
            case GameManager.LevelType.Medium:
                medium = score;
                break;
            case GameManager.LevelType.Hard:
                hard = score;
                break;
        }
    }
}
