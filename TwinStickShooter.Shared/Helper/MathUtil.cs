using Microsoft.Xna.Framework;
using System;

namespace TwinStickShooter
{
	static class MathUtil
	{
		/// <summary>
		/// Create a new vector at the given angle and with the given magnitude
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="magnitude"></param>
		/// <returns></returns>
		public static Vector2 FromPolar(float angle, float magnitude)
		{
			return magnitude * new Vector2 ((float) Math.Cos (angle), (float) Math.Sin (angle));
		}
	}
}
