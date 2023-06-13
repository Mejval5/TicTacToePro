using UnityEngine;

namespace TicTacToePro
{
    public class TTTAI
    {
        bool _crosses;
        GameBoard _board;
        AIDifficultySettings _aiDiff;

        UserAIDifficultySettings _userDiff
        {
            get
            {
                if (_overrideUserDiffSettings != null && _overrideUserDiff)
                    return _overrideUserDiffSettings;

                return LocalUser.shared.SavedData.TTTData.UserAIDifficultyMod;
            }
        }

        UserAIDifficultySettings _overrideUserDiffSettings = null;
        bool _overrideUserDiff = true;

        public void OverrideUserDiff(UserAIDifficultySettings userDiff)
        {
            _overrideUserDiffSettings = userDiff;
            _overrideUserDiff = true;
        }


        public void SetDifficulty(AIDifficultySettings aiDifficultySettings)
        {
            _aiDiff = aiDifficultySettings;
        }

        public void Init(GameBoard board, bool crosses, AIDifficultySettings aiDifficultySettings)
        {
            _crosses = crosses;
            _board = board;
            _aiDiff = aiDifficultySettings;
        }

        public int PlayFirstMove()
        {
            var width = _board.BoardWidth;
            var xPos = Mathf.RoundToInt(HM.RandomGaussian(0, width - 1));
            var yPos = Mathf.RoundToInt(HM.RandomGaussian(0, width - 1));
            return _board.PlayerPlay(xPos, yPos, _crosses);
        }

        float ClampBlunder(float aiBlunder, float userBlunder)
        {
            var blunder = aiBlunder * userBlunder;
            return Mathf.Clamp(blunder, 1f, 95f);
        }

        float ClampScoreDiffMod(float aiScoreDiffMod, float userScoreDiffMod)
        {
            var scoreDiffMod = aiScoreDiffMod * userScoreDiffMod;
            return Mathf.Clamp(scoreDiffMod, 0f, 4f);
        }

        public int MakeMove()
        {
            // Finish game
            var winAttackBlunder = ClampBlunder(_aiDiff.WinAttackBlunder, _userDiff.WinAttackBlunderMod);
            if (IsNotBlunder(winAttackBlunder))
            {
                var maxScore = ClampScoreDiffMod(_aiDiff.WinAttackMaxScoreDiff, _userDiff.WinAttackMaxScoreDiffMod);
                var winZoneAttackMove = _board.LineBoard.GetWinZoneBestMoves(_crosses, maxScore).GetRandom(-1);
                if (winZoneAttackMove != -1)
                {
                    return _board.PlayerPlay(winZoneAttackMove, _crosses);
                }
            }

            // Cover Enemy Win Moves
            var winDefendBlunder = ClampBlunder(_aiDiff.WinDefendBlunder, _userDiff.WinDefendBlunderMod);
            if (IsNotBlunder(winDefendBlunder))
            {
                var maxScore = ClampScoreDiffMod(_aiDiff.WinDefendMaxScoreDiff, _userDiff.WinDefendMaxScoreDiffMod);
                var winZoneDefMove = _board.LineBoard.GetWinZoneBestMoves(!_crosses, maxScore).GetRandom(-1);
                if (winZoneDefMove != -1)
                {
                    return _board.PlayerPlay(winZoneDefMove, _crosses);
                }
            }

            var dangerAttackBlunder = ClampBlunder(_aiDiff.DangerAttackBlunder, _userDiff.DangerAttackBlunderMod);
            // Make danger zones and make forks
            if (IsNotBlunder(dangerAttackBlunder))
            {
                var maxScore = ClampScoreDiffMod(_aiDiff.DangerAttackMaxScoreDiff, _userDiff.DangerAttackMaxScoreDiffMod);
                var dangerZoneAttackMove = _board.LineBoard.GetDangerZoneBestMoves(_crosses, maxScore).GetRandom(-1);
                if (dangerZoneAttackMove != -1)
                {
                    return _board.PlayerPlay(dangerZoneAttackMove, _crosses);
                }
            }

            // Cover danger zones and cover enemy forks
            var dangerDefendBlunder = ClampBlunder(_aiDiff.DangerDefendBlunder, _userDiff.DangerDefendBlunderMod);
            if (IsNotBlunder(dangerDefendBlunder))
            {
                var maxScore = ClampScoreDiffMod(_aiDiff.DangerDefendMaxScoreDiff, _userDiff.DangerDefendMaxScoreDiffMod);
                var dangerZoneDefMove = _board.LineBoard.GetDangerZoneBestMoves(!_crosses, maxScore).GetRandom(-1);
                if (dangerZoneDefMove != -1)
                {
                    return _board.PlayerPlay(dangerZoneDefMove, _crosses);
                }
            }

            // Try to get best expansion move
            var expansionAttackBlunder = ClampBlunder(_aiDiff.ExpansionAttackBlunder, _userDiff.ExpansionAttackBlunderMod);
            if (IsNotBlunder(expansionAttackBlunder))
            {
                var maxScore = ClampScoreDiffMod(_aiDiff.ExpansionAttackMaxScoreDiff, _userDiff.ExpansionAttackMaxScoreDiffMod);
                var expansionMoveAttack = _board.LineBoard.GetExpansionBestMoves(_crosses, maxScore).GetRandom(-1);
                if (expansionMoveAttack != -1)
                {
                    return _board.PlayerPlay(expansionMoveAttack, _crosses);
                }
            }

            // Try to block best expansion move
            var expansionDefendBlunder = ClampBlunder(_aiDiff.ExpansionDefendBlunder, _userDiff.ExpansionDefendBlunderMod);
            if (IsNotBlunder(expansionDefendBlunder))
            {
                var maxScore = ClampScoreDiffMod(_aiDiff.ExpansionDefendMaxScoreDiff, _userDiff.ExpansionDefendMaxScoreDiffMod);
                var expansionMoveDef = _board.LineBoard.GetExpansionBestMoves(!_crosses, maxScore).GetRandom(-1);
                if (expansionMoveDef != -1)
                {
                    return _board.PlayerPlay(expansionMoveDef, _crosses);
                }
            }

            // Try to get most valued move
            var basicMoveBlunder = ClampBlunder(_aiDiff.BasicMoveBlunder, _userDiff.BasicMoveBlunderMod);
            if (IsNotBlunder(basicMoveBlunder))
            {
                var maxScore = ClampScoreDiffMod(_aiDiff.BasicMoveMaxScoreDiff, _userDiff.BasicMoveMaxScoreDiffMod);
                var basicMove = _board.LineBoard.GetBasicBestMoves(maxScore).GetRandom(-1);
                if (basicMove != -1)
                {
                    return _board.PlayerPlay(basicMove, _crosses);
                }
            }

            Debug.Log("Failed to play anything, fallbacking to random");
            return _board.PlayRandomMove(_crosses);
        }

        bool IsNotBlunder(float chance)
        {
            return UnityEngine.Random.Range(0f, 100f) > chance;
        }
    }
}