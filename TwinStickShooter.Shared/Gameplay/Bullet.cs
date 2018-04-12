using Microsoft.Xna.Framework;
using System;

namespace TwinStickShooter
{
	class Bullet : Entity
	{
		static Random rand = new Random ();

		public Bullet(Vector2 position, Vector2 velocity)
		{
			image = Art.Bullet;
			this.position = position;
			this.velocity = velocity;
			orientation = velocity.ToAngle ();
			radius = 8;
		}

		public override void Update(GameTime gameTime)
		{
			// update the orientation each frame
			if (velocity.LengthSquared () > 0)
				orientation = velocity.ToAngle ();

			// update position of bullet each frame using pixel/second
			position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

			// if bullet moves out of the screen destroy it
			if (!GameRoot.Viewport.Bounds.Contains (position.ToPoint ()))
			{
				isExpired = true;

				ParticleState state;
				for(int i = 0; i < 15; i++)
				{
					state = new ParticleState ()
					{
						velocity = rand.NextVector2 (0, 150),
						type = ParticleType.Bullet,
						lengthMultiplier = 1
					};
					GameRoot.ParticleManager.CreateParticle (Art.LineParticle, position, Color.LightBlue, 0.6f, 1, state);
				}
			}
		}
		
	}
}
