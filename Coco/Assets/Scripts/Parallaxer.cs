using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//TODO: the shift speed of all prefabs should speed up or slow down uniformly
// TODO: if a reward zone encountered and reset back to its original once the reward is over
// TODO: how do we fix the height so that the bird cannot go outside the screen
// TODO: spawn the objects at a spefic time without conflicting their arrival; 
//      that way, no need to speficy spawn rate
/*
 * Coco's Odyssey is a game that recounts the journey of an African swallow named
 * Coco travelling from the Gold Cost to Great Britain (modern day Ghana to UK). 
 * Coco is carrying two coconuts at the request of King Arthur, who desperately
 * needs the coconuts so he can imitate the sound of horse hoofs by hitting them
 * againt one another on his campaign to recruit the Knights of the Round Table.
 * Horses are hard to come by in the wintry isles of Britain.
 * 
 * Along his journey, Coco encounters birds endemic to the region he's flying 
 * through. He comes across birds that are common to Africa as he flies over 
 * Africa. And likewise, other birds and areal objects common to the European
 * airspace.
 * 
 * In order to reach his destination, Coco must avoid running into the birds and
 * obstacles in the sky that he encounters, unless of course they're insects.
 * If he hits an obstacle, his journey will be cut short and he'll have to 
 * start over. But in order to continue with his journey even if he doesn't run 
 * into any obstacles, he still needs to eat. Thankfully,
 * there are flying insects that he can feed off of.
 * 
 * The appearance of obstacles is time-based and changes every five minutes as 
 * follows:
 *  0-5:    Eagle
 *  5-10:   Albatross
 *  10-15:  Hot Air Balloons
 *  15-20:  Paratroopers
 *  20-25:  Poop from Plane
 *  25-30:  Landmarks (European landmarks, e.g. Eiffel, London Clock Tower, etc.)
 * 
 *  Although the obstacles will show up according ot the timeline, food will 
 *  appear randomly mulitple times in any given 5-minute window. In order to 
 *  continue flying, the bird must eat at least once in that window. 
 * 
 *  Near the end of the game, a castle will be shown at a distance to indicate 
 *  that the bird has reached its destination. 
 * 
 * 
 * # TODO: add more explanation/game logic details 
*/
public class Parallaxer : MonoBehaviour
{

    class PoolObject
    {
        public Transform transform;
        public bool inUse;
        public PoolObject(Transform t) { transform = t; }
        public void Use() { inUse = true; }
        public void Dispose() { inUse = false; }
    }

 
    [System.Serializable]
    public struct YSpawnRange
    {
        public float minY;
        public float maxY;
    }

    [System.Serializable]
    public struct XSpawnRange
    {
        public float minX;
        public float maxX;
    }

    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;

    public YSpawnRange ySpawnRange;
    public XSpawnRange xSpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate;
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    PoolObject[] poolObjects;
    float targetAspect;
    GameManager game;
    private int SECONDS = 60;
    private int timeNow;

  

    void Awake()
    {
        Configure();
    }

    void Start()
    {
        game = GameManager.Instance;
    }

    void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }
        Configure();
    }

    /*
     * Spawn new objects according to the timeline indicated above in the comments.
    */
    void Update()
    {
        // TODO: avoid obstacles and rewards from spawning on top of each other
        if (game.GameOver) { return; }
        Shift();
        if (constraintSatisfied()) {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnRate)
            {
                Spawn();
                spawnTimer = 0;
            }

        }

       
    }

        bool constraintSatisfied() {
        timeNow = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        int elapsed = timeNow - game.startTime;
       
       
        if (Prefab.name == "Eagle" && elapsed > 0 && elapsed < 10 ||
            Prefab.name == "Albatross" && elapsed > 10  && elapsed < 20 ||
            Prefab.name == "Balloon" && elapsed > 6 && elapsed < 9 ||
            Prefab.name == "Paratrooper" && elapsed > 9 && elapsed < 12 ||
            Prefab.name == "Poop" && elapsed > 12 && elapsed < 15 ||
            Prefab.name == "Landmark" && elapsed > 15 && elapsed < 18 ||
            Prefab.name == "Cactus" && elapsed > 0 && elapsed < 9) {
            return true;
        }
        return false;



    }

    void Configure()
    {
        //spawning pool objects
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            Prefab.SetActive(true);
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Spawn()
    {
        //moving pool objects into place
        Transform t = GetPoolObject();
        if (t == null) {  return; }
        t.position = GetPosition();
    }

    /*
     * Return the a Vector3 position object to transform the current prefab. 
     * Generate the new position based on the following constraints: the cactus
     * must be fixed to the ground. Obstacles can appear anywhere but must appear
     * above a cactus. Same with rewards.
     */
    private Vector3 GetPosition() {

        Vector3 pos = Vector3.zero;

        // default position for dead zones and reward zones
        pos.x = defaultSpawnPos.x;
        pos.y = UnityEngine.Random.Range(ySpawnRange.minY, ySpawnRange.maxY);

        switch (Prefab.name) {
            case "Cactus":
                pos.x = UnityEngine.Random.Range(xSpawnRange.maxX, xSpawnRange.maxX);
                pos.y = defaultSpawnPos.y;
                break;
        }
        return pos;
    } 

    void SpawnImmediate()
    {
        Transform t = GetPoolObject();
        if (t == null) return;
        Vector3 pos = Vector3.zero;
        pos.y = UnityEngine.Random.Range(ySpawnRange.minY, ySpawnRange.maxY);
        pos.x = (immediateSpawnPos.x * Camera.main.aspect) / targetAspect;
        t.position = pos;
        Spawn();
    }

    void Shift()
    {
        //loop through pool objects 
        //moving them
        //discarding them as they go off screen
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.position += Vector3.left * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }

    void CheckDisposeObject(PoolObject poolObject)
    {
        //place objects off screen
        if (poolObject.transform.position.x < xSpawnRange.minX*2)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject()
    {
        //retrieving first available pool object
        for (int i = 0; i < poolObjects.Length; i++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }

}