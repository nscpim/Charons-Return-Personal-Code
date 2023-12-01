using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class WaveManager : NetworkBehaviour
{
    //The Spawnpoints the zombies can spawn on
    public Transform[] spawnPoints;
    //Time for the round to change
    public int roundChangeTime;
    //Timer for the round
    public Timer roundTimer;
    //Timer for the zombies to spawn, so they dont spawn all at once.
    public Timer spawnTimer;
    //Current Round
    [SyncVar]
    public int roundCount;
    //Current amount of zombies to kill
    [SyncVar]
    public int currentZombieCount;
    //Total amound of zombies in a round
    [SyncVar]
    public float roundZombieCount;

    //Array with all the zombies
    private GameObject[] zombieIndex;

    //maxHealth from the zombies
    [SyncVar]
    public float maxHealth;

    //counter towards the tank zombie
    [SyncVar]
    public int zombiesSpawnedCounter;

    //The amount of zombies that have spawned in a round
    public int spawnedZombiesCount;

    /// <summary>
    /// Initalize the GameObjects here, with Networking you can't put it in the Constructor. Must be in Start or Awake
    /// </summary>
    private void Start()
    {
        roundTimer = new Timer();
        spawnTimer = new Timer();
        zombieIndex = null;

        //Load the zombies from the Resources folder
        GameObject zombie = Resources.Load<GameObject>("Prefabs/Actors/NetworkedZombie");
        GameObject tankZombie = Resources.Load<GameObject>("Prefabs/Actors/StrongerNetworkedZombie");
        GameObject spitterZombie = Resources.Load<GameObject>("Prefabs/Actors/SpitterZombie");

        //Initalize zombieIndex Array with the Prefabs loaded in
        zombieIndex = new GameObject[] { tankZombie, zombie, spitterZombie };

        //The Time it takes for the new round to spawn
        roundChangeTime = 10;

        //Sets the timer for the first round
        roundTimer.SetTimer(roundChangeTime);

        spawnedZombiesCount = 0;
    }
    /// <summary>
    /// Changes the round when there are no more zombies left
    /// </summary>
    public void ChangeWave()
    {
        //print("ChangeWave");
        roundChangeTime = 10;
        roundTimer.SetTimer(roundChangeTime);
        if (roundCount > 0)
        {
            ChangeRoundZombieCount();
        }
        //Round the roundZombieCount to an int so you get the right amount of zombies.
        currentZombieCount = Mathf.RoundToInt(roundZombieCount);
        roundCount++;
        spawnedZombiesCount = 0;
    }
    /// <summary>
    /// Spawns the amount of zombies equal to the total amount of the current round
    /// </summary>
    public void Spawning()
    {
        //Local Variable to get the zombies from the zombieIndex Array
        GameObject tankZombie = zombieIndex[0];
        GameObject defaultZombie = zombieIndex[1];
        GameObject spitterZombie = zombieIndex[2];

        if (currentZombieCount > 0 && !roundTimer.IsActive && !spawnTimer.IsActive && spawnTimer.TimerDone() && spawnedZombiesCount <= roundZombieCount)
        {
            //print("Zombie count higher then 0");
            //For every 5 normal zombies it spawns a tank zombie, this counts towards the roundZombieCount
            if (zombiesSpawnedCounter == 5)
            {
                spawnTimer.SetTimer(1f);
                SpawnZombie(tankZombie);
                zombiesSpawnedCounter = 0;
                //print(Time.time + "Spawned Tank Zombie");
            }
            else
            {
                GameObject spawnThisZombie = null;
                if (SceneManager.GetActiveScene().name == "Level 3")
                {
                    spawnThisZombie = zombieIndex[UnityEngine.Random.Range(1, 3)];
                }
                else
                {
                    spawnThisZombie = defaultZombie;
                }
                spawnTimer.SetTimer(1f);
                SpawnZombie(spawnThisZombie);
                //print(Time.time + "Spawned Normal Zombie");
                zombiesSpawnedCounter++;
            }
        }
        else if (currentZombieCount <= 0 && !roundTimer.IsActive)
        {
            //print("No Zombies left , calling ChangeWave()");
            ChangeWave();
        }
        else if (currentZombieCount <= 0 && roundTimer.IsActive)
        {
            //print("Timer is still in progress, can't change wave or spawn zombies");
        }
    }
    /// <summary>
    /// Spawns 1 zombie in.
    /// </summary>
    /// <param name="zombie"></param>
    public void SpawnZombie(GameObject zombie)
    {
        if (spawnPoints.Length > 0)
        {
            int i = UnityEngine.Random.Range(0, spawnPoints.Length);
            if (isServer)
            {
                NetworkServer.Spawn(Instantiate(zombie, spawnPoints[i].position, Quaternion.identity) as GameObject);
                spawnedZombiesCount += 1;
            }
        }
    }
    /// <summary>
    ///  Checks if the Timer is done and if its done stops the timer.
    /// </summary>
    public void Update()
    {
        //  print(roundTimer.TimeLeft() + " Time left");

        //print(currentZombieCount + " Zombies left");
        //print(zombiesSpawnedCounter + " spawnedCounter");

        if (SceneManager.GetActiveScene().name != "NetworkedLobby" && isServer)
        {
            if (roundTimer.IsActive && roundTimer.TimerDone())
            {
                roundTimer.StopTimer();
            }
            if (spawnTimer.TimerDone() && spawnTimer.IsActive)
            {
                spawnTimer.StopTimer();
            }
            Spawning();
        }

        //   print("Timer Done");
        //    print("Spawn Timer Done");

        //  print(spawnTimer.TimeLeft() + "Spawn Timer");


        //   print(currentZombieCount + " CurrentZombieCount");
        //  print(roundZombieCount + " RoundZombieCount");
        //   print(roundCount + "Round");


    }
    /// <summary>
    /// Changes the number of zombies and the health in each wave 
    /// </summary>
    public void ChangeRoundZombieCount()
    {
        //There will be some zombies extra and ramp up with every round
        roundZombieCount = (roundZombieCount * 10f) / 7.5f;
        ChangeHealth();

    }
    /// <summary>
    /// Call this when a zombie dies
    /// </summary>
    public void ChangeCurrentZombieCount()
    {
        currentZombieCount -= 1;
    }

    /// <summary>
    /// Enables a spawnpoint
    /// </summary>
    /// <param name="spawnPoint"></param>
    public void EnableRegion(GameObject spawnPoint)
    {
        spawnPoint.gameObject.SetActive(true);
    }
    /// <summary>
    /// Disables a Spawnpoint
    /// </summary>
    /// <param name="spawnPoint"></param>
    public void DisableRegion(GameObject spawnPoint)
    {
        spawnPoint.gameObject.SetActive(false);
    }

    /// <summary>
    /// Changes the health of the zombies
    /// </summary>
    /// <param name="amount"></param>
    public void ChangeHealth()
    {
        if (isServer)
        {
            if (GameManager.instance.waveManager.roundCount <= 10)
            {
                maxHealth = maxHealth += 10;
            }
            else if (GameManager.instance.waveManager.roundCount >= 10)
            {
                maxHealth = (maxHealth * 10) / 8.5f;
            }
        }
    }
}
