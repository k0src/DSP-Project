using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using Unity.Audio;
using Unity.Collections;

public class MixerNodeWrapper : NodeWrapper 
{
    // Parameters
    [Range(0f, 5f)]
    public float masterGain = 1f;

    // Input Nodes
    [SerializeField]
    private List<OscNodeWrapper> inputNodes = new List<OscNodeWrapper>();

    private bool hasInput = false;

    // TEST BUTTON
    [SerializeField] private bool pollPortsButton = false;

    // DSP Node and DSP Graph
    private DSPNode mixerNode;
    private DSPGraphManager graphManager;

    public override void Initialize(DSPGraphManager manager, int channels)
    {
        graphManager = manager;
        var commandBlock = manager.GetDSPGraph().CreateCommandBlock();

        // Create the Mixer Node
        mixerNode = commandBlock.CreateDSPNode<MixerNode.Parameters, MixerNode.Providers, MixerNode>();

        for (int i = 0; i < 10; i++)
        {
            commandBlock.AddInletPort(mixerNode, channels);
        }

        // Add the Outlet Port
        commandBlock.AddOutletPort(mixerNode, channels);

        // Connect mixer node to Root DSP
        commandBlock.Connect(mixerNode, 0, manager.GetDSPGraph().RootDSP, 0);
        commandBlock.Complete();
    }

    // Make underlying DSPNode accessible
    public override DSPNode GetDSPNode() => mixerNode;

    void Update()
    {        
        var commandBlock = graphManager.GetDSPGraph().CreateCommandBlock();

        // Set the Mixer Parameters
        commandBlock.SetFloat<MixerNode.Parameters, MixerNode.Providers, MixerNode>(mixerNode, MixerNode.Parameters.Gain, masterGain);

        if (pollPortsButton)
        {
            pollPortsButton = false;
            PollPorts();
        }

        commandBlock.Complete();
    }

    void ConnectInputNode(int inputPort, int outputPort)
    {
        var commandBlock = graphManager.GetDSPGraph().CreateCommandBlock();
        
        if (hasInput)
        {
            commandBlock.Disconnect(inputNodes[inputPort].GetDSPNode(), 0, mixerNode, inputPort);
        }

        commandBlock.Connect(inputNodes[inputPort].GetDSPNode(), outputPort, mixerNode, inputPort);
        commandBlock.Complete();
    }

    void PollPorts()
    {
        hasInput = inputNodes.Count > 0 && inputNodes[0] != null && inputNodes[0].GetDSPNode().Valid;

        if (hasInput)
        {
            for (int i = 0; i < inputNodes.Count && i < 10; i++)
            {
                if (inputNodes[i] != null && inputNodes[i].GetDSPNode().Valid)
                {
                    ConnectInputNode(i, 0);
                }
            }
        }
    }

    void OnDestroy()
    {
        if (graphManager != null && mixerNode.Valid)
        {
            var commandBlock = graphManager.GetDSPGraph().CreateCommandBlock();
            for (int i = 0; i < inputNodes.Count && i < 10; i++)
            {
                if (inputNodes[i] != null && inputNodes[i].GetDSPNode().Valid)
                {
                    commandBlock.Disconnect(inputNodes[i].GetDSPNode(), 0, mixerNode, i);
                }
            }
            commandBlock.ReleaseDSPNode(mixerNode);
            commandBlock.Complete();
        }
    }

    public DSPGraph GetDSPGraph() => graphManager.GetDSPGraph();
}