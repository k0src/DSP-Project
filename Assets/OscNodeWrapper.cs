using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using Unity.Audio;
using Unity.Collections;

public class OscNodeWrapper : NodeWrapper 
{
    private enum OscType
    {
        Sine,
        Square,
        Triangle,
        Sawtooth
    }

    [SerializeField] private OscType type = OscType.Sine;

    private bool hasGateInput = false;
    private bool hasPitchInput = false;
    private bool hasOutput = false;

    //TEST BUTTON
    [SerializeField] private bool pollPortsButton = false;

    // Parameters

    [Range(20f, 20000f)]
    public float frequency = 440f;

    [Range(0f, 1f)]
    public float amplitude = 0.5f;

    // [Range(0f, 1f)]
    // public float attack = 0.1f;

    // [Range(0f, 1f)]
    // public float decay = 0.1f;

    // Input and Output Wrapper Nodes
    [SerializeField] private NodeWrapper gateInputNode;
    // [SerializeField] private NodeWrapper pitchInputNode;
    [SerializeField] private NodeWrapper outputNode;

    // DSP Node and DSP Graph
    private DSPNode oscNode;
    private DSPGraphManager graphManager;

    public override void Initialize(DSPGraphManager manager, int channels)
    {
        graphManager = manager;
        var commandBlock = manager.GetDSPGraph().CreateCommandBlock();

        // Create the Oscillator Node based on the type
        switch (type)
        {
            case OscType.Sine:
                oscNode = commandBlock.CreateDSPNode<SineNode.Parameters, SineNode.Providers, SineNode>();
                break;
            // case OscType.Square:
            //     oscNode = commandBlock.CreateDSPNode<SquareNode.Parameters, SquareNode.Providers, SquareNode>();
            //     break;
            // case OscType.Triangle:
            //     oscNode = commandBlock.CreateDSPNode<TriangleNode.Parameters, TriangleNode.Providers, TriangleNode>();
            //     break;
            // case OscType.Sawtooth:
            //     oscNode = commandBlock.CreateDSPNode<SawtoothNode.Parameters, SawtoothNode.Providers, SawtoothNode>();
            //     break;
        }

        // Add the Inlet and Outlet Port
        commandBlock.AddInletPort(oscNode, channels);
        // commandBlock.AddInletPort(oscNode, channels);
        commandBlock.AddOutletPort(oscNode, channels);
        commandBlock.Complete();
    }

    // Make underlying DSPNode accessible
    public override DSPNode GetDSPNode() => oscNode;

    void Update()
    {
        var commandBlock = graphManager.GetDSPGraph().CreateCommandBlock();

        // Set the Oscillator Parameters
        switch (type)
        {
            case OscType.Sine:
                commandBlock.SetFloat<SineNode.Parameters, SineNode.Providers, SineNode>(oscNode, SineNode.Parameters.Frequency, frequency);
                commandBlock.SetFloat<SineNode.Parameters, SineNode.Providers, SineNode>(oscNode, SineNode.Parameters.Amplitude, amplitude);
                // set adsr!!!
                break;
            // case OscType.Square:
            //     commandBlock.SetFloat<SquareNode.Parameters, SquareNode.Providers, SquareNode>(oscNode, SquareNode.Parameters.Frequency, frequency);
            //     commandBlock.SetFloat<SquareNode.Parameters, SquareNode.Providers, SquareNode>(oscNode, SquareNode.Parameters.Amplitude, amplitude);
            //     break;
            // case OscType.Triangle:
            //     commandBlock.SetFloat<TriangleNode.Parameters, TriangleNode.Providers, TriangleNode>(oscNode, TriangleNode.Parameters.Frequency, frequency);
            //     commandBlock.SetFloat<TriangleNode.Parameters, TriangleNode.Providers, TriangleNode>(oscNode, TriangleNode.Parameters.Amplitude, amplitude);
            //     break;
            // case OscType.Sawtooth:
            //     commandBlock.SetFloat<SawtoothNode.Parameters, SawtoothNode.Providers, SawtoothNode>(oscNode, SawtoothNode.Parameters.Frequency, frequency);
            //     commandBlock.SetFloat<SawtoothNode.Parameters, SawtoothNode.Providers, SawtoothNode>(oscNode, SawtoothNode.Parameters.Amplitude, amplitude);
            //     break;
        }

        if (pollPortsButton) 
        {
            PollPorts();
            pollPortsButton = false;
        }
        
        commandBlock.Complete();
    }

    void ConnectInputNode(int outputPort, int inputPort, NodeWrapper inputNode)
    {
        var commandBlock = graphManager.GetDSPGraph().CreateCommandBlock();

        if (hasGateInput)
        {
            commandBlock.Disconnect(gateInputNode.GetDSPNode(), 0, oscNode, 0);
        }

        commandBlock.Connect(inputNode.GetDSPNode(), outputPort, oscNode, inputPort);
        commandBlock.Complete();
    }

    void ConnectOutputNode(int outputPort, int inputPort, NodeWrapper newOutputNode)
    {
        var commandBlock = graphManager.GetDSPGraph().CreateCommandBlock();

        if (hasOutput)
        {
            commandBlock.Disconnect(oscNode, 0, outputNode.GetDSPNode(), 0);
        }

        commandBlock.Connect(oscNode, outputPort, newOutputNode.GetDSPNode(), inputPort);
        commandBlock.Complete();
    }

    public void PollPorts()
    {
        hasGateInput = gateInputNode != null && gateInputNode.GetDSPNode().Valid;
        //pitch
        hasOutput = outputNode != null && outputNode.GetDSPNode().Valid;

        if (hasGateInput)
        {
            ConnectInputNode(0, 0, gateInputNode);
        }


        if (hasOutput)
        {
            ConnectOutputNode(0, 0, outputNode);
        }
    }

    void OnDestroy()
    {
        if (graphManager != null && oscNode.Valid)
        {
            var commandBlock = graphManager.GetDSPGraph().CreateCommandBlock();
            if (gateInputNode != null && gateInputNode.GetDSPNode().Valid)
            {
                commandBlock.Disconnect(gateInputNode.GetDSPNode(), 0, oscNode, 0);
            }
            if (outputNode != null && outputNode.GetDSPNode().Valid)
            {
                commandBlock.Disconnect(oscNode, 0, outputNode.GetDSPNode(), 0);
            }
            commandBlock.ReleaseDSPNode(oscNode);
            commandBlock.Complete();
        }
    }

    public DSPGraph GetDSPGraph() => graphManager.GetDSPGraph();
}