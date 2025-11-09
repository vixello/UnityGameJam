using System.Collections;
using UnityEngine;

namespace Core
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;    // For background music
        [SerializeField] private AudioSource sfxSource;      // For sound effects

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 1f;    // seconds

        [Header("Music Clips")]
        [SerializeField] private AudioClip gameplayMusicClip;
        [SerializeField] private AudioClip menuMusicClip;

        private Coroutine _musicFadeCoroutine;

        private void OnEnable()
        {
            EventBus.OnChangeGameState += OnGameStateChanged;
            EventBus.OnSprintChanged += OnSprintChanged;
            EventBus.OnCollectGhosts += OnCollectGhosts;
        }

        private void OnDisable()
        {
            EventBus.OnChangeGameState -= OnGameStateChanged;
            EventBus.OnSprintChanged -= OnSprintChanged;
            EventBus.OnCollectGhosts -= OnCollectGhosts;
        }

        #region Music Control

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource.clip == clip) return;

            if (_musicFadeCoroutine != null)
                StopCoroutine(_musicFadeCoroutine);

            _musicFadeCoroutine = StartCoroutine(FadeMusic(clip));
        }

        public void StopMusic()
        {
            if (_musicFadeCoroutine != null)
                StopCoroutine(_musicFadeCoroutine);

            _musicFadeCoroutine = StartCoroutine(FadeOutMusic());
        }

        private IEnumerator FadeMusic(AudioClip newClip)
        {
            // Fade out current
            float startVolume = musicSource.volume;
            float time = 0f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
                yield return null;
            }

            musicSource.Stop();
            musicSource.clip = newClip;
            musicSource.Play();

            // Fade in new
            time = 0f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(0f, 1f, time / fadeDuration);
                yield return null;
            }

            musicSource.volume = 1f;
        }

        private IEnumerator FadeOutMusic()
        {
            float startVolume = musicSource.volume;
            float time = 0f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
                yield return null;
            }
            musicSource.Stop();
            musicSource.volume = 1f;
        }

        #endregion

        #region SFX Control

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            sfxSource.PlayOneShot(clip, volume);
        }

        #endregion

        #region EventBus Handlers

        private void OnGameStateChanged(GameState gameState)
        {
            // Example: Play different music for InGame vs Menu
            if (gameState == GameState.InGame)
            {
                // Play gameplay music
                PlayMusic(gameplayMusicClip);
            }
            else
            {
                // Play menu music
                PlayMusic(menuMusicClip);
            }
        }

        private void OnSprintChanged(bool isSprinting)
        {
            // Optional: trigger sprint SFX
            // PlaySFX(sprintStartClip);
        }

        private void OnCollectGhosts(int count)
        {
            // Play a ghost collected SFX
            // PlaySFX(ghostCollectedClip);
        }

        #endregion
    }
}
