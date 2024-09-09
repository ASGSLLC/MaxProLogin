using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEngine.Video;
using UnityEngine.UI;
using Firebase.Auth;
using System;
using maxprofitness.login;

public class VideoBackground : MonoBehaviour
{
    private RawImage rawImage;

    private VideoPlayer videoPlayer;

    private string videoLink = "https://firebasestorage.googleapis.com/v0/b/maxfit-app.appspot.com/o/gameHub%2FVideos%2FGameHubStartScreen.mp4?alt=media&token=55716c5b-8b79-4350-8690-404cdd3da98f";

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
        videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.playOnAwake = true;
        videoPlayer.isLooping = true;
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoLink;

        videoPlayer.started += (videoPlayer) => 
        {
            rawImage.texture = videoPlayer.texture;
        };

       // AuthenticationManager.OnAuthenticationStateChanged += OnAuthStateChanged;
    }

    private void OnAuthStateChanged(FirebaseUser user)
    {

    }

    public void DisableVideo() 
    {
        Debug.Log("VideoBackground//DisableVideo//");

        videoPlayer.Stop();
        rawImage.gameObject.SetActive(false);
    }

    public void EnableVideo()
    {
        Debug.Log("VideoBackground//EnableVideo//");

        rawImage.gameObject.SetActive(true);
        videoPlayer.Play();
    }

    private void OnDestroy()
    {
        //AuthenticationManager.OnAuthenticationStateChanged -= OnAuthStateChanged;
    }
}
