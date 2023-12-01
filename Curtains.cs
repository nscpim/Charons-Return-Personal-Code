using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtains : MonoBehaviour
{
    //All the curtains
    [Tooltip("All cinema curtains")]
    public GameObject[] curtains;

    /// <summary>
    /// if the switch has been activated the curtains will dissappear or reappear when the switch is deactivated
    /// </summary>
    public void Update()
    {
        if (GetComponent<SwitchNetwork>().GetBool())
        {
            foreach (GameObject item in curtains)
            {
                item.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject item in curtains)
            {
                item.SetActive(true);
            }
        }
    }
}
