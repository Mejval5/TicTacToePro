using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TicTacToePro
{
    public class ChangeUserIDScreen : GameScreen
    {
        public JuicyButton ExitButton;
        public JuicyButton SaveButton;

        public TMP_InputField InputField;

        public string WrongLengthTerm;
        public TextMeshProUGUI ErrorText;

        void Start()
        {
            ErrorText.gameObject.SetActive(false);
            ExitButton.AddListener(() => Hide());
            SaveButton.AddListener(Save);
        }

        public override void Show()
        {
            ErrorText.gameObject.SetActive(false);
            InputField.text = LocalUser.shared.SavedData.SettingsData.UserID;
            base.Show();
        }

        void Save()
        {
            var name = InputField.text;
            var isValid = ValidateName(name);

            if (isValid)
            {
                LocalUser.shared.SavedData.SettingsData.UserID = name;
                LocalUser.shared.Save();
                Hide();
            }
            else
            {
                ErrorText.gameObject.SetActive(true);
                StartCoroutine(ScaleAnimation(ErrorText.transform, 1.1f, 0.2f, 4));
                ErrorText.text = WrongLengthTerm.ToString();
            }

        }

        IEnumerator ScaleAnimation(Transform trans, float targetScaleFloat, float time, int loops)
        {
            // do the animation with inoutquad, loop back to the original scale
            float t = 0f;
            Vector3 startScale = trans.localScale;
            Vector3 targetScale = startScale * targetScaleFloat;
            while (t < time * loops)
            {
                t += Time.unscaledDeltaTime;
                float lerpT = Mathf.Abs(Mathf.Sin(t / (time) * Mathf.PI * 0.5f));
                trans.localScale = Vector3.Lerp(startScale, targetScale, lerpT);
                yield return null;
            }

            trans.localScale = startScale;

            yield return null;
        }

        bool ValidateName(string name)
        {
            return name.Length >= 3 && name.Length <= 20;
        }
    }
}