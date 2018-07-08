using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidChecker : MonoBehaviour {

    public static event Action<IObject> OnEnteredObject = delegate { };
    public static event Action<IObject> OnExitedObject = delegate { };

    private void OnTriggerEnter(Collider other)
    {
        OnEnteredObject(other.GetComponent<IObject>());
    }

    private void OnTriggerExit(Collider other)
    {
        OnExitedObject(other.GetComponent<IObject>());
    }
}
