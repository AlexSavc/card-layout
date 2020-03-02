using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Utility
{
    public static List<GameObject> GetSiblings(GameObject obj)
    {
        List<GameObject> objs = new List<GameObject>();
        for (int i = 0; i < obj.transform.parent.childCount; i++)
        {
            objs.Add(obj.transform.parent.GetChild(i).gameObject);
        }
        return objs;
    }

    public static List<GameObject> GetChildren(Transform form)
    {
        List<GameObject> objs = new List<GameObject>();
        for (int i = 0; i < form.childCount; i++)
        {
            objs.Add(form.GetChild(i).gameObject);
        }
        return objs;
    }

    public static void ClearChildren(GameObject target)
    {
        GameObject[] children = GetChildren(target.transform).ToArray();

        foreach (GameObject child in children)
        {
            Object.Destroy(child);
        }
    }

    public static List<GameObject> GetActiveSiblings(GameObject Obj)
    {
        List<GameObject> temp = new List<GameObject>();
        foreach (GameObject obj in Utility.GetSiblings(Obj))
        {
            if (obj != Obj && obj.activeInHierarchy == true)
            {
                temp.Add(obj);
            }
        }

        return temp;
    }

    public static void SetActiveAll(List<GameObject> toDeactivate, bool active)
    {
        if (toDeactivate == null) return;
        foreach (GameObject obj in toDeactivate)
        {
            obj.SetActive(active);
        }
    }

    public static void SetActiveAll(GameObject[] toDeactivate, bool active)
    {
        foreach (GameObject obj in toDeactivate)
        {
            obj.SetActive(active);
        }
    }

    public static void CheckDuplicates(List<object> toCheck)
    {
        List<object> objectList = new List<object>();

        foreach (object obj in toCheck)
        {
            if (objectList.Contains(obj) == false) objectList.Add(obj);
        }
    }

    public static float BrownianMotion(int x, int y, BrownianMotionData data)
    {

        //for each pixel, get the value total = 0.0f; frequency = 1.0f/(float)hgrid; amplitude = gain;

        float total = 0;

        for (int i = 0; i < data.octaves; ++i)
        {
            total += Mathf.PerlinNoise((float)x * data.frequency, (float)y * data.frequency) * data.amplitude;
            data.frequency *= data.lacunarity;
            data.amplitude *= data.gain;
        }

        Debug.Log(total);
        //now that we have the value, put it in map[x][y]=total;
        
        return total;
    }
    
    public static int CoordsToArrayPos(int x, int y, int sizeY)
    {
        return x * sizeY + y;
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataMousePos = new PointerEventData(EventSystem.current);
        Vector2 mousePos = Input.mousePosition;
        eventDataMousePos.position = new Vector2(mousePos.x, mousePos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataMousePos, results);

        if (results.Count > 0) return true;
        else return false;
    }

    public static List<RaycastResult> GetPointerRaycastResults()
    {
        PointerEventData eventDataMousePos = new PointerEventData(EventSystem.current);
        Vector2 mousePos = Input.mousePosition;
        eventDataMousePos.position = new Vector2(mousePos.x, mousePos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataMousePos, results);

        return results;
    }

    public static GameObject GetFirstParentWithComponent(GameObject origin, System.Type type)
    {
        int arbitraryCutoff = 1000;
        Transform parent = origin.transform.parent;
        int go = 0;
        while (go < arbitraryCutoff)
        {
            go++;
            if (parent.GetComponent(type))
            {
                return parent.gameObject;
            }
            else if (parent.parent == null) return null;
            else parent = parent.parent;
        }

        return null;
    }

    public static object GetFirstComponentInParents(GameObject origin, System.Type type)
    {
        int arbitraryCutoff = 1000;
        Transform parent = origin.transform.parent;
        int go = 0;
        while (go < arbitraryCutoff)
        {
            go++;
            if (parent.GetComponent(type))
            {
                return parent.GetComponent(type);
            }
            else if (parent.parent == null) return null;
            else parent = parent.parent;
        }

        return null;
    }

    public static void SaveToJson(object toSave, string path)
    {
        string jsonSave = JsonUtility.ToJson(toSave);
        System.IO.File.WriteAllText(path, jsonSave);
    }

    public static object GetFromJson(System.Type type, string path)
    {
        object obj = JsonUtility.FromJson(System.IO.File.ReadAllText(path), type);
        return obj;
    }

    public static string[] GetFiles(string path, string extentionWithJoker)
    {
        System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(path);
        string[] files = System.IO.Directory.GetFiles(path, extentionWithJoker);
        return files;
    }

    public static object[] GetFromJsons(System.Type type, string path)
    {
        string[] files = GetFiles(path, "*.json");
        object[] objs = new object[files.Length];

        for(int i = 0; i < files.Length; i++)
        {
            objs[i] = JsonUtility.FromJson(files[i], type);
        }

        return objs;
    }
    
    public static int RoundUpInt(float toRound)
    {
        float nearest = Mathf.Round(toRound);
        float f = toRound - nearest;

        int r = Mathf.RoundToInt(toRound - (f));
        return r;
    }

    /// <summary>
    /// Rounds it closer to Zero. -1.5 returns -1.
    /// </summary>
    public static int RoundDownInt(float toRound)
    {
        float remainder = GetDecimalPart(toRound);
        float f = toRound - remainder;
        return (int)f;
    }

    public static float GetDecimalPart(float f)
    {
        return f % 1;
    }

    public static List<object> GetComponentsInList(GameObject[] objects, System.Type type)
    {
        List<object> returnObjects = new List<object>();
        foreach(GameObject obj in objects)
        {
            try
            {
                if(obj.GetComponent(type))
                {
                    returnObjects.Add(obj.GetComponent(type));
                }
            } catch(System.Exception e) { Debug.LogError(e); }
        }
        return returnObjects;
    }

    public static void SetParentAll(MonoBehaviour[] objects, GameObject parent)
    {
        for(int i = 0; i < objects.Length; i++)
        {
            objects[i].transform.SetParent(parent.transform);
        }
    }

    public static float LowerBound(Camera cam)
    {
        return cam.transform.position.y - cam.orthographicSize;
    }

    public static Vector2 WorldToCanvasSpace(CanvasScaler canvasScaler, Vector2 wordPos)
    {
        Vector2 scaleReference = new Vector2(canvasScaler.referenceResolution.x / Screen.width, 
                                             canvasScaler.referenceResolution.y / Screen.height);

        Vector2 result = wordPos;
        result.Scale(scaleReference);
        return result;
    }

    /// <summary>
    /// Get the boundaries of an ortho camera in world units
    /// </summary>
    /// <param name="cam"></param>
    /// <returns></returns>
    public static Vector2 GetCameraSize(Camera cam)
    {
        float vertical = cam.orthographicSize * 2;
        float horizontal = vertical * Screen.width / Screen.height;
        return new Vector2(horizontal, vertical);
    }

    /// <summary>
    /// Get the height and width of a rect transform
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Vector2 GetRectDimensions(RectTransform rect)
    {
        float width = rect.sizeDelta.x * rect.localScale.x;
        float height = rect.sizeDelta.y * rect.localScale.y;
        return new Vector2(width, height);
    }

    /// <summary>
    /// Outputs the rect dimensions of a UI object, if you want it to be the same screen size as objectSize
    /// </summary>
    /// <param name="objectSize"></param>
    /// <param name="cam"></param>
    /// <param name="canvasRect"></param>
    /// <returns></returns>
    public static Vector2 WorldToCanvasSize(Vector2 objectSize, Camera cam, RectTransform canvasRect)
    {
        Vector2 cameraSize = GetCameraSize(cam);
        //The object compared to the camera render size
        Vector2 cameraScale = new Vector2( objectSize.x / cameraSize.x, objectSize.y / cameraSize.y);
        Vector2 rectSize = GetRectDimensions(canvasRect);
        //Get the same relative scale with the canvas
        Vector2 canvasSize = new Vector2(rectSize.x  * cameraScale.x, rectSize.y * cameraScale.y);

        return canvasSize;
    }


    /*public static Vector3 LocalToWorldSpace(GameObject obj)
    {
        GameObject gameObj = obj;
        int cutoff = 100;
        int count = 0;

        Vector3 worldSpace;
        while(gameObj.transform.parent != null)
        {
            count++;
            if (count > cutoff) break;
            worldSpace = obj.transform.parent.position + obj.localPO
        }
    }*/
}

[System.Serializable]
public class BrownianMotionData
{
    public BrownianMotionData(float Octaves, float Frequency, float Amplitude, float Gain, float Lacunarity)
    {
        octaves = Octaves;
        frequency = Frequency;
        amplitude = Amplitude;
        gain = Gain;
        lacunarity = Lacunarity;
    }
    public float octaves;
    public float frequency;
    public float amplitude;
    public float gain;
    public float lacunarity;
}