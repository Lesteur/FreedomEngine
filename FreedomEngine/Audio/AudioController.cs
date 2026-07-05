using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Audio
{
    /// <summary>
    /// Manages audio playback for the engine, including background music and sound effects.
    /// </summary>
    public class AudioController : IProcessManager
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

        #region Properties

        /// <summary>
        /// Gets the number of currently active audio processes.
        /// </summary>
        public int ActiveCount => _activeSoundEffectInstances.Count;

        /// <summary>
        /// Gets whether there are any active audio processes.
        /// </summary>
        public bool HasActiveProcesses => _activeSoundEffectInstances.Count > 0;

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
        /// <remarks>
        /// If IsMuted is true, the getter will always return back 0.0f and the
        /// setter will ignore setting the volume.
        /// </remarks>
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
        /// <remarks>
        /// If IsMuted is true, the getter will always return back 0.0f and the
        /// setter will ignore setting the volume.
        /// </remarks>
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

        #region Lifecycle Methods

        /// <summary>
        /// Updates the controller, cleaning up stopped sound instances to free memory.
        /// </summary>
        public void Update(GameTime gameTime)
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

        #region Public Methods

        /// <summary>
        /// Pauses all audio.
        /// </summary>
        public void PauseAll()
        {
            MediaPlayer.Pause();

            foreach (var instance in _activeSoundEffectInstances)
                instance.Pause();
        }

        /// <summary>
        /// Resumes play of all previous paused audio.
        /// </summary>
        public void ResumeAll()
        {
            MediaPlayer.Resume();

            foreach (var instance in _activeSoundEffectInstances)
                instance.Resume();
        }

        /// <summary>
        /// Stops all active audio immediately.
        /// </summary>
        public void StopAll()
        {
            MediaPlayer.Stop();
            foreach (var instance in _activeSoundEffectInstances)
                instance.Stop();
        }

        /// <summary>
        /// Plays the given sound effect.
        /// </summary>
        /// <param name="soundEffect">The sound effect to play.</param>
        /// <returns>The sound effect instance created by this method.</returns>
        public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect)
        {
            return PlaySoundEffect(soundEffect, 1.0f, 0.0f, 0.0f, false);
        }

        /// <summary>
        /// Plays the given sound effect with the specified properties.
        /// </summary>
        /// <param name="soundEffect">The sound effect to play.</param>
        /// <param name="volume">The volume, ranging from 0.0 (silence) to 1.0 (full volume).</param>
        /// <param name="pitch">The pitch adjustment, ranging from -1.0 (down an octave) to 0.0 (no change) to 1.0 (up an octave).</param>
        /// <param name="pan">The panning, ranging from -1.0 (left speaker) to 0.0 (centered), 1.0 (right speaker).</param>
        /// <param name="isLooped">Whether the the sound effect should loop after playback.</param>
        /// <returns>The sound effect instance created by playing the sound effect.</returns>
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
        /// Plays the given song.
        /// </summary>
        /// <param name="song">The song to play.</param>
        /// <param name="isRepeating">Optionally specify if the song should repeat. Default is true.</param>
        public void PlaySong(Song song, bool isRepeating = true)
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }

            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = isRepeating;
        }

        /// <summary>
        /// Mutes all audio.
        /// </summary>
        public void MuteAudio()
        {
            _previousSongVolume = MediaPlayer.Volume;
            _previousSoundEffectVolume = SoundEffect.MasterVolume;

            MediaPlayer.Volume = 0.0f;
            SoundEffect.MasterVolume = 0.0f;

            IsMuted = true;
        }

        /// <summary>
        /// Unmutes all audio to the volume level prior to muting.
        /// </summary>
        public void UnmuteAudio()
        {
            MediaPlayer.Volume = _previousSongVolume;
            SoundEffect.MasterVolume = _previousSoundEffectVolume;

            IsMuted = false;
        }

        /// <summary>
        /// Toggles the current audio mute state.
        /// </summary>
        public void ToggleMute()
        {
            if (IsMuted)
                UnmuteAudio();
            else
                MuteAudio();
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of this audio controller and cleans up resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this audio controller and cleans up resources.
        /// </summary>
        /// <param name="disposing">Indicates whether managed resources should be disposed.</param>
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