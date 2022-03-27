using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BeatSaberAvatarExtras.Assets
{
    /// <summary>
    /// Helper code adapted from BeatSaverDownloader
    /// Copyright (c) 2018 andruzzzhka (MIT Licensed)
    /// </summary>
    internal static class Sprites
    {
        /// Glasses icons created by Freepik - Flaticon
        /// https://www.flaticon.com/free-icons/glasses
        public static Sprite? Glasses;
        /// Glasses icons created by Freepik - Flaticon
        /// https://www.flaticon.com/free-icons/glasses
        public static Sprite? Mustache;
        public static Sprite? RainbowCircle;

        public static bool IsInitialized { get; private set; }

        public static void Initialize()
        {
            IsInitialized = true;

            Glasses = LoadSpriteFromResources("BeatSaberAvatarExtras.Assets.Sprites.Glasses.png");
            Mustache = LoadSpriteFromResources("BeatSaberAvatarExtras.Assets.Sprites.Mustache.png");
            RainbowCircle = LoadSpriteFromResources("BeatSaberAvatarExtras.Assets.Sprites.RainbowCircle.png");
        }

        private static Sprite? LoadSpriteFromResources(string resourcePath, float pixelsPerUnit = 100.0f)
        {
            var rawData = ResourceHelpers.GetResource(Assembly.GetCallingAssembly(), resourcePath);

            if (rawData is null)
                return null;

            var sprite = LoadSpriteRaw(rawData, pixelsPerUnit);

            if (sprite is null)
                return null;

            sprite.name = resourcePath;
            return sprite;
        }

        internal static Sprite? LoadSpriteRaw(byte[] image, float pixelsPerUnit = 100.0f)
        {
            var texture = LoadTextureRaw(image);

            if (texture is null)
                return null;

            return LoadSpriteFromTexture(texture, pixelsPerUnit);
        }

        private static Texture2D? LoadTextureRaw(byte[] file)
        {
            if (!file.Any())
                return null;

            var texture = new Texture2D(2, 2);
            return texture.LoadImage(file) ? texture : null;
        }

        private static Sprite LoadSpriteFromTexture(Texture2D spriteTexture, float pixelsPerUnit = 100.0f)
        {
            return Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height),
                new Vector2(0, 0), pixelsPerUnit);
        }
    }
}