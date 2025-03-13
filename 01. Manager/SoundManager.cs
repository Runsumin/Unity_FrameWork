using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace LOBS
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // SoundManager
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Enum
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Enum]
        public enum eSounds
        {
            ALL,
            BGM1,
            BGM2,
            EFFECT1,
            EFFECT2,
            Max,
        }

        public enum eType
        {
            Scale,
            Mute,
            Max,
        }

        public enum eSet
        {
            Off,
            On,
            Max,
        }
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable]
        /// <summary>
        /// The gameobject that the sound manager is attached to
        /// </summary>
        public static GameObject Gameobject { get { return instance.gameObject; } }

        /// <summary>
        /// When set to true, new music audios that have the same audio clip as any other music audios, will be ignored
        /// </summary>
        public static bool IgnoreDuplicateMusic { get; set; }

        /// <summary>
        /// When set to true, new sound audios that have the same audio clip as any other sound audios, will be ignored
        /// </summary>
        public static bool IgnoreDuplicateSounds { get; set; }

        /// <summary>
        /// When set to true, new UI sound audios that have the same audio clip as any other UI sound audios, will be ignored
        /// </summary>
        public static bool IgnoreDuplicateUISounds { get; set; }

        /// <summary>
        /// Global volume
        /// </summary>
        public static float GlobalVolume { get; set; }

        /// <summary>
        /// Global music volume
        /// </summary>
        public static float GlobalMusicVolume { get; set; }

        /// <summary>
        /// Global sounds volume
        /// </summary>
        public static float GlobalSoundsVolume { get; set; }

        /// <summary>
        /// Global UI sounds volume
        /// </summary>
        public static float GlobalUISoundsVolume { get; set; }

        private static Dictionary<int, SoundObject> musicAudio;
        private static Dictionary<int, SoundObject> soundsAudio;
        private static Dictionary<int, SoundObject> UISoundsAudio;
        private static Dictionary<int, SoundObject> audioPool;

        private static bool initialized = false;
        #endregion
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        //public static SoundManager Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = FindObjectOfType<SoundManager>(); // 🚨 이 부분이 문제
        //        }
        //        return instance;
        //    }
        //}

        //static SoundManager()
        //{
        //    Instance.Init();
        //}

        public void Awake()
        {
            instance = this;
            Init();
        }
        /// <summary>
        /// Initialized the sound manager
        /// </summary>
        private void Init()
        {
            if (!initialized)
            {
                musicAudio = new Dictionary<int, SoundObject>();
                soundsAudio = new Dictionary<int, SoundObject>();
                UISoundsAudio = new Dictionary<int, SoundObject>();
                audioPool = new Dictionary<int, SoundObject>();

                GlobalVolume = 1;
                GlobalMusicVolume = 1;
                GlobalSoundsVolume = 1;
                GlobalUISoundsVolume = 1;

                IgnoreDuplicateMusic = false;
                IgnoreDuplicateSounds = false;
                IgnoreDuplicateUISounds = false;

                initialized = true;
                DontDestroyOnLoad(this);
            }
        }

        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Event triggered when a new scene is loaded
        /// </summary>
        /// <param name="scene">The scene that is loaded</param>
        /// <param name="mode">The scene load mode</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Stop and remove all non-persistent audio
            RemoveNonPersistAudio(musicAudio);
            RemoveNonPersistAudio(soundsAudio);
            RemoveNonPersistAudio(UISoundsAudio);
        }

        private void Update()
        {
            UpdateAllAudio(musicAudio);
            UpdateAllAudio(soundsAudio);
            UpdateAllAudio(UISoundsAudio);
        }

        /// <summary>
        /// Retrieves the audio dictionary based on the audioType
        /// </summary>
        /// <param name="audioType">The audio type of the dictionary to return</param>
        /// <returns>An audio dictionary</returns>
        private static Dictionary<int, SoundObject> GetAudioTypeDictionary(SoundType audioType)
        {
            Dictionary<int, SoundObject> audioDict = new Dictionary<int, SoundObject>();
            switch (audioType)
            {
                case SoundType.BGM:
                    audioDict = musicAudio;
                    break;
                case SoundType.Effect:
                    audioDict = soundsAudio;
                    break;
                case SoundType.UIEffect:
                    audioDict = UISoundsAudio;
                    break;
            }

            return audioDict;
        }

        /// <summary>
        /// Retrieves the IgnoreDuplicates setting of audios of a specified audio type
        /// </summary>
        /// <param name="audioType">The audio type that the returned IgnoreDuplicates setting affects</param>
        /// <returns>An IgnoreDuplicates setting (bool)</returns>
        private static bool GetAudioTypeIgnoreDuplicateSetting(SoundType audioType)
        {
            switch (audioType)
            {
                case SoundType.BGM:
                    return IgnoreDuplicateMusic;
                case SoundType.Effect:
                    return IgnoreDuplicateSounds;
                case SoundType.UIEffect:
                    return IgnoreDuplicateUISounds;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Updates the state of all audios of an audio dictionary
        /// </summary>
        /// <param name="audioDict">The audio dictionary to update</param>
        private static void UpdateAllAudio(Dictionary<int, SoundObject> audioDict)
        {
            // Go through all audios and update them
            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                SoundObject audio = audioDict[key];
                audio.Update();

                // Remove it if it is no longer active (playing)
                if (!audio.Setting.isPlaying && !audio.Setting.Paused)
                {
                    Destroy(audio.AudioSource);

                    // Add it to the audio pool in case it needs to be referenced in the future
                    audioPool.Add(key, audio);
                    audio.Setting.Pooled = true;
                    audioDict.Remove(key);
                }
            }
        }

        /// <summary>
        /// Remove all non-persistant audios from an audio dictionary
        /// </summary>
        /// <param name="audioDict">The audio dictionary whose non-persistant audios are getting removed</param>
        private static void RemoveNonPersistAudio(Dictionary<int, SoundObject> audioDict)
        {
            // Go through all audios and remove them if they should not persist through scenes
            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                SoundObject audio = audioDict[key];
                if (!audio.Persist && audio.Setting.Activated)
                {
                    Destroy(audio.AudioSource);
                    audioDict.Remove(key);
                }
            }

            // Go through all audios in the audio pool and remove them if they should not persist through scenes
            keys = new List<int>(audioPool.Keys);
            foreach (int key in keys)
            {
                SoundObject audio = audioPool[key];
                if (!audio.Persist && audio.Setting.Activated)
                {
                    audioPool.Remove(key);
                }
            }
        }

        /// <summary>
        /// Restores and re-adds a pooled audio to its corresponding audio dictionary
        /// </summary>
        /// <param name="audioType">The audio type of the audio to restore</param>
        /// <param name="audioID">The ID of the audio to be restored</param>
        /// <returns>True if the audio is restored, false if the audio was not in the audio pool.</returns>
        public static bool RestoreAudioFromPool(SoundType audioType, int audioID)
        {
            if (audioPool.ContainsKey(audioID))
            {
                Dictionary<int, SoundObject> audioDict = GetAudioTypeDictionary(audioType);
                audioDict.Add(audioID, audioPool[audioID]);
                audioPool.Remove(audioID);

                return true;
            }

            return false;
        }

        #region GetAudio Functions

        /// <summary>
        /// Returns the Audio that has as its id the audioID if one is found, returns null if no such Audio is found
        /// </summary>
        /// <param name="audioID">The id of the Audio to be retrieved</param>
        /// <returns>Audio that has as its id the audioID, null if no such Audio is found</returns>
        public static SoundObject GetAudio(int audioID)
        {
            SoundObject audio;

            audio = GetMusicAudio(audioID);
            if (audio != null)
            {
                return audio;
            }

            audio = GetSoundAudio(audioID);
            if (audio != null)
            {
                return audio;
            }

            audio = GetUISoundAudio(audioID);
            if (audio != null)
            {
                return audio;
            }

            return null;
        }

        /// <summary>
        /// Returns the first occurrence of Audio that plays the given audioClip. Returns null if no such Audio is found
        /// </summary>
        /// <param name="audioClip">The audio clip of the Audio to be retrieved</param>
        /// <returns>First occurrence of Audio that has as plays the audioClip, null if no such Audio is found</returns>
        public static SoundObject GetAudio(AudioClip audioClip)
        {
            SoundObject audio = GetMusicAudio(audioClip);
            if (audio != null)
            {
                return audio;
            }

            audio = GetSoundAudio(audioClip);
            if (audio != null)
            {
                return audio;
            }

            audio = GetUISoundAudio(audioClip);
            if (audio != null)
            {
                return audio;
            }

            return null;
        }

        /// <summary>
        /// Returns the music Audio that has as its id the audioID if one is found, returns null if no such Audio is found
        /// </summary>
        /// <param name="audioID">The id of the music Audio to be returned</param>
        /// <returns>Music Audio that has as its id the audioID if one is found, null if no such Audio is found</returns>
        public static SoundObject GetMusicAudio(int audioID)
        {
            return GetAudio(SoundType.BGM, true, audioID);
        }

        /// <summary>
        /// Returns the first occurrence of music Audio that plays the given audioClip. Returns null if no such Audio is found
        /// </summary>
        /// <param name="audioClip">The audio clip of the music Audio to be retrieved</param>
        /// <returns>First occurrence of music Audio that has as plays the audioClip, null if no such Audio is found</returns>
        public static SoundObject GetMusicAudio(AudioClip audioClip)
        {
            return GetAudio(SoundType.BGM, true, audioClip);
        }

        /// <summary>
        /// Returns the sound fx Audio that has as its id the audioID if one is found, returns null if no such Audio is found
        /// </summary>
        /// <param name="audioID">The id of the sound fx Audio to be returned</param>
        /// <returns>Sound fx Audio that has as its id the audioID if one is found, null if no such Audio is found</returns>
        public static SoundObject GetSoundAudio(int audioID)
        {
            return GetAudio(SoundType.BGM, true, audioID);
        }

        /// <summary>
        /// Returns the first occurrence of sound Audio that plays the given audioClip. Returns null if no such Audio is found
        /// </summary>
        /// <param name="audioClip">The audio clip of the sound Audio to be retrieved</param>
        /// <returns>First occurrence of sound Audio that has as plays the audioClip, null if no such Audio is found</returns>
        public static SoundObject GetSoundAudio(AudioClip audioClip)
        {
            return GetAudio(SoundType.BGM, true, audioClip);
        }

        /// <summary>
        /// Returns the UI sound fx Audio that has as its id the audioID if one is found, returns null if no such Audio is found
        /// </summary>
        /// <param name="audioID">The id of the UI sound fx Audio to be returned</param>
        /// <returns>UI sound fx Audio that has as its id the audioID if one is found, null if no such Audio is found</returns>
        public static SoundObject GetUISoundAudio(int audioID)
        {
            return GetAudio(SoundType.BGM, true, audioID);
        }

        /// <summary>
        /// Returns the first occurrence of UI sound Audio that plays the given audioClip. Returns null if no such Audio is found
        /// </summary>
        /// <param name="audioClip">The audio clip of the UI sound Audio to be retrieved</param>
        /// <returns>First occurrence of UI sound Audio that has as plays the audioClip, null if no such Audio is found</returns>
        public static SoundObject GetUISoundAudio(AudioClip audioClip)
        {
            return GetAudio(SoundType.BGM, true, audioClip);
        }

        private static SoundObject GetAudio(SoundType audioType, bool usePool, int audioID)
        {
            Dictionary<int, SoundObject> audioDict = GetAudioTypeDictionary(audioType);

            if (audioDict.ContainsKey(audioID))
            {
                return audioDict[audioID];
            }

            if (usePool && audioPool.ContainsKey(audioID) && audioPool[audioID].Setting.Type == audioType)
            {
                return audioPool[audioID];
            }

            return null;
        }

        private static SoundObject GetAudio(SoundType audioType, bool usePool, AudioClip audioClip)
        {
            Dictionary<int, SoundObject> audioDict = GetAudioTypeDictionary(audioType);

            List<int> audioTypeKeys = new List<int>(audioDict.Keys);
            List<int> poolKeys = new List<int>(audioPool.Keys);
            List<int> keys = usePool ? audioTypeKeys.Concat(poolKeys).ToList() : audioTypeKeys;
            foreach (int key in keys)
            {
                SoundObject audio = null;
                if (audioDict.ContainsKey(key))
                {
                    audio = audioDict[key];
                }
                else if (audioPool.ContainsKey(key))
                {
                    audio = audioPool[key];
                }
                if (audio == null)
                {
                    return null;
                }
                if (audio.Clip == audioClip && audio.Setting.Type == audioType)
                {
                    return audio;
                }
            }

            return null;
        }

        #endregion

        #region Prepare Function

        /// <summary>
        /// Prepares and initializes background music
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PrepareMusic(AudioClip clip)
        {
            return PrepareAudio(SoundType.BGM, clip, 1f, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes background music
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PrepareMusic(AudioClip clip, float volume)
        {
            return PrepareAudio(SoundType.BGM, clip, volume, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes background music
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the music is looped</param>
        /// <param name = "persist" > Whether the audio persists in between scene changes</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PrepareMusic(AudioClip clip, float volume, bool loop, bool persist)
        {
            return PrepareAudio(SoundType.BGM, clip, volume, loop, persist, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// Prerpares and initializes background music
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the music is looped</param>
        /// <param name="persist"> Whether the audio persists in between scene changes</param>
        /// <param name="fadeInValue">How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)</param>
        /// <param name="fadeOutValue"> How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PrepareMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds)
        {
            return PrepareAudio(SoundType.BGM, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes background music
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the music is looped</param>
        /// <param name="persist"> Whether the audio persists in between scene changes</param>
        /// <param name="fadeInValue">How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)</param>
        /// <param name="fadeOutValue"> How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)</param>
        /// <param name="currentMusicfadeOutSeconds"> How many seconds it needs for current music audio to fade out. It will override its own fade out seconds. If -1 is passed, current music will keep its own fade out seconds</param>
        /// <param name="sourceTransform">The transform that is the source of the music (will become 3D audio). If 3D audio is not wanted, use null</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PrepareMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            return PrepareAudio(SoundType.BGM, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);
        }

        /// <summary>
        /// Prepares and initializes a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PrepareSound(AudioClip clip)
        {
            return PrepareAudio(SoundType.Effect, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PrepareSound(AudioClip clip, float volume)
        {
            return PrepareAudio(SoundType.Effect, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="loop">Wether the sound is looped</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PrepareSound(AudioClip clip, bool loop)
        {
            return PrepareAudio(SoundType.Effect, clip, 1f, loop, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the sound is looped</param>
        /// <param name="sourceTransform">The transform that is the source of the sound (will become 3D audio). If 3D audio is not wanted, use null</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PrepareSound(AudioClip clip, float volume, bool loop, Transform sourceTransform)
        {
            return PrepareAudio(SoundType.Effect, clip, volume, loop, false, 0f, 0f, -1f, sourceTransform);
        }

        /// <summary>
        /// Prepares and initializes a UI sound fx
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PrepareUISound(AudioClip clip)
        {
            return PrepareAudio(SoundType.UIEffect, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Prepares and initializes a UI sound fx
        /// </summary>
        /// <param name="clip">The audio clip to prepare</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PrepareUISound(AudioClip clip, float volume)
        {
            return PrepareAudio(SoundType.UIEffect, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        private static int PrepareAudio(SoundType audioType, AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            if (clip == null)
            {
                Debug.LogError("[Eazy Sound Manager] Audio clip is null", clip);
            }

            Dictionary<int, SoundObject> audioDict = GetAudioTypeDictionary(audioType);
            bool ignoreDuplicateAudio = GetAudioTypeIgnoreDuplicateSetting(audioType);

            if (ignoreDuplicateAudio)
            {
                SoundObject duplicateAudio = GetAudio(audioType, true, clip);
                if (duplicateAudio != null)
                {
                    return duplicateAudio.Setting.SoundID;
                }
            }

            // Create the audioSource
            SoundObject audio = new SoundObject(audioType, clip, loop, persist, volume, fadeInSeconds, fadeOutSeconds, sourceTransform);

            // Add it to dictionary
            audioDict.Add(audio.Setting.SoundID, audio);

            return audio.Setting.SoundID;
        }

        #endregion

        #region Play Functions

        /// <summary>
        /// Play background music
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PlayMusic(AudioClip clip)
        {
            return PlayAudio(SoundType.BGM, clip, 1f, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// Play background music
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PlayMusic(AudioClip clip, float volume)
        {
            return PlayAudio(SoundType.BGM, clip, volume, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// Play background music
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the music is looped</param>
        /// <param name = "persist" > Whether the audio persists in between scene changes</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PlayMusic(AudioClip clip, float volume, bool loop, bool persist)
        {
            return PlayAudio(SoundType.BGM, clip, volume, loop, persist, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// Play background music
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the music is looped</param>
        /// <param name="persist"> Whether the audio persists in between scene changes</param>
        /// <param name="fadeInSeconds">How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)</param>
        /// <param name="fadeOutSeconds"> How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PlayMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds)
        {
            return PlayAudio(SoundType.BGM, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, -1f, null);
        }

        /// <summary>
        /// Play background music
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the music is looped</param>
        /// <param name="persist"> Whether the audio persists in between scene changes</param>
        /// <param name="fadeInSeconds">How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)</param>
        /// <param name="fadeOutSeconds"> How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)</param>
        /// <param name="currentMusicfadeOutSeconds"> How many seconds it needs for current music audio to fade out. It will override its own fade out seconds. If -1 is passed, current music will keep its own fade out seconds</param>
        /// <param name="sourceTransform">The transform that is the source of the music (will become 3D audio). If 3D audio is not wanted, use null</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PlayMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            return PlayAudio(SoundType.BGM, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);
        }

        /// <summary>
        /// Play a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PlaySound(AudioClip clip)
        {
            return PlayAudio(SoundType.Effect, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Play a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PlaySound(AudioClip clip, float volume)
        {
            return PlayAudio(SoundType.Effect, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Play a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="loop">Wether the sound is looped</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PlaySound(AudioClip clip, bool loop)
        {
            return PlayAudio(SoundType.Effect, clip, 1f, loop, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Play a sound fx
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <param name="loop">Wether the sound is looped</param>
        /// <param name="sourceTransform">The transform that is the source of the sound (will become 3D audio). If 3D audio is not wanted, use null</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PlaySound(AudioClip clip, float volume, bool loop, Transform sourceTransform)
        {
            return PlayAudio(SoundType.Effect, clip, volume, loop, false, 0f, 0f, -1f, sourceTransform);
        }

        /// <summary>
        /// Play a UI sound fx
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PlayUISound(AudioClip clip)
        {
            return PlayAudio(SoundType.UIEffect, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// Play a UI sound fx
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume"> The volume the music will have</param>
        /// <returns>The ID of the created Audio object</returns>
        public static int PlayUISound(AudioClip clip, float volume)
        {
            return PlayAudio(SoundType.UIEffect, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        private static int PlayAudio(SoundType audioType, AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            int audioID = PrepareAudio(audioType, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);

            // Stop all current music playing
            if (audioType == SoundType.BGM)
            {
                StopAllMusic(currentMusicfadeOutSeconds);
            }

            GetAudio(audioType, false, audioID).Play();

            return audioID;
        }

        #endregion

        #region Stop Functions

        /// <summary>
        /// Stop all audio playing
        /// </summary>
        public static void StopAll()
        {
            StopAll(-1f);
        }

        /// <summary>
        /// Stop all audio playing
        /// </summary>
        /// <param name="musicFadeOutSeconds"> How many seconds it needs for all music audio to fade out. It will override  their own fade out seconds. If -1 is passed, all music will keep their own fade out seconds</param>
        public static void StopAll(float musicFadeOutSeconds)
        {
            StopAllMusic(musicFadeOutSeconds);
            StopAllSounds();
            StopAllUISounds();
        }

        /// <summary>
        /// Stop all music playing
        /// </summary>
        public static void StopAllMusic()
        {
            StopAllAudio(SoundType.BGM, -1f);
        }

        /// <summary>
        /// Stop all music playing
        /// </summary>
        /// <param name="fadeOutSeconds"> How many seconds it needs for all music audio to fade out. It will override  their own fade out seconds. If -1 is passed, all music will keep their own fade out seconds</param>
        public static void StopAllMusic(float fadeOutSeconds)
        {
            StopAllAudio(SoundType.BGM, fadeOutSeconds);
        }

        /// <summary>
        /// Stop all sound fx playing
        /// </summary>
        public static void StopAllSounds()
        {
            StopAllAudio(SoundType.Effect, -1f);
        }

        /// <summary>
        /// Stop all UI sound fx playing
        /// </summary>
        public static void StopAllUISounds()
        {
            StopAllAudio(SoundType.UIEffect, -1f);
        }

        private static void StopAllAudio(SoundType audioType, float fadeOutSeconds)
        {
            Dictionary<int, SoundObject> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                SoundObject audio = audioDict[key];
                if (fadeOutSeconds > 0)
                {
                    audio.FadeOutSeconds = fadeOutSeconds;
                }
                audio.Stop();
            }
        }

        #endregion

        #region Pause Functions

        /// <summary>
        /// Pause all audio playing
        /// </summary>
        public static void PauseAll()
        {
            PauseAllMusic();
            PauseAllSounds();
            PauseAllUISounds();
        }

        /// <summary>
        /// Pause all music playing
        /// </summary>
        public static void PauseAllMusic()
        {
            PauseAllAudio(SoundType.BGM);
        }

        /// <summary>
        /// Pause all sound fx playing
        /// </summary>
        public static void PauseAllSounds()
        {
            PauseAllAudio(SoundType.Effect);
        }

        /// <summary>
        /// Pause all UI sound fx playing
        /// </summary>
        public static void PauseAllUISounds()
        {
            PauseAllAudio(SoundType.UIEffect);
        }

        private static void PauseAllAudio(SoundType audioType)
        {
            Dictionary<int, SoundObject> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                SoundObject audio = audioDict[key];
                audio.Pause();
            }
        }

        #endregion

        #region Resume Functions

        /// <summary>
        /// Resume all audio playing
        /// </summary>
        public static void ResumeAll()
        {
            ResumeAllMusic();
            ResumeAllSounds();
            ResumeAllUISounds();
        }

        /// <summary>
        /// Resume all music playing
        /// </summary>
        public static void ResumeAllMusic()
        {
            ResumeAllAudio(SoundType.BGM);
        }

        /// <summary>
        /// Resume all sound fx playing
        /// </summary>
        public static void ResumeAllSounds()
        {
            ResumeAllAudio(SoundType.Effect);
        }

        /// <summary>
        /// Resume all UI sound fx playing
        /// </summary>
        public static void ResumeAllUISounds()
        {
            ResumeAllAudio(SoundType.UIEffect);
        }

        private static void ResumeAllAudio(SoundType audioType)
        {
            Dictionary<int, SoundObject> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                SoundObject audio = audioDict[key];
                audio.Resume();
            }
        }

        #endregion
    }

}
