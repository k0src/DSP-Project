using System.Collections.Generic;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Audio;
using Unity.Mathematics;
using Unity.Collections;

public abstract class NodeWrapper : MonoBehaviour
{
    public abstract void Initialize(DSPGraphManager manager, int channels);
    public abstract DSPNode GetDSPNode();
    public virtual bool isOscillator { get; } = false;
}