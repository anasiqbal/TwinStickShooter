using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStickShooter
{
	/// <summary>
	/// Abstract class for all entities in the game (i.e. Player, Bullets, Enemies)
	/// </summary>
	abstract class Entity
	{
		protected Texture2D image;

		// tint of the object
		protected Color color = Color.White;

		public Vector2 position, velocity;
		public float orientation;
		public float radius = 20;       // for circular collision detecction
		public bool isExpired;			// if entity was destroyed and should be deleted

		public Vector2 Size
		{
			get
			{
				return image == null ? Vector2.Zero : new Vector2 (image.Width, image.Height);
			}
		}

		// update method for every entity called every frame
		public abstract void Update(GameTime gameTime);

		// draw method for every entity called every frame to draw the entity itself
		public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.Draw (image, position, null, color, orientation, Size/2f, 1f, 0f, 0f);
		}

	}
}
