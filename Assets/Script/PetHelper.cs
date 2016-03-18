using UnityEngine;
using System.Collections;

public class PetHelper : MonoBehaviour {


	public static Vector3 ProjectPointLine (Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		Vector3 rhs = point - lineStart;
		Vector3 vector = lineEnd - lineStart;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector;
		if (magnitude > 1E-06f)
		{
			vector2 /= magnitude;
		}
		float num = Vector3.Dot (vector2, rhs);
		num = Mathf.Clamp (num, 0f, magnitude);
		return lineStart + vector2 * num;
	}
}
