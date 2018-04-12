using Microsoft.Xna.Framework;

namespace TwinStickShooter
{
	class Bullet : Entity
	{
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
				isExpired = true;
		}
		
	}
}
