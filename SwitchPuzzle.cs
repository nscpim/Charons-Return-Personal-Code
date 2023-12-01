using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SwitchPuzzle : NetworkBehaviour
{
    [Tooltip("All the possibl switches")]
    public GameObject[] switches;

    //Boolean to check if all the switches have been pulled, public for testing
    public bool projectorAcceptVideoParts;

   /// <summary>
   /// check if all the switches have been pulled
   /// </summary>
    private void Update()
    {
          if (switches[0].GetComponent<SwitchNetwork>().GetBool() && switches[1].GetComponent<SwitchNetwork>().GetBool() && switches[2].GetComponent<SwitchNetwork>().GetBool())
          {
             
              projectorAcceptVideoParts = true;
          }
    }

    /// <summary>
    /// returns projectorAcceptVideoParts boolean
    /// </summary>
    /// <returns></returns>
    public bool GetProjectorBool()
    {
        return projectorAcceptVideoParts;
    }

}
