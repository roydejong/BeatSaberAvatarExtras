using UnityEngine;

namespace BeatSaberAvatarExtras.Assets
{
    public static class MaterialFactory
    {
        public const string MaterialNamePrefix = "BSAE_";
        
        private static Material? _flatColorMaterial = null;
        public static Material FlatColorMaterial
        {
            get
            {
                if (_flatColorMaterial != null)
                    return _flatColorMaterial;

                _flatColorMaterial = new Material(Shader.Find("Custom/SimpleLit"))
                {
                    name = $"{MaterialNamePrefix}FlatColorMaterial",
                    mainTexture = Texture2D.whiteTexture,
                    color = Color.white
                };
                return _flatColorMaterial;
            }
        }

        private static Material? _rainbowMaterial = null;
        public static Material RainbowMaterial
        {
            get
            {
                if (_rainbowMaterial != null)
                    return _rainbowMaterial;

                _rainbowMaterial = BundleLoader.GetMaterial("RainbowSource");

                if (_rainbowMaterial != null)
                {
                    _rainbowMaterial.name = $"{MaterialNamePrefix}RainbowMaterial";
                    return _rainbowMaterial;
                }

                // fallback
                return FlatColorMaterial; 
            }
        }
    }
}