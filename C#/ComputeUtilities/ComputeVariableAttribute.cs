using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpdateFrequency
{
    OnStart,
    OnStep,
    OnRender,
    All = OnStart | OnStep | OnRender
}

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
public class ComputeVariableAttribute : System.Attribute
{
    public string VariableName { get; private set; }
    public string KernelName { get; private set; }
    public UpdateFrequency Frequency { get; private set; }

    public ComputeVariableAttribute(string kernelName = null, UpdateFrequency frequency = UpdateFrequency.OnStep, string variableName = null) 
    {
        KernelName = kernelName;
        Frequency = frequency;
        VariableName = variableName;
    }
}
