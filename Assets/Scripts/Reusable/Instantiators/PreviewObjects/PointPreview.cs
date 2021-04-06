using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPreview : PreviewObject
{
	public PointPreview(Transform parent = null) : base(PrimitiveType.Sphere, parent)
	{
		SetName("PointPreview");
	}
}
