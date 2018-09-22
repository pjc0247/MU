using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribeAttribute : Attribute
{
    public string name;

    public SubscribeAttribute(string name)
    {
        this.name = name;
    }
}
