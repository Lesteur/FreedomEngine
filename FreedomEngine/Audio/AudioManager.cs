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
    public class AudioManager : IProcessManager
    {
        #region Fields

        /// <summary>
        /// The collection of currently active sound effect instances.
        /// </summary>
        private readonly List<SoundEffectInstance> _activeSoundEffectInstances;

        /// <summary>
        /// Stores the previous song volume level to restore after unmuting.
        /// </summary>
        private float _previousSongVolume;

        /// <summary>
        /// Stores the previous sound effect volume level to restore after unmuting.
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
        /// Gets a value indicating whether audio is currently muted.
        /// </summary>
        public bool IsMuted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this audio manager has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or sets the global volume of songs, from 0.0 (silence) to 1.0 (full volume).
        /// </summary>
        /// <remarks>
        /// If <see cref="IsMuted"/> is <see langword="true"/>, the getter always returns 0.0f and the
        /// setter is ignored.
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
        /// Gets or sets the global volume of sound effects, from 0.0 (silence) to 1.0 (full volume).
        /// </summary>
        /// <remarks>
        /// If <see cref="IsMuted"/> is <see langword="true"/>, the getter always returns 0.0f and the
        /// setter is ignored.
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
        /// Initializes a new instance of the <see cref="AudioManager"/> class.
        /// </summary>
        public AudioManager()
        {
            _activeSoundEffectInstances = [];
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the manager, cleaning up stopped sound effect instances to free memory.
        /// </summary>
        /// <param name="gameTime">A snapshot of the game's timing values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        public void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);

            // Iterate backwards to safely remove elements while looping.
            for (int i = _activeSoundEffectInstances.Count - 1; i >= 0; i--)
            {
                SoundEffectInstance instance = _activeSoundEffectInstances[i];

                if (instance.State == SoundState.Stopped)
                {
                    if (!instance.IsDisposed)
                        instance.Dispose();

                    _activeSoundEffectInstances.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pauses all audio, including the current song and all active sound effects.
        /// </summary>
        public void PauseAll()
        {
            MediaPlayer.Pause();

            foreach (var instance in _activeSoundEffectInstances)
                instance.Pause();
        }

        /// <summary>
        /// Resumes playback of all previously paused audio.
        /// </summary>
        public void ResumeAll()
        {
            MediaPlayer.Resume();

            foreach (var instance in _activeSoundEffectInstances)
                instance.Resume();
        }

        /// <summary>
        /// Stops all active audio immediately, including the current song and all active sound effects.
        /// </summary>
        public void StopAll()
        {
            MediaPlayer.Stop();

            foreach (var instance in _activeSoundEffectInstances)
                instance.Stop();
        }

        /// <summary>
        /// Plays the given sound effect at full volume, centered, with no pitch adjustment, once.
        /// </summary>
        /// <param name="soundEffect">The sound effect to play.</param>
        /// <returns>The sound effect instance created by this method.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="soundEffect"/> is <see langword="null"/>.</exception>
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
        /// <param name="pan">The panning, ranging from -1.0 (left speaker) to 0.0 (centered) to 1.0 (right speaker).</param>
        /// <param name="isLooped">Whether the sound effect should loop after playback.</param>
        /// <returns>The sound effect instance created by playing the sound effect.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="soundEffect"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="volume"/> is outside the range [0.0, 1.0], or <paramref name="pitch"/> or
        /// <paramref name="pan"/> is outside the range [-1.0, 1.0].
        /// </exception>
        public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect, float volume, float pitch, float pan, bool isLooped)
        {
            ArgumentNullException.ThrowIfNull(soundEffect);

            if (volume < 0.0f || volume > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(volume), volume, "Volume must be between 0.0 and 1.0.");

            if (pitch < -1.0f || pitch > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(pitch), pitch, "Pitch must be between -1.0 and 1.0.");

            if (pan < -1.0f || pan > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(pan), pan, "Pan must be between -1.0 and 1.0.");

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
        /// Plays the given song, stopping any song that is currently playing.
        /// </summary>
        /// <param name="song">The song to play.</param>
        /// <param name="isRepeating">Whether the song should repeat. Defaults to <see langword="true"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="song"/> is <see langword="null"/>.</exception>
        public void PlaySong(Song song, bool isRepeating = true)
        {
            ArgumentNullException.ThrowIfNull(song);

            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }

            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = isRepeating;
        }

        /// <summary>
        /// Mutes all audio, remembering the current volume levels so they can be restored by
        /// <see cref="UnmuteAudio"/>.
        /// </summary>
        /// <remarks>Calling this method while already muted has no effect.</remarks>
        public void MuteAudio()
        {
            if (IsMuted)
                return;

            _previousSongVolume = MediaPlayer.Volume;
            _previousSoundEffectVolume = SoundEffect.MasterVolume;

            MediaPlayer.Volume = 0.0f;
            SoundEffect.MasterVolume = 0.0f;

            IsMuted = true;
        }

        /// <summary>
        /// Unmutes all audio, restoring the volume levels captured by <see cref="MuteAudio"/>.
        /// </summary>
        /// <remarks>Calling this method while not muted has no effect.</remarks>
        public void UnmuteAudio()
        {
            if (!IsMuted)
                return;

            MediaPlayer.Volume = _previousSongVolume;
            SoundEffect.MasterVolume = _previousSoundEffectVolume;

            IsMuted = false;
        }

        /// <summary>
        /// Toggles the current audio mute state, muting if currently unmuted and vice versa.
        /// </summary>
        public void ToggleMute()
        {
            if (IsMuted)
                UnmuteAudio();
            else
                MuteAudio();
        }

        /// <summary>
        /// Stops all audio and releases every active sound effect instance.
        /// </summary>
        public void Clear()
        {
            StopAll();

            foreach (var instance in _activeSoundEffectInstances)
            {
                if (!instance.IsDisposed)
                    instance.Dispose();
            }

            _activeSoundEffectInstances.Clear();
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of this audio manager and cleans up resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of this audio manager and cleans up resources.
        /// </summary>
        /// <param name="disposing">Indicates whether managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

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