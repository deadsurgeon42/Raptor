﻿//  Raptor - a client API for Terraria
//  Copyright (C) 2013-2015 MarioE
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Raptor.Extensions
{
	/// <summary>
	/// Terraria GUI-related sprite batch extensions.
	/// </summary>
	public static class GuiExtensions
	{
		internal static Texture2D solidTexture;

		/// <summary>
		/// Draws outlined text at the mouse.
		/// </summary>
		/// <param name="sb">The spritebatch.</param>
		/// <param name="str">The text.</param>
		/// <param name="color">The color.</param>
		/// <param name="font">The font.</param>
		public static void DrawGuiMouseText(this SpriteBatch sb, string str, Color color, SpriteFont font = null)
		{
			float xLength = Main.fontMouseText.MeasureString(str).X;
			float yLength = Main.fontMouseText.MeasureString(str).Y;

			float x = MathHelper.Clamp(Input.MouseX + 10f, 4f, Main.screenWidth - xLength - 4f);
			float y = MathHelper.Clamp(Input.MouseY + 10f, 4f, Main.screenHeight - yLength - 4f);
			
			Vector2 position = new Vector2(x, y);

			SpriteFont drawFont = font ?? Main.fontMouseText;
			sb.DrawString(drawFont, str, position + new Vector2(1.5f, 0), Color.Black);
			sb.DrawString(drawFont, str, position + new Vector2(-1.5f, 0), Color.Black);
			sb.DrawString(drawFont, str, position + new Vector2(0, 1.5f), Color.Black);
			sb.DrawString(drawFont, str, position + new Vector2(0, -1.5f), Color.Black);
			sb.DrawString(drawFont, str, position, color);
		}
		/// <summary>
		/// Draws a Terraria GUI rectangle.
		/// </summary>
		/// <param name="sb">The spritebatch.</param>
		/// <param name="rect">The rectangle.</param>
		/// <param name="color">The color.</param>
		/// <param name="texture">The backing texture.</param>
		public static void DrawGuiRectangle(this SpriteBatch sb, Rectangle rect, Color color, Texture2D texture = null)
		{
			// If there's anything better, please tell >.<

			Texture2D backTexture = texture ?? Raptor.rectBackTexture;

			sb.Draw(backTexture,
				new Rectangle(rect.X, rect.Y, 8, 8),
				new Rectangle(0, 0, 8, 8),
				color);
			sb.Draw(backTexture,
				new Rectangle(rect.X + rect.Width - 8, rect.Y, 8, 8),
				new Rectangle(44, 0, 8, 8),
				color);
			sb.Draw(backTexture,
				new Rectangle(rect.X, rect.Y + rect.Height - 8, 8, 8),
				new Rectangle(0, 44, 8, 8),
				color);
			sb.Draw(backTexture,
				new Rectangle(rect.X + rect.Width - 8, rect.Y + rect.Height - 8, 8, 8),
				new Rectangle(44, 44, 8, 8),
				color);
			sb.Draw(backTexture,
				new Rectangle(rect.X + 8, rect.Y, rect.Width - 16, 8),
				new Rectangle(9, 0, 34, 8),
				color);
			sb.Draw(backTexture,
				new Rectangle(rect.X + 8, rect.Y + rect.Height - 8, rect.Width - 16, 8),
				new Rectangle(9, 44, 34, 8),
				color);
			sb.Draw(backTexture,
				new Rectangle(rect.X, rect.Y + 8, 8, rect.Height - 16),
				new Rectangle(0, 9, 8, 34),
				color);
			sb.Draw(backTexture,
				new Rectangle(rect.X + rect.Width - 8, rect.Y + 8, 8, rect.Height - 16),
				new Rectangle(44, 9, 8, 34),
				color);
			sb.Draw(backTexture,
				new Rectangle(rect.X + 8, rect.Y + 8, rect.Width - 16, rect.Height - 16),
				new Rectangle(8, 8, 36, 36),
				color);
		}
		/// <summary>
		/// Draws outlined text.
		/// </summary>
		/// <param name="sb">The spritebatch.</param>
		/// <param name="str">The text.</param>
		/// <param name="position">The position.</param>
		/// <param name="color">The color.</param>
		/// <param name="font">The font.</param>
		public static void DrawGuiText(this SpriteBatch sb, string str, Vector2 position, Color color, SpriteFont font = null)
		{
			SpriteFont drawFont = font ?? Main.fontMouseText;
			sb.DrawString(drawFont, str, position + new Vector2(1.5f, 0), Color.Black);
			sb.DrawString(drawFont, str, position + new Vector2(-1.5f, 0), Color.Black);
			sb.DrawString(drawFont, str, position + new Vector2(0, 1.5f), Color.Black);
			sb.DrawString(drawFont, str, position + new Vector2(0, -1.5f), Color.Black);
			sb.DrawString(drawFont, str, position, color);
		}
		/// <summary>
		/// Draws a line.
		/// </summary>
		/// <param name="sb">The spritebatch.</param>
		/// <param name="p1">The first point.</param>
		/// <param name="p2">The second point.</param>
		/// <param name="color">The color.</param>
		public static void DrawGuiLine(this SpriteBatch sb, Vector2 p1, Vector2 p2, Color color)
		{
			if (solidTexture == null)
			{
				solidTexture = new Texture2D(sb.GraphicsDevice, 1, 1);
				solidTexture.SetData<Color>(new[] { Color.White });
			}

			sb.Draw(solidTexture, new Vector2(p1.X, p1.Y), null, color, (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X), Vector2.Zero,
				new Vector2(new Vector2(p1.X - p2.X, p1.Y - p2.Y).Length(), 1), SpriteEffects.None, 0);
		}
		/// <summary>
		/// Fills a rectangle with a solid color.
		/// </summary>
		/// <param name="sb">The spritebatch.</param>
		/// <param name="rectangle">The rectangle.</param>
		/// <param name="color">The color.</param>
		public static void FillRectangle(this SpriteBatch sb, Rectangle rectangle, Color color)
		{
			if (solidTexture == null)
			{
				solidTexture = new Texture2D(sb.GraphicsDevice, 1, 1);
				solidTexture.SetData<Color>(new[] { Color.White });
			}

			sb.Draw(solidTexture, rectangle, color);
		}
	}
}
