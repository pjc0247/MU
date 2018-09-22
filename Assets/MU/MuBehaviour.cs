using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuBehaviour : MonoBehaviour
{
    protected virtual void Awake()
    {
        MU.AddBehaviour(this);
    }
    protected virtual void OnDestroy()
    {
        MU.RemoveBehaviour(this);
    }
}
