using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace TicTacToePro.Game2048
{
    public class Game2048 : MonoBehaviour
    {
        public GameScreen Screen;
        public JuicyButton GoBackButton;
        public JuicyButton OptionsButton;
        public float MoveSpeed;
        public Transform[] CubeLimits;
        public Cube2048 CurrentCube;
        public GameObject CubePrefab;
        public Transform CubeHolder;
        public float ReloadTime = 0.25f;
        public float ShootDelay = 0.25f;
        public Settings2048 Settings;
        public List<Cube2048> ActiveCubes;
        public TextMeshProUGUI ScoreText;
        public int CurrentScore;
        public SFXName ShootSound;
        public SFXName MatchSound;

        float _currentCubePos = 0.5f;
        float _currentPitch = 0.75f;
        float _currentPitchCooldown = 0f;

        bool _isPlaying;

        public bool IsPlaying => _isPlaying;

        AudioSource _prevAudio;

        void OpenOptions()
        {
            SettingsScreen.shared.Show();
            SettingsScreen.shared.ShowHomeButton(GoBackSettings);
            SettingsScreen.shared.ShowRestartButton(RestartSettings);
        }

        void RestartGame()
        {
            for (int i = ActiveCubes.Count - 1; i >= 0; i--)
            {
                var cube = ActiveCubes[i];
                cube.KillThis();
            }

            CurrentScore = 0;
            UpdateScore();
        }

        public void AddScore(int score)
        {
            CurrentScore += score;

            LocalUser.shared.SavedData.Game2048Data.MatchedBlocks += 1;
            if (CurrentScore > LocalUser.shared.SavedData.Game2048Data.MaxScore)
                LocalUser.shared.SavedData.Game2048Data.MaxScore = CurrentScore;

            LocalUser.shared.Save();

            if (LocalUser.shared.SavedData.Game2048Data.MatchedBlocks % 15 == 0)
                InterstitialDefault.shared.OpenInterstitial();

            UpdateScore();
        }

        void UpdateScore()
        {
            ScoreText.text = "Score: " + CurrentScore.ToString();
        }

        void GoBackSettings()
        {
            SettingsScreen.shared.Hide();
            GoBackForce();
        }

        void RestartSettings()
        {
            SettingsScreen.shared.Hide();
            RestartGame();
        }

        void GoBack()
        {
            GoBackForce();
        }

        public void GoBackForce()
        {
            _isPlaying = false;
            ToggleKinematicCubes(true);
            StopAllCoroutines();
            ScreenManager.shared.SelectScreen(ScreenType.GameSelector);
        }

        public void ToggleKinematicCubes(bool toggle)
        {
            foreach (var cube in ActiveCubes)
            {
                cube.SetKinematic(toggle);
            }
        }

        void Awake()
        {
            Screen.OnShow.AddListener(OnShow);
            OptionsButton.AddListener(OpenOptions);
            GoBackButton.AddListener(GoBack);
        }

        public void PlayMatchSound()
        {
            if (SoundManager.shared != null)
            {
                if (_prevAudio != null)
                    _prevAudio.Stop();

                SoundManager.shared.PlaySFX(MatchSound, _currentPitch * Random.Range(0.9f, 1.1f));
                _currentPitch += 0.075f;
                _currentPitch = Mathf.Clamp(_currentPitch, 0.5f, 1.3f);
                _currentPitchCooldown = 2f;
            }
        }

        public void PlayShootSound()
        {
            if (SoundManager.shared != null)
            {
                SoundManager.shared.PlaySFX(ShootSound, Random.Range(0.9f, 1.1f), 0.8f);
            }
        }

        void Update()
        {
            if (_currentPitchCooldown <= 0f)
                _currentPitch = 0.75f;
            else
                _currentPitchCooldown -= Time.deltaTime;
        }

        public void MoveCube(float deltaX)
        {
            if (_isPlaying == false)
                return;

            var speed = deltaX * MoveSpeed / 10000f;
            _currentCubePos += speed;
            UpdateCubePos();
        }

        public void Shoot()
        {
            if (CurrentCube == null || _isPlaying == false)
                return;

            PlayShootSound();
            ActiveCubes.Add(CurrentCube);
            CurrentCube.Shoot();
            CurrentCube = null;
            SpawnNewCube(ReloadTime);
        }

        void SpawnNewCube(float time)
        {
            StartCoroutine(SpawnCubeIn(time));
        }

        IEnumerator SpawnCubeIn(float time)
        {
            yield return new WaitForSeconds(time);
            _currentCubePos = Mathf.Clamp(_currentCubePos, 0f, 1f);
            var newPos = Vector3.Lerp(CubeLimits[0].position, CubeLimits[1].position, _currentCubePos);
            var go = Instantiate(CubePrefab, newPos, Quaternion.identity, CubeHolder);
            CurrentCube = go.GetComponent<Cube2048>();

            var value = Settings.StartValues.GetRandom(2);
            CurrentCube.UpdateValue(value);
            CurrentCube.Init(this);

            yield return new WaitForSeconds(ShootDelay);
        }


        void OnShow()
        {
            foreach (var cube in ActiveCubes)
            {
                cube.Init(this);
            }

            if (CurrentCube == null)
                SpawnNewCube(0f);
            _isPlaying = true;
            ToggleKinematicCubes(false);
        }

        void UpdateCubePos()
        {
            if (CurrentCube == null)
                return;

            _currentCubePos = Mathf.Clamp(_currentCubePos, 0f, 1f);
            var newPos = Vector3.Lerp(CubeLimits[0].position, CubeLimits[1].position, _currentCubePos);
            CurrentCube.transform.position = newPos;
        }

        [Button(nameof(ShowGame))] public bool showGame;

        public void ShowGame()
        {
            Screen.SelectScreen();
        }
    }
}