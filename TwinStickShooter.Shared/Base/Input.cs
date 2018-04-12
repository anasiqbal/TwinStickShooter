using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwinStickShooter
{
	static class Input
	{
		// track current and previous state of input devices
		// previous state is required to detect if the button was just pressed
		private static KeyboardState keyboardState, lastKeyboardState;
		private static MouseState mouseState, lastMouseState;
		private static GamePadState gamePadState, lastGamePadState;

		// track if the user is using mouse to aim
		private static bool isAimingWithMouse = false;

		public static Vector2 MousePosition { get { return new Vector2 (mouseState.X, mouseState.Y); } }

		public static void Update(GameTime gameTime)
		{
			// save previous states
			lastKeyboardState = keyboardState;
			lastMouseState = mouseState;
			lastGamePadState = gamePadState;

			// get current states
			keyboardState = Keyboard.GetState ();
			mouseState = Mouse.GetState ();
			gamePadState = GamePad.GetState (PlayerIndex.One);

			// if player uses any arrow keys or right thumbstick on gamepad, disable mouse aiming
			// otherwise is player moves the mouse, enable mouse aiming
			if (new [] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }.Any (key => keyboardState.IsKeyDown (key)) || gamePadState.ThumbSticks.Right != Vector2.Zero)
				isAimingWithMouse = false;
			else if (MousePosition != new Vector2 (lastMouseState.X, lastMouseState.Y))
				isAimingWithMouse = true;
		}

		/// <summary>
		/// Check if keyboard key was just pressed
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool WasKeyPressed(Keys key)
		{
			return lastKeyboardState.IsKeyUp (key) && keyboardState.IsKeyDown (key);
		}

		/// <summary>
		/// Check if gamepad button was just pressed
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		public static bool WasButtonPressed(Buttons button)
		{
			return lastGamePadState.IsButtonUp (button) && gamePadState.IsButtonDown (button);
		}


		public static Vector2 GetMovementDirection()
		{
			// controller thumbstick returns +ve y when pushed up
			// but monogame coordinates have +ve y downwards (origin is at the top-left corner)
			Vector2 direction = gamePadState.ThumbSticks.Left;
			direction.Y *= -1; // invert Y-axis

			if (keyboardState.IsKeyDown (Keys.A))
				direction.X -= 1;
			if (keyboardState.IsKeyDown (Keys.D))
				direction.X += 1;
			if (keyboardState.IsKeyDown (Keys.W))
				direction.Y -= 1;
			if (keyboardState.IsKeyDown (Keys.S))
				direction.Y += 1;

			// non-normalized move direction will cause unwanted movement speed increase/ decrease
			if (direction.LengthSquared () > 1)
				direction.Normalize ();

			return direction;
		}

		public static Vector2 GetAimDirection()
		{
			if (isAimingWithMouse)
				return GetMouseAimDirection ();

			// controller thumbstick returns +ve y when pushed up
			// but monogame coordinates have +ve y downwards (origin is at the top-left corner)
			Vector2 direction = gamePadState.ThumbSticks.Right;
			direction.Y *= -1; // invert Y-axis

			if (keyboardState.IsKeyDown (Keys.Left))
				direction.X -= 1;
			if (keyboardState.IsKeyDown (Keys.Right))
				direction.X += 1;
			if (keyboardState.IsKeyDown (Keys.Up))
				direction.Y -= 1;
			if (keyboardState.IsKeyDown (Keys.Down))
				direction.Y += 1;

			// if no aim direction then normalize it
			if (direction == Vector2.Zero)
				return Vector2.Zero;
			else
				return Vector2.Normalize (direction);
		}

		private static Vector2 GetMouseAimDirection()
		{
			Vector2 direction = MousePosition - PlayerShip.Instance.position;

			if (direction == Vector2.Zero)
				return Vector2.Zero;
			else
				return Vector2.Normalize (direction);
		}

		public static bool WasBombButtonPressed()
		{
			return WasButtonPressed (Buttons.LeftTrigger) || WasKeyPressed (Keys.Space);
		}
	}
}
