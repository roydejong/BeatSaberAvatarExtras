using System.Collections.Generic;
using IPA.Config.Stores.Attributes;
using UnityEngine;

namespace BeatSaberAvatarExtras
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PluginConfig
    {
        [UseConverter]
        public Dictionary<string, Color>? BackupColors = new();

        public void StoreBackupColor(Color colorValue, EditAvatarViewController.AvatarEditPart editPart)
        {
            if (BackupColors == null)
                BackupColors = new();

            var key = editPart.ToString();
            
            BackupColors[key] = colorValue;
        }
        
        public Color? GetBackupColor(EditAvatarViewController.AvatarEditPart editPart)
        {
            if (BackupColors == null)
                return null;

            var key = editPart.ToString();

            if (BackupColors.TryGetValue(key, out var color))
                return color;

            return null;
        }
    }
}