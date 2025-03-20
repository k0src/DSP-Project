using Unity.Burst;
using Unity.Audio;
using Unity.Collections;

[BurstCompile(CompileSynchronously = true)]
public struct MixerNode : IAudioKernel<MixerNode.Parameters, MixerNode.Providers>
{
    public enum Parameters { Gain }
    public enum Providers { }

    public void Initialize() { }

    public void Execute(ref ExecuteContext<Parameters, Providers> context)
    {
        SampleBuffer output = context.Outputs.GetSampleBuffer(0);
        float gain = context.Parameters.GetFloat(Parameters.Gain, 0);

        int numChannels = output.Channels;
        int numSamples = output.Samples;
        int numInputs = context.Inputs.Count;

        for (int s = 0; s < numSamples; s++)
        {
            for (int c = 0; c < numChannels; c++)
            {
                float sum = 0f;
                for (int i = 0; i < numInputs; i++)
                {
                    SampleBuffer input = context.Inputs.GetSampleBuffer(i);
                    NativeArray<float> inputBuffer = input.GetBuffer(c);
                    sum += inputBuffer[s];
                }
                NativeArray<float> outputBuffer = output.GetBuffer(c);
                outputBuffer[s] = sum * gain;
            }
        }
    }

    public void Dispose() { }
}