using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwinStickShooter
{
	class BlackHole : Entity
	{
		static Random rand = new Random ();

		const int areaOfEffectRadius = 250;
		int hitpoints = 10;
		float sprayAngle = 0;

		public BlackHole(Vector2 position)
		{
			image = Art.BlackHole;
			this.position = position;
			radius = image.Width / 2f;
		}

		public void WasShot()
		{
			hitpoints--;
			if (hitpoints <= 0)
			{
				isExpired = true;
			}

			float hue = (float) ((3 * GameRoot.GameTime.TotalGameTime.TotalSeconds) % 6);
			Color color = ColorUtil.HSVToColor (hue, 0.25f, 1);
			const int numParticles = 150;
			float startOffset = rand.NextFloat (0, MathHelper.TwoPi / numParticles);

			for (int i = 0; i < numParticles; i++)
			{
				Vector2 sprayVel = MathUtil.FromPolar (MathHelper.TwoPi * i / numParticles + startOffset, rand.NextFloat (8, 16));
				Vector2 pos = position + 2f * sprayVel;
				var state = new ParticleState ()
				{
					velocity = sprayVel * 90,
					lengthMultiplier = 1,
					type = ParticleType.IgnoreGravity
				};

				GameRoot.ParticleManager.CreateParticle (Art.LineParticle, pos, color, 2.5f, 1.5f, state);
			}
		}

		public void Kill()
		{
			hitpoints = 0;
			WasShot ();
		}

		public override void Update(GameTime gameTime)
		{
			var entities = EntityManager.GetNearbyEntities (position, areaOfEffectRadius);
			foreach(Entity entity in entities)
			{
				if (entity is Enemy && !(entity as Enemy).IsActive)
					continue;

				if (entity is Bullet)
					entity.velocity += (entity.position - position).ScaleTo(12f);
				else
				{
					var dPos = position - entity.position;
					var length = dPos.Length ();

					entity.velocity += dPos.ScaleTo (MathHelper.Lerp (150f, 0, length / 250f));
				}
			}

			// The black holes spray some orbiting particles. The spray toggles on and off every quarter second.
			if ((GameRoot.GameTime.TotalGameTime.Milliseconds / 250) % 2 == 0)
			{
				Vector2 sprayVel = MathUtil.FromPolar (sprayAngle, rand.NextFloat (12, 15));
				Color color = ColorUtil.HSVToColor (5, 0.5f, 0.8f);  // light purple
				Vector2 pos = position + 2f * new Vector2 (sprayVel.Y, -sprayVel.X) + rand.NextVector2 (4, 8);
				var state = new ParticleState ()
				{
					velocity = sprayVel,
					lengthMultiplier = 1,
					type = ParticleType.Enemy
				};

				GameRoot.ParticleManager.CreateParticle (Art.LineParticle, pos, color, 3, 1.5f, state);
			}

			// rotate the spray direction
			sprayAngle -= MathHelper.TwoPi / 50f;
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			float scale = 1 + 0.1f * (float) Math.Sin(gameTime.TotalGameTime.TotalSeconds);
			spriteBatch.Draw (image, position, null, color, orientation, Size / 2f, scale, 0, 0);
			
			// draw the area of effect
			//spriteBatch.Draw (image, position, null, new Color(color, 0.3f), orientation, Size / 2f, 250f/ Size.X, 0, 0);
		}
	}
}
