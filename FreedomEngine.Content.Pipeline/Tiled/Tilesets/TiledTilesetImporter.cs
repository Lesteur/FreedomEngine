using System;
using System.IO;
using System.Text.Json;

using Microsoft.Xna.Framework.Content.Pipeline;

namespace FreedomEngine.Content.Pipeline.Tiled.Tilesets
{
    [ContentImporter(".json", ".tsj", DisplayName = "Tiled Tileset Importer", DefaultProcessor = "TiledTilesetProcessor")]
    public class TiledTilesetImporter : ContentImporter<TiledTilesetContent>
    {
        public override TiledTilesetContent Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing Tiled tileset: {0}", filename);

            try
            {
                // Read the JSON file content
                string jsonContent = File.ReadAllText(filename);

                // Configure JSON serializer options for case-insensitive matching if needed
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Deserialize ignoring unmapped fields
                var tilesetContent = JsonSerializer.Deserialize<TiledTilesetContent>(jsonContent, options);

                return tilesetContent;
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Failed to import Tiled tileset: {0}. Error: {1}", filename, ex.Message);
                throw;
            }
        }
    }
}