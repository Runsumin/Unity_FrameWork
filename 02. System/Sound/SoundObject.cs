using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOBS
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // SoundObject
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class SoundObject
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Nested Class
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [NestedClass] Setting
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [Serializable]
        public class SoundObjectSetting
        {
            public int SoundID;
            public SoundType Type;
            public bool isPlaying;
            public bool Paused;
            public bool Stopping;
            public bool Activated;
            public bool Pooled;
            public float Volume;

        }
        public SoundObjectSetting Setting = new SoundObjectSetting();
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] Base
        public AudioSource AudioSource { get; private set; }

        private static int audioCounter = 0;

        private AudioClip clip;
        private bool loop;
        private bool mute;
        private int priority;
        private float pitch;
        private float stereoPan;
        private float spatialBlend;
        private float reverbZoneMix;
        private float dopplerLevel;
        private float spread;
        private AudioRolloffMode rolloffMode;
        private float max3DDistance;
        private float min3DDistance;

        private float targetVolume;
        private float initTargetVolume;
        private float tempFadeSeconds;
        private float fadeInterpolater;
        private float onFadeStartVolume;
        private Transform sourceTransform;
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] Base
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Transform SourceTransform { get { return sourceTransform; } set { if (value == null) { } else { sourceTransform = value; } } }
        public AudioClip Clip { get { return clip; } set { clip = value; if (AudioSource != null) { AudioSource.clip = clip; } } }
        public bool Loop { get { return loop; } set { loop = value; if (AudioSource != null) { AudioSource.loop = loop; } } }
        public bool Mute { get { return mute; } set { mute = value; if (AudioSource != null) { AudioSource.mute = mute; } } }
        public int Priority { get { return priority; } set { priority = Mathf.Clamp(value, 0, 256); if (AudioSource != null) { AudioSource.priority = priority; } } }

        /// <summary>
        /// The pitch of the audio
        /// </summary>
        public float Pitch { get { return pitch; } set { pitch = Mathf.Clamp(value, -3, 3); if (AudioSource != null) { AudioSource.pitch = pitch; } } }

        /// <summary>
        /// Pans a playing sound in a stereo way (left or right). This only applies to sounds that are Mono or Stereo.
        /// </summary>
        public float StereoPan { get { return stereoPan; } set { stereoPan = Mathf.Clamp(value, -1, 1); if (AudioSource != null) { AudioSource.panStereo = stereoPan; } } }

        /// <summary>
        /// Sets how much this AudioSource is affected by 3D spatialisation calculations (attenuation, doppler etc). 0.0 makes the sound full 2D, 1.0 makes it full 3D.
        /// </summary>
        public float SpatialBlend { get { return spatialBlend; } set { spatialBlend = Mathf.Clamp01(value); if (AudioSource != null) { AudioSource.spatialBlend = spatialBlend; } } }

        /// <summary>
        /// The amount by which the signal from the AudioSource will be mixed into the global reverb associated with the Reverb Zones.
        /// </summary>
        public float ReverbZoneMix { get { return reverbZoneMix; } set { reverbZoneMix = Mathf.Clamp(value, 0, 1.1f); if (AudioSource != null) { AudioSource.reverbZoneMix = reverbZoneMix; } } }

        /// <summary>
        /// The doppler scale of the audio
        /// </summary>
        public float DopplerLevel { get { return dopplerLevel; } set { dopplerLevel = Mathf.Clamp(value, 0, 5); if (AudioSource != null) { AudioSource.dopplerLevel = dopplerLevel; } } }

        /// <summary>
        /// The spread angle (in degrees) of a 3d stereo or multichannel sound in speaker space.
        /// </summary>
        public float Spread { get { return spread; } set { spread = Mathf.Clamp(value, 0, 360); if (AudioSource != null) { AudioSource.spread = spread; } } }

        /// <summary>
        /// How the audio attenuates over distance
        /// </summary>
        public AudioRolloffMode RolloffMode { get { return rolloffMode; } set { rolloffMode = value; if (AudioSource != null) { AudioSource.rolloffMode = rolloffMode; } } }

        /// <summary>
        /// (Logarithmic rolloff) MaxDistance is the distance a sound stops attenuating at.
        /// </summary>
        public float Max3DDistance { get { return max3DDistance; } set { max3DDistance = Mathf.Max(value, 0.01f); if (AudioSource != null) { AudioSource.maxDistance = max3DDistance; } } }

        /// <summary>
        /// Within the Min distance the audio will cease to grow louder in volume.
        /// </summary>
        public float Min3DDistance { get { return min3DDistance; } set { min3DDistance = Mathf.Max(value, 0); if (AudioSource != null) { AudioSource.minDistance = min3DDistance; } } }

        /// <summary>
        /// Whether the audio persists in between scene changes
        /// </summary>
        public bool Persist { get; set; }

        /// <summary>
        /// How many seconds it needs for the audio to fade in/ reach target volume (if higher than current)
        /// </summary>
        public float FadeInSeconds { get; set; }

        /// <summary>
        /// How many seconds it needs for the audio to fade out/ reach target volume (if lower than current)
        /// </summary>
        public float FadeOutSeconds { get; set; }
        #endregion

        public SoundObject(SoundType audioType, AudioClip clip, bool loop, bool persist, float volume, float fadeInValue, float fadeOutValue, Transform sourceTransform)
        {
            // Set unique audio ID
            Setting.SoundID = audioCounter;
            audioCounter++;

            // Initialize values
            this.Setting.Type = audioType;
            this.Clip = clip;
            this.SourceTransform = sourceTransform;
            this.Loop = loop;
            this.Persist = persist;
            this.targetVolume = volume;
            this.initTargetVolume = volume;
            this.tempFadeSeconds = -1;
            this.FadeInSeconds = fadeInValue;
            this.FadeOutSeconds = fadeOutValue;

            Setting.Volume = 0f;
            Setting.Pooled = false;

            // Set audiosource default values
            Mute = false;
            Priority = 128;
            Pitch = 1;
            StereoPan = 0;
            //////if (sourceTransform != null && sourceTransform != EazySoundManager.Gameobject.transform)
            //////{
            //////    SpatialBlend = 1;
            //////}
            ReverbZoneMix = 1;
            DopplerLevel = 1;
            Spread = 0;
            RolloffMode = AudioRolloffMode.Logarithmic;
            Min3DDistance = 1;
            Max3DDistance = 500;

            // Initliaze states
            Setting.isPlaying = false;
            Setting.Paused = false;
            Setting.Activated = false;
        }

        /// <summary>
        /// Creates and initializes the audiosource component with the appropriate values
        /// </summary>
        private void CreateAudiosource()
        {
            AudioSource = SourceTransform.gameObject.AddComponent<AudioSource>() as AudioSource;
            AudioSource.clip = Clip;
            AudioSource.loop = Loop;
            AudioSource.mute = Mute;
            AudioSource.volume = Setting.Volume;
            AudioSource.priority = Priority;
            AudioSource.pitch = Pitch;
            AudioSource.panStereo = StereoPan;
            AudioSource.spatialBlend = SpatialBlend;
            AudioSource.reverbZoneMix = ReverbZoneMix;
            AudioSource.dopplerLevel = DopplerLevel;
            AudioSource.spread = Spread;
            AudioSource.rolloffMode = RolloffMode;
            AudioSource.maxDistance = Max3DDistance;
            AudioSource.minDistance = Min3DDistance;
        }

        /// <summary>
        /// Start playing audio clip from the beginning
        /// </summary>
        public void Play()
        {
            Play(initTargetVolume);
        }

        /// <summary>
        /// Start playing audio clip from the beggining
        /// </summary>
        /// <param name="volume">The target volume</param>
        public void Play(float volume)
        {
            // Check if audio still exists in sound manager
            if (Setting.Pooled)
            {
                // If not, restore it from the audioPool
                //////bool restoredFromPool = EazySoundManager.RestoreAudioFromPool(Type, Setting.SoundID);
                //////if (!restoredFromPool)
                //////{
                //////    return;
                //////}

                Setting.Pooled = true;
            }

            // Recreate audiosource if it does not exist
            if (AudioSource == null)
            {
                CreateAudiosource();
            }

            AudioSource.Play();
            Setting.isPlaying = true;

            fadeInterpolater = 0f;
            onFadeStartVolume = this.Setting.Volume;
            targetVolume = volume;
        }

        /// <summary>
        /// Stop playing audio clip
        /// </summary>
        public void Stop()
        {
            fadeInterpolater = 0f;
            onFadeStartVolume = Setting.Volume;
            targetVolume = 0f;

            Setting.Stopping = true;
        }

        /// <summary>
        /// Pause playing audio clip
        /// </summary>
        public void Pause()
        {
            AudioSource.Pause();
            Setting.Paused = true;
        }

        /// <summary>
        /// Resume playing audio clip
        /// </summary>
        public void UnPause()
        {
            AudioSource.UnPause();
            Setting.Paused = false;
        }

        /// <summary>
        /// Resume playing audio clip
        /// </summary>
        public void Resume()
        {
            AudioSource.UnPause();
            Setting.Paused = false;
        }

        /// <summary>
        /// Sets the audio volume
        /// </summary>
        /// <param name="volume">The target volume</param>
        public void SetVolume(float volume)
        {
            if (volume > targetVolume)
            {
                SetVolume(volume, FadeOutSeconds);
            }
            else
            {
                SetVolume(volume, FadeInSeconds);
            }
        }

        /// <summary>
        /// Sets the audio volume
        /// </summary>
        /// <param name="volume">The target volume</param>
        /// <param name="fadeSeconds">How many seconds it needs for the audio to fade in/out to reach target volume. If passed, it will override the Audio's fade in/out seconds, but only for this transition</param>
        public void SetVolume(float volume, float fadeSeconds)
        {
            SetVolume(volume, fadeSeconds, this.Setting.Volume);
        }

        /// <summary>
        /// Sets the audio volume
        /// </summary>
        /// <param name="volume">The target volume</param>
        /// <param name="fadeSeconds">How many seconds it needs for the audio to fade in/out to reach target volume. If passed, it will override the Audio's fade in/out seconds, but only for this transition</param>
        /// <param name="startVolume">Immediately set the volume to this value before beginning the fade. If not passed, the Audio will start fading from the current volume towards the target volume</param>
        public void SetVolume(float volume, float fadeSeconds, float startVolume)
        {
            targetVolume = Mathf.Clamp01(volume);
            fadeInterpolater = 0;
            onFadeStartVolume = startVolume;
            tempFadeSeconds = fadeSeconds;
        }

        /// <summary>
        /// Sets the Audio 3D distances
        /// </summary>
        /// <param name="min">the min distance</param>
        /// <param name="max">the max distance</param>
        public void Set3DDistances(float min, float max)
        {
            Min3DDistance = min;
            Max3DDistance = max;
        }

        /// <summary>
        /// Update loop of the Audio. This is automatically called from the sound manager itself. Do not use this function anywhere else, as it may lead to unwanted behaviour.
        /// </summary>
        public void Update()
        {
            if (AudioSource == null)
            {
                return;
            }

            Setting.Activated = true;

            // Increase/decrease volume to reach the current target
            if (Setting.Volume != targetVolume)
            {
                float fadeValue;
                fadeInterpolater += Time.unscaledDeltaTime;
                if (Setting.Volume > targetVolume)
                {
                    fadeValue = tempFadeSeconds != -1 ? tempFadeSeconds : FadeOutSeconds;
                }
                else
                {
                    fadeValue = tempFadeSeconds != -1 ? tempFadeSeconds : FadeInSeconds;
                }

                Setting.Volume = Mathf.Lerp(onFadeStartVolume, targetVolume, fadeInterpolater / fadeValue);
            }
            else if (tempFadeSeconds != -1)
            {
                tempFadeSeconds = -1;
            }

            // Set the volume, taking into account the global volumes as well.
            switch (Setting.Type)
            {
                case SoundType.BGM:
                    {
                        AudioSource.volume = Setting.Volume/* * EazySoundManager.GlobalMusicVolume * EazySoundManager.GlobalVolume*/;
                        break;
                    }
                case SoundType.Effect:
                    {
                        AudioSource.volume = Setting.Volume/* * EazySoundManager.GlobalSoundsVolume * EazySoundManager.GlobalVolume*/;
                        break;
                    }
                case SoundType.UIEffect:
                    {
                        AudioSource.volume = Setting.Volume /** EazySoundManager.GlobalUISoundsVolume * EazySoundManager.GlobalVolume*/;
                        break;
                    }
            }

            // Completely stop audio if it finished the process of stopping
            if (Setting.Volume == 0f && Setting.Stopping)
            {
                AudioSource.Stop();
                Setting.Stopping = false;
                Setting.isPlaying = false;
                Setting.Paused = false;
            }

            // Update playing status
            if (AudioSource.isPlaying != Setting.isPlaying && Application.isFocused)
            {
                Setting.isPlaying = AudioSource.isPlaying;
            }
        }
    }

}
