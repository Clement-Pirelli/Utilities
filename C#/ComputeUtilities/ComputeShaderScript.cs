using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public abstract class ComputeShaderScript : MonoBehaviour
{
    [SerializeField]
    protected ComputeShader computeShader;

    [SerializeField]
    protected Material outMaterial;

    [SerializeField, Range(.0f, 2.0f)]
    protected float simulationSpeed = 1.0f;
    protected int steps = 0;

    private Dictionary<string, int> namesToKernels = new Dictionary<string, int>();
    protected int GetKernel(string name) 
    {
        int kernel;
        return namesToKernels.TryGetValue(name, out kernel) ? kernel : computeShader.FindKernel(name);
    }

    void Start()
    {
        UnityEngine.Random.InitState(DateTime.Now.Second);
        Restart();
    }

    float accTime = .0f;
    void Update()
    {
        accTime += Time.deltaTime * simulationSpeed;
        if (accTime > 1.0f / 60.0f)
        {
            steps++;
            UpdateComputeVariables(UpdateFrequency.OnStep);
            Step();
            accTime = .0f;
        }
        UpdateComputeVariables(UpdateFrequency.OnRender);
        Render();
    }

    [NaughtyAttributes.Button]
    public void Restart()
    {
        GetKernels();
        SetupResources();
        UpdateComputeVariables(UpdateFrequency.OnStart);
        ResetState();
    }

    protected abstract void SetupResources();

    protected abstract void ResetState();
    protected abstract void Step();
    protected virtual void Render() { }
    protected virtual void ReleaseResources() { }

    private void OnDestroy() => ReleaseResources();
    private void OnEnable() => ReleaseResources();
    private void OnDisable() => ReleaseResources();


    const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
    private void UpdateComputeVariables(UpdateFrequency frequencyToMatch) 
    {
        foreach (var field in GetType().GetFields(bindingFlags)) 
        {
            var attributes = (ComputeVariableAttribute[]) field.GetCustomAttributes(typeof(ComputeVariableAttribute), false);
            foreach (ComputeVariableAttribute attribute in attributes)
            {
                if (attribute.Frequency.HasFlag(frequencyToMatch))
                {
                    var value = field.GetValue(this);
                    var name = attribute.VariableName ?? field.Name;
                    var kernel = attribute.KernelName is null ? 0 : GetKernel(attribute.KernelName);
                    UpdateComputeVariable(value, name, kernel);
                }
            }
        }
    }

    private void UpdateComputeVariable(object value, string name, int kernel) 
    {
        //for enums, extract their underlying value and type
        if (value.GetType().IsEnum)
        {
            Type underlying = value.GetType().GetEnumUnderlyingType();
            value = Convert.ChangeType(value, underlying);
        }

        switch (value)
        {
            case bool b:
                computeShader.SetInt(name, b ? 1 : 0);
                break;
            case int i:
                computeShader.SetInt(name, i);
                break;
            case float f:
                computeShader.SetFloat(name, f);
                break;
            case Vector3 v3:
                computeShader.SetVector(name, v3.asDirection());
                break;
            case Vector4 v4:
                computeShader.SetVector(name, v4);
                break;
            case Color c:
                computeShader.SetVector(name, c.asVector4());
                break;
            case Texture2D t2:
                computeShader.SetTexture(kernel, name, t2);
                break;
            case Texture3D t3:
                computeShader.SetTexture(kernel, name, t3);
                break;
            case RenderTexture t:
                computeShader.SetTexture(kernel, name, t);
                break;
            case ComputeBuffer cb:
                computeShader.SetBuffer(kernel, name, cb);
                break;
            case AnimationCurve am:
                computeShader.SetFloat(name, am.Evaluate(Time.time));
                break;
            default:
                Debug.LogError($"ComputeVariable of name {name} has an unsupported type! Type was {value?.GetType()}");
                break;
        }
    }

    private void GetKernels()
    {
        namesToKernels.Clear();

        foreach (var field in GetType().GetFields(bindingFlags))
        {
            var attribute = field.GetCustomAttribute<ComputeKernelAttribute>();
            if(attribute != null)
            {
                var name = attribute.NameOverride ?? field.Name;
                var value = field.GetValue(this);
                if (value.GetType() != typeof(int))
                {
                    Debug.LogError($"ComputeKernel attribute can only be used on ints! {field} is not of int type!");
                    continue;
                }

                int kernel = computeShader.FindKernel(name);
                namesToKernels.Add(name, kernel);
                field.SetValue(this, kernel);
            }
        }
    }
}
