using System;
using UnityEngine;

[CreateAssetMenu]
public class BoundingBox3DVariable : ScriptableObject, ISerializationCallbackReceiver
{
	[NonSerialized]
	public BoundingBox3D Value;
	public BoundingBox3D InitialValue = new BoundingBox3D();

	public void OnAfterDeserialize()
	{
		Value = InitialValue;
	}

	public void OnBeforeSerialize() { }
}