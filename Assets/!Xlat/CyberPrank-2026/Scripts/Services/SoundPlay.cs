using UnityEngine;

/// <summary>
/// Минимальный сервис для проигрывания звуков
/// </summary>

namespace Service
{
    public static class Sound
    {
        private static GameObject _root;
        private static AudioSource _source;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_root == null)
            {
                _root = new GameObject("[SoundPlay]");
                Object.DontDestroyOnLoad(_root);
                _source = _root.AddComponent<AudioSource>();
                _source.playOnAwake = false;
            }
        }

        /// <summary>
        /// Проиграть звук
        /// </summary>
        public static void Play(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            Initialize();
            _source.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Проиграть звук по имени (Resources)
        /// </summary>
        public static void Play(string clipPath, float volume = 1f)
        {
            var clip = Resources.Load<AudioClip>(clipPath);
            if (clip != null) Play(clip, volume);
        }
    }
}