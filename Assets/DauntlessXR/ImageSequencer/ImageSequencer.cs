//Copyright (c) XR Solutions, Inc.
//Licensed under the MIT license. See LICENSE file in the folder root for full license information.

///<summary>
///This script is used to play a sequence of images as an animation on a SpriteRenderer component.  Images can be loaded procedurally or
///from the Unity Editor.  The playback speed, starting frame, playback mode, and reverse playback can be set in the Inspector.  When loading
///images procedurally, make sure to set the playOnStart variable to true to start playback immediately and call the LoadImageSequence() method
///with the overloads desired to set the playback options.  The Play(), Pause(), and Stop() methods can be called to control playback at runtime.
///</summary>
using System.Collections.Generic;
using UnityEngine;

namespace Dauntless.DevTools
{
    public class ImageSequencer : MonoBehaviour
    {
        //--Public variables
        public List<Texture2D> ImageSequence = new List<Texture2D>();
        public enum PlayBackMode
        {
            PlayOnce,
            Loop,
            PingPong
        }

        //--Inspector settings
        [SerializeField]
        [Tooltip("Sprite that the image sequence will play on")]
        private SpriteRenderer renderTarget;

        [SerializeField]
        [Tooltip("Playback speed in frames-per-second")]
        private float playbackSpeed;

        [SerializeField]
        [Tooltip("PlayOnce: Plays animation once, stops at the end of the sequence \n" +
            "Loop: Plays animation continuously, loops at the end of the sequence \n" +
            "PingPong: Plays animation continuously, reverses playback direction at the end of the sequence")]
        private PlayBackMode playbackMode;

        [SerializeField]
        [Tooltip("Start playing animation as soon as component initializes, off by default")]
        private bool playOnStart = false;

        [SerializeField]
        [Tooltip("Reverses the image sequence playback, will not be reversed by default")]
        private bool isReversed = false;

        [SerializeField]
        [Tooltip("Frame that the playback will start on, will start on frame 0 by default")]
        private int startingFrame = 0;

        //--Private variables
        private Sprite[] _renderingSprites;

        private bool _isPlaying = false;
        private int _currentFrame;
        private float _playbackTimer;
        private float _startingPlaybackSpeed;

        #region Unity Methods
        private void Start()
        {
            _currentFrame = startingFrame;
            _playbackTimer = 0;
            _startingPlaybackSpeed = playbackSpeed;

            PrepareSequence(playOnStart);
        }

        private void Update()
        {
            if (_isPlaying)
            {
                _playbackTimer += Time.deltaTime * playbackSpeed;
                if (_playbackTimer >= 1f)
                {
                    if (isReversed)
                    {
                        PreviousFrame();
                    }
                    else
                    {
                        NextFrame();
                    }
                    _playbackTimer = 0f;
                }
            }
        }
        #endregion

        #region Private Methods
        private void NextFrame()
        {
            _currentFrame++;

            if (_currentFrame >= ImageSequence.Count)
            {
                if (playbackMode == PlayBackMode.Loop)
                {
                    _currentFrame = 0;
                }
                else if(playbackMode == PlayBackMode.PingPong)
                {
                    isReversed = !isReversed;
                }
                else
                {
                    _isPlaying = false;
                }
            }

            UpdateDisplay();
        }

        private void PreviousFrame()
        {
            _currentFrame--;

            if (_currentFrame < 0)
            {
                if (playbackMode == PlayBackMode.Loop)
                {
                    _currentFrame = ImageSequence.Count - 1;
                }
                else if (playbackMode == PlayBackMode.PingPong)
                {
                    isReversed = !isReversed;
                }
                else
                {
                    _isPlaying = false;
                }
            }

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            renderTarget.sprite = _renderingSprites[_currentFrame];
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Plays image sequence from the current frame
        /// </summary>
        public void Play()
        {
            _isPlaying = true;
            playbackSpeed = _startingPlaybackSpeed;
            isReversed = false;
        }

        /// <summary>
        /// Plays image sequence from the current frame with the specified playback mode
        /// </summary>
        /// <param name="playMode"></param>
        public void Play(PlayBackMode playMode)
        {
            _isPlaying = true;
            playbackSpeed = _startingPlaybackSpeed;
            isReversed = false;
            playbackMode = playMode;
        }

        /// <summary>
        /// Plays image sequence from the starting frame with the specified playback mode
        /// </summary>
        /// <param name="playMode"></param>
        public void PlayFromStart(PlayBackMode playMode)
        {
            _isPlaying = true;
            _currentFrame = startingFrame;
            playbackSpeed = _startingPlaybackSpeed;
            isReversed = false;
            playbackMode = playMode;
        }

        /// <summary>
        /// Plays image sequence from the specified frame with the specified playback mode
        /// </summary>
        /// <param name="frameIndex"></param>
        /// <param name="playMode"></param>
        public void PlayFromFrame(int frameIndex, PlayBackMode playMode)
        {
            _isPlaying = true;
            playbackMode = playMode;
            _currentFrame = frameIndex;
        }

        /// <summary>
        /// Pauses image sequence playback at the current frame, use Play() to resume playback
        /// </summary>
        public void Pause()
        {
            _isPlaying = false;
        }

        /// <summary>
        /// Stops image sequence playback and resets the playback to the starting frame, playback speed, and direction
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
            _currentFrame = startingFrame;
            renderTarget.sprite = null;
            _playbackTimer = 0f;
            playbackSpeed = _startingPlaybackSpeed;
            isReversed = false;
        }

