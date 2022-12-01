// Copyright 2022 Niantic, Inc. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Niantic.ARDK.AR.WayspotAnchors;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using Niantic.ARDK.Extensions;

namespace Niantic.ARDKExamples.WayspotAnchors
{
  public class WayspotAnchorDataUtility : MonoBehaviour
  {
    public Mesh roomMesh;
    public List<GameObject> placedObjects;
    public Material meshTransparent;

    private const string DataKey = "wayspot_anchor_payloads";
    private static WayspotAnchorDataUtility _instance;

    /// <summary>
    /// The singleton instance referring to the class
    /// </summary>
    public static WayspotAnchorDataUtility Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<WayspotAnchorDataUtility>();
            }

            return _instance;
        }
    }

    public void SavePayloads(WayspotAnchorPayload[] wayspotAnchorPayloads)
    {
        var wayspotAnchorsData = new WayspotAnchorsData();
        wayspotAnchorsData.Payloads = wayspotAnchorPayloads.Select(a => a.Serialize()).ToArray();
        string wayspotAnchorsJson = JsonUtility.ToJson(wayspotAnchorsData);

       // SaveToServer(wayspotAnchorsJson);
        SaveLocalPayloads(wayspotAnchorsJson);
    }

    public void SaveToServer(string wayspotAnchorsJson)
    {
        Debug.Log("Database : " + (Database.Instance != null));
        StartCoroutine(Database.Instance.SaveData(
            wayspotAnchorsJson,
            () => { Debug.Log("succeeded to save"); },
            () => { Debug.Log("failed to save"); }));
    }

    public void SaveLocalPayloads(string wayspotAnchorsJson)
    {
        PlayerPrefs.SetString(DataKey, wayspotAnchorsJson);
    }

    public WayspotAnchorPayload[] LoadPayloads()
    {
            var payloads = new List<WayspotAnchorPayload>();
            payloads.Add(WayspotAnchorPayload.Deserialize("ChUIxJ2k7dbdr6VHEIWvl9Pv3oXCxwEY2ZqPkI/W+wIqJgoUCLiO2oz1g8unIxCjybXD3bGCqm0SCQoAEgUlAACAPx0AAIA/"));

            GameObject roomObject = new GameObject();
            WayspotAnchorTracker tracker = roomObject.AddComponent(typeof(WayspotAnchorTracker)) as WayspotAnchorTracker;
            MeshRenderer meshRenderer = roomObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            MeshFilter meshFilter = roomObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
            meshFilter.mesh = roomMesh;
            meshFilter.sharedMesh = roomMesh;
            meshRenderer.material = meshTransparent;
            Debug.Log("Mesh set : " + roomMesh.name);

            foreach(GameObject placedObjectData in placedObjects)
            {
                GameObject placedObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                WayspotAnchorTracker placedTracker = placedObject.AddComponent(typeof(WayspotAnchorTracker)) as WayspotAnchorTracker;
                placedObject.transform.position = placedObjectData.transform.position;
                placedObject.transform.rotation = placedObjectData.transform.rotation;
                placedObject.transform.localScale = placedObjectData.transform.localScale;
            }

            return payloads.ToArray();
            /*
          if (PlayerPrefs.HasKey(DataKey))
          {
            var payloads = new List<WayspotAnchorPayload>();
            var json = PlayerPrefs.GetString(DataKey);
            var wayspotAnchorsData = JsonUtility.FromJson<WayspotAnchorsData>(json);
            foreach (var wayspotAnchorPayload in wayspotAnchorsData.Payloads)
            {
                  var payload = WayspotAnchorPayload.Deserialize(wayspotAnchorPayload);
                  payloads.Add(payload);
            }

            return payloads.ToArray();
          }
          else
          {
                var payloads = new List<WayspotAnchorPayload>();
                StartCoroutine(Database.Instance.GetFullData(
                    DataKey,
                    (data) => {
                        Debug.Log("succeeded to load data");
                        Debug.Log("succeeded to load data");
                        foreach (var wayspotAnchorPayload in data.Payloads)
                        {
                            var payload = WayspotAnchorPayload.Deserialize(wayspotAnchorPayload);
                            payloads.Add(payload);
                        }
                    },
                    () => {
                        Debug.Log("failed to save"); 
                    }));
                return payloads.ToArray();
           }
            */
        }

    public static void ClearLocalPayloads()
    {
      if (PlayerPrefs.HasKey(DataKey))
      {
        PlayerPrefs.DeleteKey(DataKey);
      }
    }

    [Serializable]
    public class WayspotAnchorsData
    {
      /// The payloads to save via JsonUtility
      public string[] Payloads = Array.Empty<string>();
    }
    }
}
