using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
 * Africa. And likewise, other birds and areal creatures (flying worms and such)
 * as he flies over the sea and enters Europe. 
 * 
 * In order to reach his destination, Coco must avoid running into the birds and
 * other objects in the sky that he encounters, unless of course they're worms.
 * If he hits an obstacles, he'll journey will be cut short and he'll have to 
 * start over. But in order to continue with his journey even if he doesn't run 
 * into any obstacles, he still needs to eat and rest. But he can't land. That 
 * means he has to somehow fly five thousand miles without landing. Thankfully,
 * there are flying worms he can feed off of and occasional hot air balloons
 * and other objects where he can take a respite for a while. Accomplishing any 
 * of those would increase his energy and he'll have a better chance of getting 
 * to Europe.
 * 
 * The game is designed to be fair. There won't be a situation where Coco is low
 * on energy but there is no place to rest or no food nearby. Although the 
 * appearance of food and rest stops are random, they are tied to how much he has 
 * left. If he's full, it' less likely that he'll find food or a rest stop. If 
 * he's hungry, they'll be within reach. 
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

    void Update()
    {
        // TODO: avoid obstacles and rewards from spawning on top of each other
        if (game.GameOver) { return; }
        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
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
        pos.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);

        switch (Prefab.name) {
            case "Cactus":
                pos.x = Random.Range(xSpawnRange.maxX, xSpawnRange.maxX);
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
        pos.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);
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