        /// <summary>
        /// Increases the playback speed by the specified amount
        /// </summary>
        /// <param name="speed"></param>
        public void FastForward(float speed)
        {
            playbackSpeed *= speed;

            if(isReversed == true)
            {
                isReversed = false;
            }
        }

        /// <summary>
        /// Decreases the playback speed by the specified amount
        /// </summary>
        /// <param name="speed"></param>
        public void Rewind(float speed)
        {
            playbackSpeed *= speed;

            if(isReversed == false)
            {
                isReversed = true;
            }
        }

        /// <summary>
        /// Loads a new image sequence and plays it from the starting frame, set playOnComplete to true to start playback immediately
        /// </summary>
        /// <param name="images"></param>
        /// <param name="playOnComplete"></param>
        public void LoadImageSequence(List<Texture2D> images, bool playOnComplete)
        {
            ImageSequence.Clear();

            ImageSequence = images;

            PrepareSequence(playOnComplete);
        }

        /// <summary>
        /// Loads a new image sequence and plays it with the specified playback mode, set playOnComplete to true to start playback immediately
        /// </summary>
        /// <param name="images"></param>
        /// <param name="playOnComplete"></param>
        /// <param name="playMode"></param>
        public void LoadImageSequence(List<Texture2D> images, bool playOnComplete, PlayBackMode playMode)
        {
            ImageSequence.Clear();

            ImageSequence = images;
            playbackMode = playMode;
            
            PrepareSequence(playOnComplete);
        }

        /// <summary>
        /// Loads a new image sequence and plays it with the specified playback mode and starting frame, set playOnComplete to true to start playback immediately
        /// </summary>
        /// <param name="images"></param>
        /// <param name="playOnComplete"></param>
        /// <param name="playMode"></param>
        /// <param name="startingIndex"></param>
        public void LoadImageSequence(List<Texture2D> images, bool playOnComplete, PlayBackMode playMode, int startingIndex)
        {
            ImageSequence.Clear();

            ImageSequence = images;
            playbackMode = playMode;
            startingFrame = startingIndex;

            PrepareSequence(playOnComplete);
        }

        /// <summary>
        /// Loads a new image sequence and plays it with the specified playback mode, starting frame, and playback speed, set playOnComplete to true to start playback immediately
        /// </summary>
        /// <param name="images"></param>
        /// <param name="playOnComplete"></param>
        /// <param name="playMode"></param>
        /// <param name="startingIndex"></param>
        /// <param name="speed"></param>
        public void LoadImageSequence(List<Texture2D> images, bool playOnComplete, PlayBackMode playMode, int startingIndex, float speed)
        {
            ImageSequence.Clear();

            ImageSequence = images;
            playbackMode = playMode;
            startingFrame = startingIndex;
            playbackSpeed = speed;

            PrepareSequence(playOnComplete);
        }

        /// <summary>
        /// Loads a new image sequence and plays it with the specified playback mode, starting frame, playback speed, and reverse playback, set playOnComplete to true to start playback immediately
        /// </summary>
        /// <param name="images"></param>
        /// <param name="playOnComplete"></param>
        /// <param name="playMode"></param>
        /// <param name="startingIndex"></param>
        /// <param name="speed"></param>
        /// <param name="reversePlayback"></param>
        public void LoadImageSequence(List<Texture2D> images, bool playOnComplete, PlayBackMode playMode, int startingIndex, float speed, bool reversePlayback)
        {
            ImageSequence.Clear();

            ImageSequence = images;
            playbackMode = playMode;
            startingFrame = startingIndex;
            playbackSpeed = speed;
            isReversed = reversePlayback;

            PrepareSequence(playOnComplete);
        }

        /// <summary>
        /// Prepares the image sequence as sprites for playback, set playOnComplete to true to start playback immediately
        /// </summary>
        /// <param name="playOnComplete"></param>
        public void PrepareSequence(bool playOnComplete)
        {
            _renderingSprites = new Sprite[ImageSequence.Count];

            for (int i = 0; i < ImageSequence.Count; i++)
            {
                Sprite sprite = Sprite.Create(ImageSequence[i], new Rect(0.0f, 0.0f, ImageSequence[i].width, ImageSequence[i].height), new Vector2(0.5f, 0.5f), 100.0f);
                _renderingSprites[i] = sprite;
            }

            _isPlaying = playOnComplete;
        }
        #endregion
    }
}

