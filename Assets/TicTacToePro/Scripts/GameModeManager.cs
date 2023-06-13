using System;
using UnityEngine;

namespace TicTacToePro
{
    public class GameModeManager : MonoBehaviour
    {
        public float AIFirstWait;
        public Vector2 AIDelay;

        public float OnlineFirstWait;
        public Vector2 OnlineDelay;

        public BasicGameMode CurrentGameMode;
        public DifficultyLevel CurrentAIDifficultyLevel;

        public GameSettings _3x3Settings;
        public GameSettings _6x6Settings;
        public GameSettings _9x9Settings;
        public GameSettings _11x11Settings;

        public GameSettings Settings(BasicGameMode mode)
        {
            switch (mode)
            {
                case BasicGameMode._3x3:
                    return _3x3Settings;
                case BasicGameMode._6x6:
                    return _6x6Settings;
                case BasicGameMode._9x9:
                    return _9x9Settings;
                case BasicGameMode._11x11:
                    return _11x11Settings;
                default:
                    return _3x3Settings;
            }
        }

        public AIDifficultySettings EasyDifficulty;
        public AIDifficultySettings MediumDifficulty;
        public AIDifficultySettings HardDifficulty;

        public AIDifficultySettings AISettings(DifficultyLevel level)
        {
            switch (level)
            {
                case DifficultyLevel.Easy:
                    return EasyDifficulty;
                case DifficultyLevel.Medium:
                    return MediumDifficulty;
                case DifficultyLevel.Hard:
                    return HardDifficulty;
                default:
                    return EasyDifficulty;
            }
        }

        public AIDifficultySettings CurrentAIDifficulty
        {
            get { return AISettings(CurrentAIDifficultyLevel); }
        }

        public GameSettings CurrentGameSettings
        {
            get { return Settings(CurrentGameMode); }
        }

        public void ChangeDifficulty(DifficultyLevel difficulty)
        {
            CurrentAIDifficultyLevel = difficulty;

            LocalUser.shared.SavedData.TTTData.AIDifficulty = difficulty;
            LocalUser.shared.SavedData.TTTData.Last20GamesPlayerResults = new();
            LocalUser.shared.SavedData.TTTData.UserAIDifficultyMod = new();

            LocalUser.shared.Save();
        }

        void Start()
        {
            var difficulty = LocalUser.shared.SavedData.TTTData.AIDifficulty;
            CurrentAIDifficultyLevel = difficulty;
        }

        public void RecalculateUserAIDiffMod(int playerWinResult)
        {
            var lastGames = LocalUser.shared.SavedData.TTTData.Last20GamesPlayerResults;
            if (lastGames == null)
                lastGames = new();

            lastGames.Add(playerWinResult);

            var maxGames = 20;
            for (int i = lastGames.Count - 1; i >= 0; i--)
            {
                if (i < maxGames)
                    break;
                lastGames.RemoveAt(i);
            }

            float wins = 0;
            float losses = 0;
            float draws = 0;
            for (int i = 0; i < lastGames.Count; i++)
            {
                var result = lastGames[i];
                if (result == 1)
                    wins += 1;
                if (result == 0)
                    draws += 1;
                if (result == -1)
                    losses += 1;
            }

            var userDiffMod = LocalUser.shared.SavedData.TTTData.UserAIDifficultyMod;

            wins = Mathf.Clamp(wins, 1, 20);
            losses = Mathf.Clamp(losses, 1, 20);
            draws = Mathf.Clamp(draws, 1, 20);
            var blunderMod = (losses + draws / 2) / (wins);
            var scoreDiffMod = (losses + draws / 2 + 8) / (wins + 8);

            userDiffMod.ChangeAllBlunders(blunderMod);
            userDiffMod.ChangeAllScoreDiffs(scoreDiffMod);

            LocalUser.shared.SavedData.TTTData.Last20GamesPlayerResults = lastGames;
            LocalUser.shared.SavedData.TTTData.UserAIDifficultyMod = userDiffMod;
            LocalUser.shared.Save();
        }
    }

    [Serializable]
    public class AIDifficultySettings
    {
        public float WinAttackBlunder;
        public float WinDefendBlunder;
        public float DangerAttackBlunder;
        public float DangerDefendBlunder;
        public float ExpansionAttackBlunder;
        public float ExpansionDefendBlunder;
        public float BasicMoveBlunder;

        public float WinAttackMaxScoreDiff;
        public float WinDefendMaxScoreDiff;
        public float DangerAttackMaxScoreDiff;
        public float DangerDefendMaxScoreDiff;
        public float ExpansionAttackMaxScoreDiff;
        public float ExpansionDefendMaxScoreDiff;
        public float BasicMoveMaxScoreDiff;
    }

    [Serializable]
    public class UserAIDifficultySettings
    {
        public float WinAttackBlunderMod = 1f;
        public float WinDefendBlunderMod = 1f;
        public float DangerAttackBlunderMod = 1f;
        public float DangerDefendBlunderMod = 1f;
        public float ExpansionAttackBlunderMod = 1f;
        public float ExpansionDefendBlunderMod = 1f;
        public float BasicMoveBlunderMod = 1f;

        public float WinAttackMaxScoreDiffMod = 1f;
        public float WinDefendMaxScoreDiffMod = 1f;
        public float DangerAttackMaxScoreDiffMod = 1f;
        public float DangerDefendMaxScoreDiffMod = 1f;
        public float ExpansionAttackMaxScoreDiffMod = 1f;
        public float ExpansionDefendMaxScoreDiffMod = 1f;
        public float BasicMoveMaxScoreDiffMod = 1f;

        public void ChangeAllBlunders(float mod)
        {
            WinAttackBlunderMod = mod;
            WinDefendBlunderMod = mod;
            DangerAttackBlunderMod = mod;
            DangerDefendBlunderMod = mod;
            ExpansionAttackBlunderMod = mod;
            ExpansionDefendBlunderMod = mod;
            BasicMoveBlunderMod = mod;
        }

        public void ChangeAllScoreDiffs(float mod)
        {
            WinAttackMaxScoreDiffMod = mod;
            WinDefendMaxScoreDiffMod = mod;
            DangerAttackMaxScoreDiffMod = mod;
            DangerDefendMaxScoreDiffMod = mod;
            ExpansionAttackMaxScoreDiffMod = mod;
            ExpansionDefendMaxScoreDiffMod = mod;
            BasicMoveMaxScoreDiffMod = mod;
        }
    }

    [Serializable]
    public enum BasicGameMode
    {
        _3x3,
        _6x6,
        _9x9,
        _11x11
    }

    [Serializable]
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }

    [Serializable]
    public class GameSettings
    {
        public int BoardWidth = 3;
        public int WinningLength = 3;
    }
}