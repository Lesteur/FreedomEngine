using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using System;
using System.Collections.Generic;

namespace FreedomEngine.Audio
{
    /// <summary>
    /// Manages audio playback for the engine, including background music and sound effects.
    /// </summary>
    public class AudioController : IDisposable
    {
        #region Fields

        /// <summary>
        /// Represents the collection of currently active sound effect instances.
        /// </summary>
        private readonly List<SoundEffectInstance> _activeSoundEffectInstances;

        /// <summary>
        /// Stores the previous volume levels for songs and sound effects to restore after unmuting.
        /// </summary>
        private float _previousSongVolume;

        /// <summary>
        /// Stores the previous volume level for sound effects to restore after unmuting.
        /// </summary>
        private float _previousSoundEffectVolume;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="AudioController"/> class, initializing the active sound effect instances list.
        /// </summary>
        public AudioController()
        {
            _activeSoundEffectInstances = [];
        }

        /// <summary>
        /// Finalizer called when the object is collected by the garbage collector.
        /// </summary>
        ~AudioController() => Dispose(false);

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that indicates if audio is muted.
        /// </summary>
        public bool IsMuted { get; private set; }

        /// <summary>
        /// Gets a value that indicates if this audio controller has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or Sets the global volume of songs.
        /// </summary>
        public float SongVolume
        {
            get => IsMuted ? 0.0f : MediaPlayer.Volume;
            set
            {
                if (!IsMuted)
                    MediaPlayer.Volume = Math.Clamp(value, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// Gets or Sets the global volume of sound effects.
        /// </summary>
        public float SoundEffectVolume
        {
            get => IsMuted ? 0.0f : SoundEffect.MasterVolume;
            set
            {
                if (!IsMuted)
                    SoundEffect.MasterVolume = Math.Clamp(value, 0.0f, 1.0f);
            }
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the controller, cleaning up stopped sound instances to free memory.
        /// </summary>
        public void Update()
        {
            // Iterate backwards to safely remove elements while looping
            for (int i = _activeSoundEffectInstances.Count - 1; i >= 0; i--)
            {
                SoundEffectInstance instance = _activeSoundEffectInstances[i];

                if (instance.State == SoundState.Stopped)
                {
                    if (!instance.IsDisposed)
                    {
                        instance.Dispose();
                    }
                    _activeSoundEffectInstances.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Public Methods (Playback)

        /// <summary>
        /// Plays the given sound effect with default properties.
        /// </summary>
        public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect)
        {
            return PlaySoundEffect(soundEffect, 1.0f, 0.0f, 0.0f, false);
        }

        /// <summary>
        /// Plays the given sound effect with specified volume, pitch, pan, and looping.
        /// </summary>
        public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect, float volume, float pitch, float pan, bool isLooped)
        {
            ArgumentNullException.ThrowIfNull(soundEffect);

            SoundEffectInstance instance = soundEffect.CreateInstance();

            instance.Volume = volume;
            instance.Pitch = pitch;
            instance.Pan = pan;
            instance.IsLooped = isLooped;

            instance.Play();
            _activeSoundEffectInstances.Add(instance);

            return instance;
        }

        /// <summary>
        /// Plays the given song, stopping any currently playing song first.
        /// </summary>
        public void PlaySong(Song song, bool isRepeating = true)
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }

            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = isRepeating;
        }

        #endregion

        #region Public Methods (Control)

        public void PauseAudio()
        {
            MediaPlayer.Pause();

            foreach (var instance in _activeSoundEffectInstances)
                instance.Pause();
        }

        public void ResumeAudio()
        {
            MediaPlayer.Resume();

            foreach (var instance in _activeSoundEffectInstances)
                instance.Resume();
        }

        public void MuteAudio()
        {
            _previousSongVolume = MediaPlayer.Volume;
            _previousSoundEffectVolume = SoundEffect.MasterVolume;

            MediaPlayer.Volume = 0.0f;
            SoundEffect.MasterVolume = 0.0f;

            IsMuted = true;
        }

        public void UnmuteAudio()
        {
            MediaPlayer.Volume = _previousSongVolume;
            SoundEffect.MasterVolume = _previousSoundEffectVolume;

            IsMuted = false;
        }

        public void ToggleMute()
        {
            if (IsMuted) UnmuteAudio();
            else MuteAudio();
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases all resources used by the current instance of the class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                foreach (SoundEffectInstance instance in _activeSoundEffectInstances)
                {
                    if (!instance.IsDisposed)
                        instance.Dispose();
                }
                _activeSoundEffectInstances.Clear();
            }

            IsDisposed = true;
        }

        #endregion
    }
}