using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TwinStickShooter
{
	public class ParticleManager<T>
	{
		// delegat will be called for each particle
		Action<Particle, GameTime> updateParticle;
		CircularParticleArray particleList;

		public ParticleManager(int capacity, Action<Particle, GameTime> updateParticle)
		{
			this.updateParticle = updateParticle;
			particleList = new CircularParticleArray (capacity);

			for (int i = 0; i < capacity; i++)
				particleList [i] = new Particle ();
		}

		public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, float scale, T state, float theta = 0)
		{
			CreateParticle (texture, position, tint, duration, new Vector2(scale), state, theta);
		}

		public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, Vector2 scale, T state, float theta = 0)
		{
			Particle particle;
			if (particleList.Count == particleList.Capacity)
			{
				particle = particleList [0];
				particleList.Start++;
			}
			else
			{
				particle = particleList [particleList.Count];
				particleList.Count++;
			}

			particle.texture = texture;
			particle.position = position;
			particle.color = tint;
			particle.duration = duration;
			particle.percentLife = 1;
			particle.scale = scale;
			particle.orientation = theta;
			particle.state = state;
		}

		public void Update(GameTime gameTime)
		{
			int removalCount = 0;
			for(int i = 0; i < particleList.Count; i++)
			{
				var particle = particleList [i];
				updateParticle (particle, gameTime);
				
				particle.percentLife -= (1 / particle.duration);

				Swap (particleList, 1 - removalCount, i);

				if(particle.percentLife <= 0)
				{
					removalCount++;
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			for(int i = 0; i < particleList.Count; i++)
			{
				var particle = particleList [i];

				Vector2 origin = new Vector2 (particle.texture.Width / 2, particle.texture.Height / 2);
				spriteBatch.Draw (particle.texture, particle.position, null, particle.color, particle.orientation, origin, particle.scale, 0, 0);
			}
		}

		void Swap(CircularParticleArray list, int index1, int index2)
		{
			var temp = list [index1];
			list [index1] = list [index2];
			list [index2] = temp;
		}


		#region Helper Classes

		public class Particle
		{
			public Texture2D texture;
			public Vector2 position;
			public float orientation;

			public Vector2 scale = Vector2.One;

			public Color color;
			public float duration;
			public float percentLife = 1;
			public T state;
		}

		class CircularParticleArray
		{
			int start;
			public int Start
			{
				get { return start; }
				set { start = value % list.Length; }
			}

			Particle [] list;
			public int Count { get; set; }
			public int Capacity { get { return list.Length; } }

			public CircularParticleArray(int capacity)
			{
				list = new Particle [capacity];
			}

			public Particle this[int i]
			{
				get { return list [(start + i) % list.Length]; }
				set { list [(start + i) % list.Length] = value; }
			}
		}

		#endregion
	}
}
