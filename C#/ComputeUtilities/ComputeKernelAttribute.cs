using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class ComputeKernelAttribute : System.Attribute
{
    public string NameOverride { get; private set; }

    public ComputeKernelAttribute(string nameOverride = null)
    {
        NameOverride = nameOverride;
    }
}
