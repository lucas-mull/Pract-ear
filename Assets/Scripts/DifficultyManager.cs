using UnityEngine;

class DifficultyManager
{
    public const string EASY = "easy";
    public const string MEDIUM = "medium";
    public const string HARD = "hard";
    public const string EXTREME = "extreme";

    public const string SIMON = "simon";
    public const string BLINDTEST = "blindtest";
    public const string TAMTAM = "tamtam";
    public const string TEMPO = "tempo";

    public const int ID_SIMON = 1;
    public const int ID_BLINDTEST = 4;
    public const int ID_TAMTAM = 3;
    public const int ID_TEMPO = 2;

    public static string PICKED_DIFFICULTY = EASY;

    static string[] _difficulties = new string[] { EASY, MEDIUM, HARD, EXTREME };
    string _currentGame;

    public static bool isDifficultyUnlocked(string difficulty, string game)
    {
        if (isDifficultyValid(difficulty) && isGameValid(game))
        {
            int res = PlayerPrefs.GetInt(game + "_" + difficulty, 0);
            return res > 0;
        }

        return false;
    }

    public static bool isDifficultyUnlocked(string difficulty, int game)
    {
        return isDifficultyUnlocked(difficulty, GetGameName(game));
    }

    public static void IncrementDifficulty()
    {
        switch (PICKED_DIFFICULTY)
        {
            case EASY:
                PICKED_DIFFICULTY = MEDIUM;
                break;
            case MEDIUM:
                PICKED_DIFFICULTY = HARD;
                break;
            case HARD:
                PICKED_DIFFICULTY = EXTREME;
                break;
            case EXTREME:
                break;
            default:
                break;
        }
    }

    public static void UnlockDifficulty(string difficulty, string game)
    {
        if (isDifficultyValid(difficulty) && isGameValid(game))
        {
            PlayerPrefs.SetInt(game + "_" + difficulty, 1);
        }
    }

    public static void UnlockNextDifficulty(string game)
    {
        foreach(string difficulty in _difficulties)
        {
            if (isDifficultyUnlocked(difficulty, game))
                continue;

            UnlockDifficulty(difficulty, game);
            break;
        }
    }

    public static void UnlockNextDifficulty(int game)
    {
        UnlockNextDifficulty(GetGameName(game));
    }

    public static string GetGameName(int gameId)
    {
        switch(gameId)
        {
            case ID_SIMON:
                return SIMON;
            case ID_BLINDTEST:
                return BLINDTEST;
            case ID_TAMTAM:
                return TAMTAM;
            case ID_TEMPO:
                return TEMPO;
            default:
                return "";
        }
    }

    private static bool isGameValid(string game)
    {
        return game == SIMON || game == BLINDTEST || game == TAMTAM || game == TEMPO;
    }

    public static bool isDifficultyValid(string difficulty)
    {
        return difficulty == EASY || difficulty == MEDIUM || difficulty == HARD || difficulty == EXTREME;
    }

}
