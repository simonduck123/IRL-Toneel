using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

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

        // Find the Marker Track
        var markerTrack = timelineAsset.GetOutputTracks().FirstOrDefault(t => t is MarkerTrack) as MarkerTrack;
        if (markerTrack == null) return;

        // Get all "Skip" markers and sort by their time on the timeline
        var skipMarkers = markerTrack.GetMarkers()
            .OfType<SignalEmitter>()
            .Where(signal => signal.asset != null && signal.asset.name == targetSignalName)
            .OrderBy(signal => signal.time)  // Sort by time (earliest first)
            .ToArray();

        if (skipMarkers.Length == 0) return;  // Exit if no markers found

        // Move the timeline to the next "Skip" marker
        playableDirector.time = skipMarkers[currentMarkerIndex].time;
        //playableDirector.Play();

        // Update index: Move to next marker or loop back to 0 if at the end
        currentMarkerIndex = (currentMarkerIndex + 1) % skipMarkers.Length;
    }

}
