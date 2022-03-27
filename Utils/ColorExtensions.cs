using UnityEngine;

namespace BeatSaberAvatarExtras.Utils
{
    public static class ColorExtensions
    {
        public static bool ApproximatelyEquals(this Color a, Color? b)
        {
            if (!b.HasValue)
                return false;
            
            return ApproximatelyWithTolerance(a.r, b.Value.r) &&
                   ApproximatelyWithTolerance(a.g, b.Value.g) &&
                   ApproximatelyWithTolerance(a.b, b.Value.b) &&
                   ApproximatelyWithTolerance(a.a, b.Value.a);
        }
        
        private static bool ApproximatelyWithTolerance(float a, float b, float tolerance = 0.02f)
            => (Mathf.Abs(a - b) < tolerance);
    }
}