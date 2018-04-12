using Microsoft.Xna.Framework;
using System;

namespace TwinStickShooter
{
	static class Extensions
	{
		/// <summary>
		/// takes a vector and returns an angle in radians
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static float ToAngle(this Vector2 vector)
		{
			return (float) Math.Atan2 (vector.Y, vector.X);
		}

		/// <summary>
		/// returns a random value between given minimum and maximum value
		/// </summary>
		/// <param name="rand"></param>
		/// <param name="minValue"></param>
		/// <param name="maxValue"></param>
		/// <returns></returns>
		public static float NextFloat(this Random rand, float minValue, float maxValue)
		{
			return (float) rand.NextDouble () * (maxValue - minValue) + minValue;
		}

		/// <summary>
		/// scales a vector to a given length. in other words changes the magnitude of vector to given length.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static Vector2 ScaleTo(this Vector2 vector, float length)
		{
			return vector * (length / vector.Length ());
		}

		/// <summary>
		/// Create a vecctor in random direction with magnitude between min and max length
		/// </summary>
		/// <param name="rand"></param>
		/// <param name="minLength"></param>
		/// <param name="maxLength"></param>
		/// <returns></returns>
		public static Vector2 NextVector2(this Random rand, float minLength, float maxLength)
		{
			double theta = rand.NextDouble () * 2 * Math.PI;
			float length = rand.NextFloat (minLength, maxLength);
			return new Vector2 (length * (float) Math.Cos (theta), length * (float) Math.Sin (theta));
		}
	}
}
