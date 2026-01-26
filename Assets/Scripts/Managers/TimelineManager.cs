using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TimelineManager : MonoBehaviour
{
    public PlayableDirector playableDirector;
    private int currentMarkerIndex = 0;
    public string targetSignalName = "TimelineSkipSignal";

    public void PlayTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Play();
        }
    }

    public void RestartTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.time = 0;
            playableDirector.Play();
        }
    }

    public void PauseTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Pause();
        }
    }

    public void SkipTimeline()
    {
        var timelineAsset = playableDirector.playableAsset as TimelineAsset;
        if (timelineAsset == null) return;

        var markerTrack = timelineAsset.GetOutputTracks().FirstOrDefault(t => t is MarkerTrack) as MarkerTrack;
        if (markerTrack == null) return;

        var skipMarkers = markerTrack.GetMarkers()
            .OfType<SignalEmitter>()
            .Where(signal => signal.asset != null && signal.asset.name == targetSignalName)
            .OrderBy(signal => signal.time)
            .ToArray();

        if (skipMarkers.Length == 0) return; 

        playableDirector.time = skipMarkers[currentMarkerIndex].time;
        playableDirector.Play();
        //playableDirector.Pause();

        currentMarkerIndex = (currentMarkerIndex + 1) % skipMarkers.Length;
    }

    public void Update()
    {
        if(!Input.GetKey(KeyCode.Space) || playableDirector==null)
            return;
            
        if(Input.GetKeyDown(KeyCode.S))
            SkipTimeline();

        if(Input.GetKeyDown(KeyCode.P))
        {
            if(playableDirector.state == PlayState.Playing)
                PauseTimeline();
            else
                PlayTimeline();
        }
            
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TimelineManager))]
public class TimelineManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

         if (!Application.isPlaying)
            return;

        TimelineManager timelineManager = (TimelineManager)target;

        if (GUILayout.Button("Play Timeline"))
            timelineManager.PlayTimeline();

        if (GUILayout.Button("Pause Timeline"))
            timelineManager.PauseTimeline();

        if (GUILayout.Button("Restart Timeline"))
            timelineManager.RestartTimeline();

        if (GUILayout.Button("Skip Timeline"))
            timelineManager.SkipTimeline();
    }
}
#endif
