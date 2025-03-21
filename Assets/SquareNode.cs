using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Audio;
using Unity.Mathematics;
using Unity.Collections;

[BurstCompile(CompileSynchronously = true)]
public struct SquareNode : IAudioKernel<SquareNode.Parameters, SquareNode.Providers>
{
    public enum Parameters
    {
        [ParameterDefault(440f), ParameterRange(20f, 24000f)]
        Frequency,
        [ParameterDefault(0.5f), ParameterRange(0f, 1f)]
        Amplitude
        // add adsr
    }

    public enum Providers { }

    private float phase;

    public void Initialize() 
    {
        phase = 0f;
    }

    public void Execute(ref ExecuteContext<Parameters, Providers> context) 
    {
        SampleBuffer output = context.Outputs.GetSampleBuffer(0);
        SampleBuffer gateInput = context.Inputs.GetSampleBuffer(0);
        // SampleBuffer pitchInput = context.Inputs.GetSampleBuffer(1);

        int numChannels = output.Channels;
        int numSamples = output.Samples;
        int sampleRate = context.SampleRate;

        float frequency = context.Parameters.GetFloat(Parameters.Frequency, 0);
        float amplitude = context.Parameters.GetFloat(Parameters.Amplitude, 0);
        float phaseIncrement = 2f * math.PI * frequency / sampleRate;

        for (int s = 0; s < numSamples; s++)
        {
            float gateValue = gateInput.GetBuffer(0)[s];
            float value = (gateValue == 1f) ? amplitude * math.sign(math.sin(phase)) : 0f; // TEMPORARY!!!!!!

            phase += phaseIncrement;

            if (phase >= 2f * math.PI)
                phase -= 2f * math.PI;

            for (int c = 0; c < numChannels; c++)
            {
                NativeArray<float> outputBuffer = output.GetBuffer(c);
                outputBuffer[s] = value;
            }
        }
    }

     // precompute times for adsr
            // get gate input from buffer
            // if gate is on compute osc
            // if gate is off, set output to 0
            // write output to buffer

    public void Dispose() { }
}