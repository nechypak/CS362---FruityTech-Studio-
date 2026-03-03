using System.Collections;
using UnityEngine.TestTools;

using NUnit.Framework;
using UnityEngine;

public class PianoNoteMap_UnitTests
{
    [Test]
    public void GetClipForRow_ClampsOutOfRangeRow()
    {
        var map = ScriptableObject.CreateInstance<PianoNoteMap>();

        // Build a tiny clip array so clamping is obvious
        map.rowClips = new AudioClip[3];
        var clip0 = AudioClip.Create("clip0", 100, 1, 44100, false);
        var clip2 = AudioClip.Create("clip2", 100, 1, 44100, false);
        map.rowClips[0] = clip0;
        map.rowClips[2] = clip2;

        // Below range clamps to 0
        Assert.AreSame(clip0, map.GetClipForRow(-999));

        // Above range clamps to last index (2)
        Assert.AreSame(clip2, map.GetClipForRow(999));

        Object.DestroyImmediate(map);
    }
}