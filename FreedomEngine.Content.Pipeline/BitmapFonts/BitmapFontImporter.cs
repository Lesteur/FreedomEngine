using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Xna.Framework.Content.Pipeline;

namespace FreedomEngine.Content.Pipeline.BitmapFonts
{
    /// <summary>
    /// Imports a BMFont (.fnt) file in binary format (version 3).
    /// </summary>
    [ContentImporter(".fnt", DisplayName = "Bitmap Font Importer", DefaultProcessor = "BitmapFontProcessor")]
    public class BitmapFontImporter : ContentImporter<BitmapFontFileContent>
    {
        public override BitmapFontFileContent Import(string filename, ContentImporterContext context)
        {
            using var stream = File.OpenRead(filename);
            using var reader = new BinaryReader(stream);

            var font = new BitmapFontFileContent
            {
                FilePath = filename,

                // --- Read header ---
                Header = ReadStruct<BitmapFontFileContent.HeaderBlock>(reader)
            };

            if (!font.Header.IsValid)
                throw new InvalidOperationException("Invalid BMFont file. Only binary BMFont v3 is supported.");

            // --- Read blocks ---
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                byte blockType = reader.ReadByte();
                int blockSize = reader.ReadInt32();

                switch (blockType)
                {
                    // INFO
                    case 1:
                        {
                            font.Info = ReadStruct<BitmapFontFileContent.InfoBlock>(reader);

                            int nameLength = blockSize - Marshal.SizeOf<BitmapFontFileContent.InfoBlock>();
                            var nameBytes = reader.ReadBytes(nameLength);

                            // Font name is null-terminated
                            string fontName = Encoding.UTF8.GetString(nameBytes).TrimEnd('\0');

                            // Optional: store if needed later
                            // font.FontName = fontName;

                            break;
                        }

                    // COMMON
                    case 2:
                        {
                            font.Common = ReadStruct<BitmapFontFileContent.CommonBlock>(reader);
                            break;
                        }

                    // PAGES
                    case 3:
                        {
                            var pageData = reader.ReadBytes(blockSize);
                            var pages = Encoding.UTF8.GetString(pageData)
                                .Split('\0', StringSplitOptions.RemoveEmptyEntries);

                            foreach (var page in pages)
                                font.Pages.Add(page);

                            break;
                        }

                    // CHARS
                    case 4:
                        {
                            int structSize = Marshal.SizeOf<BitmapFontFileContent.CharacterBlock>();
                            int count = blockSize / structSize;

                            for (int i = 0; i < count; i++)
                            {
                                var character = ReadStruct<BitmapFontFileContent.CharacterBlock>(reader);
                                font.Characters.Add(character);
                            }

                            break;
                        }

                    // KERNING
                    case 5:
                        {
                            int structSize = Marshal.SizeOf<BitmapFontFileContent.KerningBlock>();
                            int count = blockSize / structSize;

                            for (int i = 0; i < count; i++)
                            {
                                var kerning = ReadStruct<BitmapFontFileContent.KerningBlock>(reader);
                                font.Kernings.Add(kerning);
                            }

                            break;
                        }

                    // UNKNOWN BLOCK → skip
                    default:
                        reader.BaseStream.Seek(blockSize, SeekOrigin.Current);
                        break;
                }
            }
            return font;
        }

        /// <summary>
        /// Reads a struct from the binary stream.
        /// </summary>
        private static T ReadStruct<T>(BinaryReader reader) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] buffer = reader.ReadBytes(size);

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }
    }
}