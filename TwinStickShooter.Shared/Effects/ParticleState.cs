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

		static Random rand = new Random ();

		public ParticleState(Vector2 velocity, ParticleType type, float lengthMultiplier = 1f)
		{
			this.velocity = velocity;
			this.type = type;
			this.lengthMultiplier = lengthMultiplier;
		}

		public static ParticleState GetRandom(float minVel, float maxVel)
		{
			var state = new ParticleState ();
			state.velocity = rand.NextVector2 (minVel, maxVel);
			state.type = ParticleType.None;
			state.lengthMultiplier = 1;

			return state;
		}

		public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle, GameTime gameTime)
		{
			var vel = particle.state.velocity;
			particle.position += vel * (float) gameTime.ElapsedGameTime.TotalSeconds;
			particle.orientation = vel.ToAngle ();

			float speed = vel.Length ();
			float alpha = Math.Min (1, Math.Min (particle.percentLife * 2, speed * 1f));
			alpha *= alpha;

			particle.color.A = (byte) (255 * alpha);
			particle.scale.X = particle.state.lengthMultiplier * Math.Min (Math.Min (1f, 0.2f * speed + 0.1f), alpha);

			// denormalized floats cause significant performance issues
			if (Math.Abs (vel.X) + Math.Abs (vel.Y) < 0.00000000001f)
				vel = Vector2.Zero;

			// Make particles bounce off the edge of the screen (collision with screen edge)
			var pos = particle.position;
			int width = (int) GameRoot.ScreenSize.X;
			int height = (int) GameRoot.ScreenSize.Y;

			if (pos.X < 0)
				vel.X = Math.Abs (vel.X);
			else if (pos.X > width)
				vel.X = -Math.Abs (vel.X);

			if (pos.Y < 0)
				vel.Y = Math.Abs (vel.Y);
			else if (pos.Y > height)
				vel.Y = -Math.Abs (vel.Y);


			foreach(BlackHole blackHole in EntityManager.blackHoles)
			{
				var dPos = blackHole.position - pos;
				float distance = dPos.Length ();
				var n = dPos / distance;
				vel += 1000000 * n / (distance * distance + 10000);

				// add tangential acceleration for nearby particles
				if (distance < 400)
					vel += 200 * new Vector2 (n.Y, -n.X) / (distance + 100);
			}

			vel *= 0.97f; // particles gradually slow down
			particle.state.velocity = vel;
		}


	}
}
