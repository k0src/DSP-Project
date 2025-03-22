using Unity.Audio;
using UnityEngine;
using UnityEngine.UI;

public class PitchNodeWrapper : NodeWrapper 
{
    public new bool isOscillator = false;

    // ui params
    [SerializeField] private Slider pitchSlider;
    // [SerializeField] private Slider speedSlider;
    // [SerializeField] private Slider lengthSlider;
    // [SerializeField] private Slider evolutionSlider;
    // [SerializeField] private Button randomizeButton;

    private float pitch = 0f;

    // Output Wrapper Node
    [SerializeField] private NodeWrapper outputNode;

    private bool hasOutput = false;

    // TETS BUTTON
    [SerializeField] private bool pollPortsButton = false;

    // DSP Node and DSP Graph
    private DSPNode pitchNode;
    private DSPGraphManager graphManager;

    public override void Initialize(DSPGraphManager manager, int channels)
    {
        Debug.Log("Initializing Pitch Node");
        graphManager = manager;
        var commandBlock = manager.GetDSPGraph().CreateCommandBlock();

        // Create the Pitch Node
        pitchNode = commandBlock.CreateDSPNode<PitchNode.Parameters, PitchNode.Providers, PitchNode>();

        // Add the Outlet Port
        commandBlock.AddOutletPort(pitchNode, channels);
        commandBlock.Complete();
    }

    // Make underlying DSPNode accessible
    public override DSPNode GetDSPNode() => pitchNode;

    void Update()
    {
        pitch = pitchSlider.value;
        
        var commandBlock = graphManager.GetDSPGraph().CreateCommandBlock();

        // Set the Pitch Parameters
        commandBlock.SetFloat<PitchNode.Parameters, PitchNode.Providers, PitchNode>(pitchNode, PitchNode.Parameters.Frequency, pitch);

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
            commandBlock.Disconnect(pitchNode, 0, outputNode.GetDSPNode(), 1);
        }

        commandBlock.Connect(pitchNode, outputPort, outputNode.GetDSPNode(), inputPort);
        commandBlock.Complete();
    }

    void PollPorts()
    {
        hasOutput = outputNode != null && outputNode.GetDSPNode().Valid;

        if (hasOutput)
        {
            if (outputNode.isOscillator)
            {
                ConnectOutputNode(0, 1); // connect to input port 1 if output node is an oscillator
            }
            else
            {
                ConnectOutputNode(0, 0);
            }
        }
    }

    void OnDestroy()
    {
        if (graphManager != null && pitchNode.Valid)
        {
            var commandBlock = graphManager.GetDSPGraph().CreateCommandBlock();
            if (outputNode != null && outputNode.GetDSPNode().Valid)
            {
                commandBlock.Disconnect(pitchNode, 0, outputNode.GetDSPNode(), 0);
            }
            commandBlock.ReleaseDSPNode(pitchNode);
            commandBlock.Complete();
        }
    }

    public DSPGraph GetDSPGraph() => graphManager.GetDSPGraph();
}