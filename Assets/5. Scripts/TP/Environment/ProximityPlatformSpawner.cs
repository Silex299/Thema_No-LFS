using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Video;

public class ProximityPlatformSpawner : SerializedMonoBehaviour
{


    public Transform target;
    public float posMultiplier;
    public float height;
    public float heightNoise;
    public float disablePlatformDistance;
    public GameObject platform;
    public int platformsCount = 10;

    [SerializeField] private Dictionary<string, GameObject> spawnedPlatforms = new Dictionary<string, GameObject>();
    [SerializeField] private Queue<GameObject> platforms = new Queue<GameObject>();



    private void Update()
    {

        Vector3 targetPostion = target.position;


        float x = targetPostion.x;
        float z = targetPostion.z;


        x = (float)Math.Truncate(x / posMultiplier);
        z = (float)Math.Truncate(z / posMultiplier);


        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {

                string key = ((i + x) * posMultiplier) + "," + ((j + z) * posMultiplier);



                if (!spawnedPlatforms.ContainsKey(key))
                {
                    GameObject obj = null;

                    //With queue
                    if (platforms.Count > 0)
                    {
                        obj = platforms.Dequeue();
                        obj.transform.position = new Vector3((i + x) * posMultiplier, height + UnityEngine.Random.Range(-heightNoise, heightNoise), (j + z) * posMultiplier);
                        StartCoroutine(SpawnPlatform(obj));
                    }
                    else
                    {
                        if (platformsCount > 0)
                        {
                            obj = Instantiate(platform, new Vector3((i + x) * posMultiplier, height + UnityEngine.Random.Range(-heightNoise, heightNoise), (j + z) * posMultiplier), Quaternion.identity, transform);
                            StartCoroutine(SpawnPlatform(obj));
                            platformsCount--;
                        }
                        else
                        {

                        }
                    }

                    //Without Queue
                    //obj = Instantiate(platform, new Vector3((i + x) * posMultiplier, height, (j + z) * posMultiplier), Quaternion.identity, transform);

                    if (obj != null)
                    {
                        spawnedPlatforms.Add(key, obj);
                        StartCoroutine(DestoryPlatform(key, obj));
                    }

                }
            }

        }



    }

    private IEnumerator DestoryPlatform(string key, GameObject obj)
    {
        yield return new WaitForSeconds(0.3f);

        if (Vector3.Distance(target.position, obj.transform.position) > disablePlatformDistance)
        {
            //Destroy(obj);
            obj.SetActive(false);
            spawnedPlatforms.Remove(key);
            platforms.Enqueue(obj);
        
        }
        else
        {
            StartCoroutine(DestoryPlatform(key, obj));
        }

    }


    private IEnumerator SpawnPlatform(GameObject obj)
    {
        float timeElapsed = 0;

        obj.SetActive(true);
        obj.transform.localScale = Vector3.zero;

        while (timeElapsed < 0.2f)
        {
            timeElapsed += Time.deltaTime;

            obj.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(posMultiplier, 0.1f, posMultiplier), timeElapsed / 0.2f);

            yield return null;
        }
    }

    private IEnumerator DisablePlatform(GameObject obj, string key)
    {
        float timeElapsed = 0;

        obj.SetActive(true);

        Vector3 initialScale = obj.transform.localScale;

        while (timeElapsed < 0.1f)
        {
            timeElapsed += Time.deltaTime;

            obj.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, timeElapsed / 0.1f);

            yield return null;
        }

        obj.SetActive(false);
    }




}
