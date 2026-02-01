using System;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using Path = System.IO.Path;

namespace RIEVES.GGJ2026.Runtime.Videos
{
    internal sealed class StreamingVideoPlayer : MonoBehaviour
    {
        [SerializeField]
        private VideoPlayer videoPlayer;

        [SerializeField]
        private string videoName;

        [SerializeField]
        private UnityEvent onVideoEnded;

        private void Start()
        {
            var path = Path.Combine(Application.streamingAssetsPath, videoName);
            videoPlayer.url = path;
            videoPlayer.Play();
        }

        private void OnEnable()
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        private void OnDisable()
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }

        private void OnVideoEnd(VideoPlayer source)
        {
            onVideoEnded.Invoke();
        }
    }
}
