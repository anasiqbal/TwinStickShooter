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
				isExpired = true;
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
