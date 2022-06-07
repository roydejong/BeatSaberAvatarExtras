using System.Text;

namespace BeatSaberAvatarExtras.Networking
{
    /// <summary>
    /// We pack extra information inside an avatar data field, so it can be stored and sent on the network
    ///  to ANY server, including official ones.
    /// </summary>
    public struct PackedExtrasString
    {
        public const char PackStart = '#';
        public const char PackDelim = '$';
        
        public string? GlassesId;
        public string? FacialHairId;

        public PackedExtrasString(string? glassesId, string? facialHairId)
        {
            GlassesId = glassesId;
            if (GlassesId is "" or "None") GlassesId = null;
            
            FacialHairId = facialHairId;
            if (FacialHairId is "" or "None") FacialHairId = null;
        }

        public static PackedExtrasString? TryFromAvatarData(AvatarData avatarData)
        {
            // Extras are stored in the facial hair 🥸
            return TryFromString(avatarData.facialHairId);
        }

        public static PackedExtrasString? TryFromString(string? str)
        {
            if (string.IsNullOrEmpty(str) || !str!.StartsWith(PackStart.ToString()))
                return null;

            str = str.Substring(1); // skip past PackStart
            
            var parts = str.Split(PackDelim);
            
            if (parts.Length < 2)
                return null;
            
            return new PackedExtrasString(parts[0], parts[1]);
        }

        public void ApplyTo(AvatarData avatarData)
        {
            // Extras are stored in the facial hair 🥸
            avatarData.facialHairId = ToString();
        }

        public void ApplyTo(ref MultiplayerAvatarData multiplayerAvatarData)
        {
            // Extras are stored in the facial hair 🥸
            multiplayerAvatarData
                .GetType()
                .GetField("facialHairId")
                .SetValueDirect(__makeref(multiplayerAvatarData), ToString());
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            
            sb.Append(PackStart);
            
            if (GlassesId is not null)
                sb.Append(GlassesId);
            
            sb.Append(PackDelim);
            
            if (FacialHairId is not null)
                sb.Append(FacialHairId);
            
            return sb.ToString();
        }
    }
}