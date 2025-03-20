using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using Unity.Audio;
using Unity.Collections;

public class GateNodeWrapper : NodeWrapper 
{
    // Parameters
    [Range(0f, 1)]
    public float speed = 0.5f;

    // Output Wrapper Node
    [SerializeField] private NodeWrapper outputNode;

    private bool hasOutput = false;

    // TETS BUTTON
    [SerializeField] private bool pollPortsButton = false;

    // DSP Node and DSP Graph
    private DSPNode gateNode;
    private DSPGraphManager graphManager;

    public override void Initialize(DSPGraphManager manager, int channels)
    {
        graphManager = manager;
        var commandBlock = manager.GetDSPGraph().CreateCommandBlock();

        // Create the Gate Node
        gateNode = commandBlock.CreateDSPNode<GateNode.Parameters, GateNode.Providers, GateNode>();

        // Add the Outlet Port
        commandBlock.AddOutletPort(gateNode, channels);
        commandBlock.Complete();
    }

    // Make underlying DSPNode accessible
    public override DSPNode GetDSPNode() => gateNode;

    void Update()
    {
        var commandBlock = graphManager.GetDSPGraph().CreateCommandBlock();

        // Set the Gate Parameters
        commandBlock.SetFloat<GateNode.Parameters, GateNode.Providers, GateNode>(gateNode, GateNode.Parameters.Speed, speed);

        commandBlock.Complete();

        if (pollPortsButton)
        {
            pollPortsButton = false;
            PollPorts();
        }
    }

    void ConnectOutputNode(int outputPort, int inputPort)
    {
        var commandBlock = graphManager.GetDSPGraph().CreateCommandBlock();
        
        if (hasOutput)
        {
            commandBlock.Disconnect(gateNode, 0, outputNode.GetDSPNode(), 0);
        }

        commandBlock.Connect(gateNode, outputPort, outputNode.GetDSPNode(), inputPort);
        commandBlock.Complete();
    }

    void PollPorts()
    {
        hasOutput = outputNode != null && outputNode.GetDSPNode().Valid;

        if (hasOutput)
        {
            ConnectOutputNode(0, 0);
        }
    }

    void OnDestroy()
    {
        if (graphManager != null && gateNode.Valid)
        {
            var commandBlock = graphManager.GetDSPGraph().CreateCommandBlock();
            if (outputNode != null && outputNode.GetDSPNode().Valid)
            {
                commandBlock.Disconnect(gateNode, 0, outputNode.GetDSPNode(), 0);
            }
            commandBlock.ReleaseDSPNode(gateNode);
            commandBlock.Complete();
        }
    }

    public DSPGraph GetDSPGraph() => graphManager.GetDSPGraph();
}