using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwinStickShooter
{
	public enum ParticleType { None, Enemy, Bullet, IgnoreGravity }
	public struct ParticleState
	{
		public Vector2 velocity;
		public ParticleType type;
		public float lengthMultiplier;

		public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle, GameTime gameTime)
		{
			var vel = particle.state.velocity;
			// TODO: switch to per second
			particle.position += vel;
			particle.orientation = vel.ToAngle ();

			float speed = vel.Length ();
			float alpha = Math.Min (1, Math.Min (particle.percentLife * 2, speed * 1f));
			alpha *= alpha;

			particle.color.A = (byte) (255 * alpha);
			particle.scale.X = particle.state.lengthMultiplier * Math.Min (Math.Min (1f, 0.2f * speed + 0.1f), alpha);

			// denormalized floats cause significant performance issues
			if (Math.Abs (vel.X) + Math.Abs (vel.Y) < 0.00000000001f)
				vel = Vector2.Zero;

			vel *= 0.97f; // particles gradually slow down
			particle.state.velocity = vel;
		}


	}
}
