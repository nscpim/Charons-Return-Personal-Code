using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PressurePlate : NetworkBehaviour
{
    //syncvar so it syncs if someone stands on the pressureplate so it is the same for everyone (Only public due to testing)
    [SyncVar]
    public bool pressurePlateEnabled;

    /// <summary>
    /// Check if the players is within the collider range and calls a command to set the pressureplate bool on the server.
    /// </summary>
    /// <param name="collEnter"></param>
    public void OnTriggerEnter(Collider collEnter)
    {
        if (collEnter.name == "Body")
        {
            GameManager.instance.localPlayer.GetComponent<InteractionsLevel2>().CmdSetPressurePlateBool(gameObject, true);
            print("Pressure Plate: " + gameObject.name);
        }
    }
    /// <summary>
    /// Check if the player exits the collider range and calls a command for disabling the pressureplate bool on the server.
    /// </summary>
    /// <param name="collExit"></param>
    public void OnTriggerExit(Collider collExit)
    {
        if (collExit.name == "Body")
        {
            GameManager.instance.localPlayer.GetComponent<InteractionsLevel2>().CmdSetPressurePlateBool(gameObject, false);
        }
    }

    /// <summary>
    /// returns pressureplate bool
    /// </summary>
    /// <returns></returns>
    public bool GetBool()
    {
        return pressurePlateEnabled;
    }
}
