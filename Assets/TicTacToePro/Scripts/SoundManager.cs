using System.Collections;
using System.Collections.Generic;
using TicTacToePro.Pooling;
using UnityEngine;
using UnityEngine.Audio;

namespace TicTacToePro
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager shared;

        public float FadeoutTimeMusic = 1f;
        public float FadeinTimeMusic = 1f;

        public AudioMixer SFXMixer;
        public AudioMixer MusicMixer;

        public PooledObject SFXPlayer;
        public PooledObject MusicPlayer;

        public ObjectPooler SFXPooler;
        public ObjectPooler MusicPooler;

        public AudioClip SFXSounds(SFXName sfx)
        {
            switch (sfx)
            {
                case SFXName.Bubble:
                    return BubbleSound;
                case SFXName.Button:
                    return ButtonSound;
                case SFXName.Draw:
                    return DrawSound;
                case SFXName.Lose:
                    return LoseSound;
                case SFXName.Shoot:
                    return ShootSound;
                case SFXName.Upgrade:
                    return UpgradeSound;
                case SFXName.Win:
                    return WinSound;
                default:
                    return null;
            }
        }

        public List<AudioClip> MusicList;

        public AudioClip BubbleSound;
        public AudioClip ButtonSound;
        public AudioClip DrawSound;
        public AudioClip LoseSound;
        public AudioClip ShootSound;
        public AudioClip UpgradeSound;
        public AudioClip WinSound;

        AudioSource _currentMusic;

        void OnValidate()
        {
            shared = this;
        }

        void Awake()
        {
            shared = this;
        }

        void Start()
        {
            shared = this;


            var sfx = LocalUser.shared.SavedData.SettingsData.SFXEnabled;
            SetSFXVolume(sfx);

            var music = LocalUser.shared.SavedData.SettingsData.MusicEnabled;
            SetMusicVolume(music);
        }

        void SetSFXVolume(bool enabled)
        {
            var volume = enabled ? 0f : -80f;
            SFXMixer.SetFloat("volume", volume);
        }

        void SetMusicVolume(bool enabled)
        {
            var volume = enabled ? 0f : -80f;
            MusicMixer.SetFloat("volume", volume);
        }

        public bool ToggleSFXVolume(bool toggle)
        {
            var sfx = toggle;
            SetSFXVolume(sfx);

            LocalUser.shared.SavedData.SettingsData.SFXEnabled = sfx;
            LocalUser.shared.Save();

            return sfx;
        }

        public bool ToggleMusicVolume(bool toggle)
        {
            var music = toggle;
            SetMusicVolume(music);


            LocalUser.shared.SavedData.SettingsData.MusicEnabled = music;
            LocalUser.shared.Save();

            return music;
        }

        public void PlayMusic(ScreenType screenType)
        {
            if (!Application.isPlaying)
                return;

            if (_currentMusic != null)
            {
                StartCoroutine(FadeoutAudio(FadeoutTimeMusic, _currentMusic));
                _currentMusic = null;
            }

            PlayNewMusic();
        }

        private void PlayNewMusic()
        {
            var clip = MusicList.GetRandom(null);
            if (clip == null)
                return;

            var musicPlayer = MusicPooler.GetPooledObject(MusicPlayer);

            musicPlayer.SetActive(true);

            var source = musicPlayer.GetComponent<AudioSource>();
            _currentMusic = source;
            source.clip = clip;
            source.loop = false;
            source.Play();
            StartCoroutine(FadeinAudio(FadeinTimeMusic, source));
        }

        void Update()
        {
            if (_currentMusic == null || _currentMusic.isPlaying)
                return;

            _currentMusic.gameObject.SetActive(false);

            PlayNewMusic();
        }

        IEnumerator FadeoutAudio(float fadeoutTime, AudioSource source)
        {
            var currentTime = 0f;
            while (currentTime < fadeoutTime)
            {
                var percent = 1 - currentTime / fadeoutTime;
                source.volume = percent;

                currentTime += Time.deltaTime;
                yield return null;
            }

            source.Stop();
            source.gameObject.SetActive(false);
        }

        IEnumerator FadeinAudio(float fadeinTime, AudioSource source)
        {
            var currentTime = 0f;
            while (currentTime < fadeinTime)
            {
                var percent = currentTime / fadeinTime;
                source.volume = percent;

                currentTime += Time.deltaTime;
                yield return null;
            }

            source.volume = 1;
        }

        public AudioSource PlaySFX(SFXName soundName, float pitch = 1f, float volume = 1f)
        {
            var clip = SFXSounds(soundName);

            if (clip == null)
                return null;

            var sfxPlayer = SFXPooler.GetPooledObject(SFXPlayer);

            sfxPlayer.SetActive(true);

            var source = sfxPlayer.GetComponent<AudioSource>();
            source.clip = clip;
            source.pitch = pitch;
            source.volume = volume;
            source.Play();
            StartCoroutine(StopSoundAfterTime(clip.length, source));
            return source;
        }

        IEnumerator StopSoundAfterTime(float time, AudioSource source)
        {
            yield return new WaitForSeconds(time + 0.1f);

            source.Stop();
            source.gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public enum SFXName
    {
        Bubble,
        Button,
        Draw,
        Lose,
        Shoot,
        Upgrade,
        Win,
    }
}