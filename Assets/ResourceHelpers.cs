using System.Reflection;

namespace BeatSaberAvatarExtras.Assets
{
    public static class ResourceHelpers
    {
        public static byte[]? GetResource(Assembly asm, string resourceName)
        {
            var stream = asm.GetManifestResourceStream(resourceName);

            if (stream is null)
                return null;

            var data = new byte[stream.Length];
            stream.Read(data, 0, (int) stream.Length);
            return data;
        }
    }
}