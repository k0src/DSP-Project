using Unity.Burst;
using Unity.Audio;
using Unity.Collections;

[BurstCompile(CompileSynchronously = true)]
public struct AudioDriver : IAudioOutput 
{
    public DSPGraph dspGraph;
    int m_ChannelCount;
    private bool m_FirstMix;

    public void Initialize(int channelCount, SoundFormat format, int sampleRate, long dspBufferSize)
    {
        m_ChannelCount = channelCount;
        m_FirstMix = true;
    }

    public void BeginMix(int frameCount)
    {
        if (!m_FirstMix)
            return;
        m_FirstMix = false;
        dspGraph.OutputMixer.BeginMix(frameCount, DSPGraph.ExecutionMode.Jobified | DSPGraph.ExecutionMode.ExecuteNodesWithNoOutputs);
    }

    public void EndMix(NativeArray<float> output, int frames)
    {
        dspGraph.OutputMixer.ReadMix(output, frames, m_ChannelCount);
        dspGraph.OutputMixer.BeginMix(frames);
    }

    public void Dispose()
    {
        dspGraph.Dispose();
    }
}