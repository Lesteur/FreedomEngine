using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FreedomEngine.Content.Pipeline.BitmapFonts
{
    /// <summary>
    /// Represents the raw data extracted from a BMFont (.fnt) file.
    /// This class is used during the Content Pipeline import/processing stages.
    /// </summary>
    public sealed class BitmapFontFileContent
    {
        /// <summary>
        /// Original path of the .fnt file.
        /// </summary>
        public string FilePath;

        /// <summary>
        /// Header block (must match BMFont signature).
        /// </summary>
        public HeaderBlock Header;

        /// <summary>
        /// Global font information.
        /// </summary>
        public InfoBlock Info;

        /// <summary>
        /// Common font metrics.
        /// </summary>
        public CommonBlock Common;

        /// <summary>
        /// Texture pages used by the font.
        /// </summary>
        public List<string> Pages { get; } = new List<string>();

        /// <summary>
        /// Character definitions.
        /// </summary>
        public List<CharacterBlock> Characters { get; } = new List<CharacterBlock>();

        /// <summary>
        /// Kerning pairs.
        /// </summary>
        public List<KerningBlock> Kernings { get; } = new List<KerningBlock>();

        #region Binary Block Definitions

        /// <summary>
        /// BMFont header (must be "BMF" + version 3).
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HeaderBlock
        {
            public byte B;
            public byte M;
            public byte F;
            public byte Version;

            public bool IsValid =>
                B == 0x42 && // 'B'
                M == 0x4D && // 'M'
                F == 0x46 && // 'F'
                Version == 3;
        }

        /// <summary>
        /// Font info block.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct InfoBlock
        {
            public short FontSize;
            public byte BitField;
            public byte CharSet;
            public ushort StretchH;
            public byte AntiAliasing;

            public byte PaddingUp;
            public byte PaddingRight;
            public byte PaddingDown;
            public byte PaddingLeft;

            public sbyte SpacingHoriz;
            public sbyte SpacingVert;

            public byte Outline;
        }

        /// <summary>
        /// Common font metrics.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CommonBlock
        {
            public ushort LineHeight;
            public ushort Base;
            public ushort ScaleWidth;
            public ushort ScaleHeight;
            public ushort PageCount;

            public byte BitField;
            public byte AlphaChannel;
            public byte RedChannel;
            public byte GreenChannel;
            public byte BlueChannel;
        }

        /// <summary>
        /// A single character glyph definition.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CharacterBlock
        {
            public int Id;

            public ushort X;
            public ushort Y;
            public ushort Width;
            public ushort Height;

            public short XOffset;
            public short YOffset;
            public short XAdvance;

            public byte Page;
            public byte Channel;
        }

        /// <summary>
        /// Kerning information between two characters.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct KerningBlock
        {
            public int First;
            public int Second;
            public short Amount;
        }

        #endregion
    }
}