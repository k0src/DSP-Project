using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Audio;
using Unity.Mathematics;
using Unity.Collections;
 
[BurstCompile(CompileSynchronously = true)]
public struct GateNode : IAudioKernel<GateNode.Parameters, GateNode.Providers>
{
    public enum Parameters
    {
        [ParameterDefault(0.5f), ParameterRange(0f, 1f)]
        Speed
    }
 
    public enum Providers { }
 
    private int counter;
    
    public void Initialize() 
    {
    }

    public void Execute(ref ExecuteContext<Parameters, Providers> context) 
    {
        SampleBuffer output = context.Outputs.GetSampleBuffer(0);
        int numChannels = output.Channels;
        int numSamples = output.Samples;
        int sampleRate = context.SampleRate;
 
        float period = context.Parameters.GetFloat(Parameters.Speed, 0);
        int n = (int)(sampleRate * period);
 
        for (int s = 0; s < numSamples; s++)
        {
            for (int c = 0; c < numChannels; c++)
            {
                float value = (counter / n) % 2 == 0 ? 1f : 0f;
                counter = (counter + 1) % (2 * n);
 
                NativeArray<float> outputBuffer = output.GetBuffer(c);
                outputBuffer[s] = value;
            }
        }
    }
 
    public void Dispose() { }
}