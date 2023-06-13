using TMPro;
using UnityEngine;

namespace TicTacToePro
{
    public class TTTPowerups : MonoBehaviour
    {
        public TTTGameMode GameMode;

        public JuicyButton UndoButton;
        public TextMeshProUGUI UndoAmount;
        public GameObject UndoRefill;

        public JuicyButton HintButton;
        public TextMeshProUGUI HintAmount;
        public GameObject HintRefill;


        void Awake()
        {
            UndoButton.AddListener(Undo);
            HintButton.AddListener(Hint);
        }

        void Start()
        {
            UpdateUndosAndHints();
        }

        void UpdateUndosAndHints()
        {
            var undos = LocalUser.shared.SavedData.TTTData.UndoAmount;

            bool hasUndos = undos >= 1;
            UndoAmount.text = undos.ToString();
            UndoAmount.gameObject.SetActive(hasUndos);
            UndoRefill.SetActive(!hasUndos);


            var hints = LocalUser.shared.SavedData.TTTData.HintsAmount;

            bool hasHints = hints >= 1;
            HintAmount.text = hints.ToString();
            HintAmount.gameObject.SetActive(hasHints);
            HintRefill.SetActive(!hasHints);
        }

        void Update()
        {
            UndoButton.Interactable = IsUndoAvailable;
            HintButton.Interactable = IsHintAvailable;
        }

        void Undo()
        {
            var undos = LocalUser.shared.SavedData.TTTData.UndoAmount;
            if (undos <= 0)
            {
                RefillUndos();
                return;
            }

            if (!IsUndoAvailable)
                return;


            undos -= 1;
            DoUndo();

            LocalUser.shared.SavedData.TTTData.UndoAmount = undos;
            LocalUser.shared.Save();

            UpdateUndosAndHints();
        }

        bool IsUndoAvailable => GameMode.CanReceiveInput && !GameMode.Board.IsFirstMove;

        void Hint()
        {
            var hints = LocalUser.shared.SavedData.TTTData.HintsAmount;

            if (hints <= 0)
            {
                RefillHints();
                return;
            }

            if (!IsHintAvailable)
                return;


            DoHint();
            hints -= 1;

            LocalUser.shared.SavedData.TTTData.HintsAmount = hints;
            LocalUser.shared.Save();

            UpdateUndosAndHints();
        }

        bool IsHintAvailable => GameMode.CanReceiveInput;

        void DoUndo()
        {
            GameMode.Undo();
        }

        void DoHint()
        {
            GameMode.Hint();
        }

        void RefillUndos()
        {
            RewardedDefault.shared.OnAdReceivedReward.AddListener(RewardUndos);
            RewardedDefault.shared.ShowRewarded();
        }

        void RewardUndos()
        {
            RewardedDefault.shared.OnAdReceivedReward.RemoveListener(RewardUndos);

            var undos = LocalUser.shared.SavedData.TTTData.UndoAmount;

            undos += 5;

            LocalUser.shared.SavedData.TTTData.UndoAmount = undos;
            LocalUser.shared.Save();

            UpdateUndosAndHints();
        }

        void RefillHints()
        {
            RewardedDefault.shared.OnAdReceivedReward.AddListener(RewardHints);
            RewardedDefault.shared.ShowRewarded();
        }

        void RewardHints()
        {
            RewardedDefault.shared.OnAdReceivedReward.RemoveListener(RewardHints);

            var hints = LocalUser.shared.SavedData.TTTData.HintsAmount;

            hints += 5;

            LocalUser.shared.SavedData.TTTData.HintsAmount = hints;
            LocalUser.shared.Save();

            UpdateUndosAndHints();
        }
    }
}