using System;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace FreedomEngine.Collections
{
    /// <summary>
    /// Provides utility methods for parsing various types of data from strings, such as colors and scales.
    /// </summary>
    public static class Parsing
    {
        /// <summary>
        /// Attempts to parse a color from the specified string representation.
        /// </summary>
        /// <remarks>Supported color names include common colors such as "white", "black", "red", "green",
        /// "blue", "yellow", "orange", "purple", "cyan", "magenta", and both "gray" and "grey". Hexadecimal color codes
        /// are supported in the formats RRGGBB and AARRGGBB, with or without a leading '#'. Parsing is
        /// case-insensitive.</remarks>
        /// <param name="token">The string containing the color name (e.g., "red", "blue") or a hexadecimal color code (with or without a
        /// leading '#').</param>
        /// <param name="color">When this method returns, contains the parsed color if the operation succeeded; otherwise, contains <see
        /// cref="Color.White"/>.</param>
        /// <returns>true if the string was successfully parsed as a color; otherwise, false.</returns>
        public static bool TryParseColor(string token, out Color color)
        {
            color = Color.White;

            if (string.IsNullOrWhiteSpace(token))
                return false;

            token = token.Trim();

            switch (token.ToLowerInvariant())
            {
                case "white":   color = Color.White;    return true;
                case "black":   color = Color.Black;    return true;
                case "red":     color = Color.Red;      return true;
                case "green":   color = Color.Green;    return true;
                case "blue":    color = Color.Blue;     return true;
                case "yellow":  color = Color.Yellow;   return true;
                case "orange":  color = Color.Orange;   return true;
                case "purple":  color = Color.Purple;   return true;
                case "cyan":    color = Color.Cyan;     return true;
                case "magenta": color = Color.Magenta;  return true;
                case "gray":
                case "grey":    color = Color.Gray;     return true;
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

        /// <summary>
        /// Attempts to parse a scale vector from an array of string representations.
        /// </summary>
        /// <param name="parts">An array of strings containing one or two elements to parse as the X and Y components of the scale. If the
        /// array contains one element, a uniform scale is applied to both components.</param>
        /// <param name="scale">When this method returns, contains the parsed scale as a Vector2 if parsing succeeded; otherwise, contains
        /// Vector2.One.</param>
        /// <returns>true if the scale was successfully parsed; otherwise, false.</returns>
        public static bool TryParseScale(string[] parts, out Vector2 scale)
        {
            scale = Vector2.One;

            if (parts.Length < 1)
                return false;

            if (parts.Length == 1)
            {
                if (!TryParseFloat(parts[0], out float uniform))
                    return false;

                scale = new Vector2(uniform, uniform);
                return true;
            }

            if (!TryParseFloat(parts[0], out float x))
                return false;

            if (!TryParseFloat(parts[1], out float y))
                return false;

            scale = new Vector2(x, y);
            return true;
        }

        /// <summary>
        /// Attempts to parse a floating-point number from the specified string token using invariant culture formatting.
        /// </summary>
        /// <param name="token">The string containing the floating-point number to parse.</param>
        /// <param name="value">When this method returns, contains the parsed float value if parsing succeeded; otherwise, contains 0.</param>
        /// <returns>true if the string was successfully parsed as a float; otherwise, false.</returns>
        public static bool TryParseFloat(string token, out float value)
        {
            return float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }
}