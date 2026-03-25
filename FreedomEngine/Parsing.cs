using System;
using System.Globalization;

using Microsoft.Xna.Framework;

namespace FreedomEngine
{
    public static class Parsing
    {
        public static bool TryParseColor(string token, out Color color)
        {
            color = Color.White;

            if (string.IsNullOrWhiteSpace(token))
                return false;

            token = token.Trim();

            switch (token.ToLowerInvariant())
            {
                case "white": color = Color.White; return true;
                case "black": color = Color.Black; return true;
                case "red": color = Color.Red; return true;
                case "green": color = Color.Green; return true;
                case "blue": color = Color.Blue; return true;
                case "yellow": color = Color.Yellow; return true;
                case "orange": color = Color.Orange; return true;
                case "purple": color = Color.Purple; return true;
                case "cyan": color = Color.Cyan; return true;
                case "magenta": color = Color.Magenta; return true;
                case "gray":
                case "grey": color = Color.Gray; return true;
            }

            if (token[0] == '#')
                token = token.Substring(1);

            try
            {
                if (token.Length == 6)
                {
                    byte r = byte.Parse(token.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte g = byte.Parse(token.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte b = byte.Parse(token.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    color = new Color(r, g, b, (byte)255);
                    return true;
                }

                if (token.Length == 8)
                {
                    byte a = byte.Parse(token.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte r = byte.Parse(token.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte g = byte.Parse(token.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte b = byte.Parse(token.AsSpan(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    color = new Color(r, g, b, a);
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public static bool TryParseScale(string[] parts, out Vector2 scale)
        {
            scale = Vector2.One;

            if (parts.Length < 2)
                return false;

            if (parts.Length == 2)
            {
                if (!TryParseFloat(parts[1], out float uniform))
                    return false;

                scale = new Vector2(uniform, uniform);
                return true;
            }

            if (!TryParseFloat(parts[1], out float x))
                return false;

            if (!TryParseFloat(parts[2], out float y))
                return false;

            scale = new Vector2(x, y);
            return true;
        }

        public static bool TryParseShake(string[] parts, out float amplitude)
        {
            amplitude = 0f;

            if (parts.Length < 1)
                return false;

            if (!TryParseFloat(parts[1], out amplitude))
                return false;

            return true;
        }

        public static bool TryParseFloat(string token, out float value)
        {
            return float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }
}
