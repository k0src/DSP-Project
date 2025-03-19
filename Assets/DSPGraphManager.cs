using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using Unity.Audio;
using Unity.Collections;

public class DSPGraphManager : MonoBehaviour
{
    [Range(20f, 20000f)]
    public float frequency = 440f;

    [Range(0f, 1f)]
    public float amplitude = 0.5f;

    private DSPGraph dspGraph;
    private AudioDriver driver;
    private AudioOutputHandle outputHandle;

    private DSPNode oscNode;
    private NativeArray<float> sampleFrameValues;

    void Awake() 
    {
        var format = ChannelEnumConverter.GetSoundFormatFromSpeakerMode(AudioSettings.speakerMode);
        var channels = ChannelEnumConverter.GetChannelCountFromSoundFormat(format);
        AudioSettings.GetDSPBufferSize(out var bufferLength, out var numBuffers);
        var sampleRate = AudioSettings.outputSampleRate;

        Debug.LogFormat("Format={2} Channels={3} BufferLength={0} SampleRate={1}", bufferLength, sampleRate, format, channels);

        dspGraph = DSPGraph.Create(format, channels, bufferLength, sampleRate);
        driver = new AudioDriver { dspGraph = dspGraph };
        outputHandle = driver.AttachToDefaultOutput();

        var commandBlock = dspGraph.CreateCommandBlock();
        oscNode = commandBlock.CreateDSPNode<OscNode.Parameters, OscNode.Providers, OscNode>();
        commandBlock.AddOutletPort(oscNode, channels);
        commandBlock.Connect(oscNode, 0, dspGraph.RootDSP, 0);
        commandBlock.Complete();
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        var commandBlock = dspGraph.CreateCommandBlock();
        commandBlock.SetFloat<OscNode.Parameters, OscNode.Providers, OscNode>(oscNode, OscNode.Parameters.Frequency, frequency);
        commandBlock.SetFloat<OscNode.Parameters, OscNode.Providers, OscNode>(oscNode, OscNode.Parameters.Amplitude, amplitude);
        commandBlock.Complete();

        dspGraph.Update();
    }

    void OnDestroy() 
    {
        if (dspGraph.Valid) {
            outputHandle.Dispose();
            dspGraph.Dispose();
        }
    }
}
