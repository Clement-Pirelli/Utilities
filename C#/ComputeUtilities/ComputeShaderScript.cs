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
    float simulationSpeed = 1.0f;
    protected int steps = 0;

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

    private void UpdateComputeVariables(UpdateFrequency frequencyToMatch) 
    {
        const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
        foreach (var field in GetType().GetFields(bindingFlags)) 
        {
            var attributes = (ComputeVariableAttribute[]) field.GetCustomAttributes(typeof(ComputeVariableAttribute), false);
            foreach (ComputeVariableAttribute attribute in attributes)
            {
                if (attribute.Frequency.HasFlag(frequencyToMatch))
                {
                    var value = field.GetValue(this);
                    var name = attribute.VariableName ?? field.Name;
                    var kernel = attribute.KernelName is null ? 0 : computeShader.FindKernel(attribute.KernelName);
                    UpdateComputeVariable(value, name, kernel);
                }
            }
        }
    }

    private void UpdateComputeVariable(object value, string name, int kernel) 
    {
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
            default:
                Debug.LogError($"ComputeVariable of name {name} has an unsupported type! Type was {value.GetType()}");
                break;
        }
    }
}
