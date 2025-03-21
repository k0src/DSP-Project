using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Audio;
using Unity.Mathematics;
using Unity.Collections;

[BurstCompile(CompileSynchronously = true)]
public struct PitchNode : IAudioKernel<PitchNode.Parameters, PitchNode.Providers>
{
    public enum Parameters
    {
        [ParameterDefault(0f), ParameterRange(0f, 24000f)] // Start at 0, 0 = off
        Frequency // not a param ,delete later
        // [ParameterDefault(0.5f), ParameterRange(0f, 1f)]
        // Speed,
        // [ParameterDefault(0f), ParameterRange(0f, 1f)]
        // Toggle,
        // [ParameterDefault(0f), ParameterRange(0f, 1f)]
        // Variation,
        // [ParameterDefault(0f), ParameterRange(0f, 24000f)]
        // Range
        // [ParameterDefault(65f), ParameterRange(20f, 5000f)]
        // MinFrequency,
        // [ParameterDefault(2000f), ParameterRange(20f, 5000f)]
        // MaxFrequency

    }

    public enum Providers { }

    public void Initialize() 
    {
        random = new Unity.Mathematics.Random(1234);
        counter = 0;
        currentFrequency = 440f;
        targetFrequency = 440f;
    }

    private int counter;
    private Unity.Mathematics.Random random;
    private float currentFrequency;
    private float targetFrequency;

    public void Execute(ref ExecuteContext<Parameters, Providers> context) 
    {
        SampleBuffer output = context.Outputs.GetSampleBuffer(0);

        int numChannels = output.Channels;
        int numSamples = output.Samples;
        int sampleRate = context.SampleRate;

        float period = 0.5f;
        float maxFreq = 2000f;
        float minFreq = 65f;

        int samplesPerPeriod = (int)(sampleRate * period);
        float smoothingFactor = 0.01f;

        float frequency = context.Parameters.GetFloat(Parameters.Frequency, 0); // not a param ,delete later
        int n = (int)(sampleRate * period);

        for (int s = 0; s < numSamples; s++)
        {
            if (counter % samplesPerPeriod == 0)
            {
                targetFrequency = random.NextFloat(minFreq, maxFreq); // chnage this later
            }

            counter = (counter + 1) % (2 * n);

            currentFrequency = currentFrequency + (targetFrequency - currentFrequency) * smoothingFactor;

            for (int c = 0; c < numChannels; c++)
            {
                NativeArray<float> outputBuffer = output.GetBuffer(c);
                outputBuffer[s] = currentFrequency;
            }
        }
    }

    public void Dispose() { }
}