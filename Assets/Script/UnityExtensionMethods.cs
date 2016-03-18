using UnityEngine;
using System.Collections;

public static class UnityExtensionMethods {
	/// <summary>
	/// Determines whether the quaternion is safe for interpolation or use with transform.rotation.
	/// </summary>
	/// <returns><c>false</c> if using the quaternion in Quaternion.Lerp() will result in an error (eg. NaN values or zero-length quaternion).</returns>
	/// <param name="quaternion">Quaternion.</param>
	public static bool IsValid(this Quaternion quaternion)
	{
		bool isNaN = float.IsNaN(quaternion.x + quaternion.y + quaternion.z + quaternion.w);
		
		bool isZero = quaternion.x == 0 && quaternion.y == 0 && quaternion.z == 0 && quaternion.w == 0;
		
		return !(isNaN || isZero);
	}

	public static bool IsValid(this Vector3 euler)
	{
		bool isNaN = float.IsNaN(euler.x + euler.y + euler.z);

		return !isNaN;
	}

}