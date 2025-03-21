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

    // Node prefabs
    [SerializeField]
    public GameObject sineOscPrefab;
    [SerializeField]
    public GameObject squareOscPrefab;
    [SerializeField]
    public GameObject triangleOscPrefab;
    [SerializeField]
    public GameObject sawtoothOscPrefab;
    [SerializeField]
    public GameObject gatePrefab;
    [SerializeField]
    public GameObject pitchPrefab;
    [SerializeField]
    public GameObject mixerPrefrab;

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

        // Create the Mixer Node
        Vector3 mixerPosition = new Vector3(0, 0, 0);
        GameObject mixerObj = Instantiate(mixerPrefrab, mixerPosition, Quaternion.identity);
        mixerObj.GetComponent<MixerNodeWrapper>().Initialize(this, channels);
        commandBlock.Complete();
    }

    public void CreateSineOscillatorNode(Vector3 position)
    {
        var format = ChannelEnumConverter.GetSoundFormatFromSpeakerMode(AudioSettings.speakerMode);
        var channels = ChannelEnumConverter.GetChannelCountFromSoundFormat(format);

        GameObject sineObj = Instantiate(sineOscPrefab, position, Quaternion.identity);
        sineObj.GetComponent<OscNodeWrapper>().Initialize(this, channels);
    }

    public void CreateSquareOscillatorNode(Vector3 position)
    {
        var format = ChannelEnumConverter.GetSoundFormatFromSpeakerMode(AudioSettings.speakerMode);
        var channels = ChannelEnumConverter.GetChannelCountFromSoundFormat(format);

        GameObject squareObj = Instantiate(squareOscPrefab, position, Quaternion.identity);
        squareObj.GetComponent<OscNodeWrapper>().Initialize(this, channels);
    }

    public void CreateTriangleOscillatorNode(Vector3 position)
    {
        var format = ChannelEnumConverter.GetSoundFormatFromSpeakerMode(AudioSettings.speakerMode);
        var channels = ChannelEnumConverter.GetChannelCountFromSoundFormat(format);

        GameObject triangleObj = Instantiate(triangleOscPrefab, position, Quaternion.identity);
        triangleObj.GetComponent<OscNodeWrapper>().Initialize(this, channels);
    }

    public void CreateSawtoothOscillatorNode(Vector3 position)
    {
        var format = ChannelEnumConverter.GetSoundFormatFromSpeakerMode(AudioSettings.speakerMode);
        var channels = ChannelEnumConverter.GetChannelCountFromSoundFormat(format);

        GameObject sawtoothObj = Instantiate(sawtoothOscPrefab, position, Quaternion.identity);
        sawtoothObj.GetComponent<OscNodeWrapper>().Initialize(this, channels);
    }

    public void CreateGateNode(Vector3 position)
    {
        var format = ChannelEnumConverter.GetSoundFormatFromSpeakerMode(AudioSettings.speakerMode);
        var channels = ChannelEnumConverter.GetChannelCountFromSoundFormat(format);
        
        GameObject gateObj = Instantiate(gatePrefab, position, Quaternion.identity);
        gateObj.GetComponent<GateNodeWrapper>().Initialize(this, channels);
    }

    public void CreatePitchNode(Vector3 position)
    {
        var format = ChannelEnumConverter.GetSoundFormatFromSpeakerMode(AudioSettings.speakerMode);
        var channels = ChannelEnumConverter.GetChannelCountFromSoundFormat(format);
        
        GameObject pitchObj = Instantiate(pitchPrefab, position, Quaternion.identity);
        pitchObj.GetComponent<PitchNodeWrapper>().Initialize(this, channels);
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