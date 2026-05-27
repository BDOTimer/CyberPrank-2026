using UnityEngine;
using UnityEngine.Video;

public class TV : MonoBehaviour, IInteractable
{
    private VideoPlayer videoPlayer;
    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    public void OnInteract(float hitDistance)
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            videoPlayer.targetTexture.Release();
        }
        else
        {
            videoPlayer.Play();
        }
    }

    public void OnInteractCanceled()
    {
    }

    void OnDestroy()
    {
        videoPlayer.targetTexture.Release();
    }
}
