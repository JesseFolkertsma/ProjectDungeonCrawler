using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour {

    public static bool initialized = false;
    public static int widthMin = 3, widthMax = 5;
    public static List<TagManager.TagType> tags =
        new List<TagManager.TagType>() {TagManager.TagType.Default };

    public void Initialize(int _widthMin, int _widthMax, List<TagManager.TagType> _tags)
    {
        widthMin = _widthMin;
        widthMax = _widthMax;
        tags = _tags;
        initialized = true;
    }
}
