using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using Unity.Audio;
using Unity.Collections;

public class DSPGraphManager : MonoBehaviour
{
    private DSPGraph dspGraph;
    private AudioDriver driver;
    private AudioOutputHandle outputHandle;
    private MixerNodeWrapper mixerNodeWrapper;

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

        GameObject mixerObj = new GameObject("Mixer");
        mixerNodeWrapper = mixerObj.AddComponent<MixerNodeWrapper>();
        mixerNodeWrapper.Initialize(this, channels);

        // TEST
        GameObject sineObj = new GameObject("Sine");
        OscNodeWrapper sineNodeWrapper = sineObj.AddComponent<OscNodeWrapper>();
        sineNodeWrapper.Initialize(this, channels);
        GameObject gateObj = new GameObject("Gate");
        GateNodeWrapper gateNodeWrapper = gateObj.AddComponent<GateNodeWrapper>();
        gateNodeWrapper.Initialize(this, channels);

        commandBlock.Complete();
    }
    
    void Update()
    {
        dspGraph.Update();
    }

    void OnDestroy() 
    {
        if (dspGraph.Valid) {
            outputHandle.Dispose();
            dspGraph.Dispose();
        }
    }

    public DSPGraph GetDSPGraph() => dspGraph;
}