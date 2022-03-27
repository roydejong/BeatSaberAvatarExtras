using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberAvatarExtras.Models;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Tags.Settings;
using HMUI;
using SiraUtil.Affinity;
using UnityEngine;
using Zenject;

namespace BeatSaberAvatarExtras.Patches.Menu
{
    /// <summary>
    /// Patches the avatar color editor to show a rainbow option.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class EditAvatarColorViewControllerPatcher : IInitializable, IAffinity
    {
        [Inject] private readonly EditColorController _editColorViewController = null!;

        private ListSetting? _settingsControl = null;

        #region Events

        public void Initialize()
        {
            _editColorViewController.didChangeColorEvent += HandleColorEditChange;
        }

        private void HandleColorEditChange(Color value)
        {
            SyncSelectedSetting(value);
        }

        private void SyncSelectedSetting(Color color)
        {
            if (_settingsControl is not null)
                _settingsControl.Value = SpecialColorOption.DetectOptionMagically(color);
        }

        #endregion

        #region Patches

        [AffinityPatch(typeof(EditAvatarViewController), "DidActivate")]
        [AffinityPostfix]
        public void PostfixDidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                var bottomPanel = _editColorViewController.transform.Find("BottomPanel");
                
                _settingsControl = CreateListSetting(bottomPanel.transform, "Extra Options",
                    SpecialColorOption.AllOptions.Cast<object>().ToList(), SpecialColorOption.Default, OnSettingsControlChange);
            }
        }

        private void OnSettingsControlChange(object value)
        {
            if (value is SpecialColorOption option)
            {
                var setColor = Color.black; // TODO Back up "original" colors somehow

                if (option.MagicColor != null)
                    setColor = option.MagicColor.Value;
                
                _editColorViewController.SetColor(setColor);
                _editColorViewController.ChangeColor(setColor);
            }
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(EditColorController), nameof(EditColorController.SetColor))]
        public void PrefixSetColor(ref Color color)
        {
            SyncSelectedSetting(color);
        }

        #endregion

        #region BSML UI
        
        private ListSetting CreateListSetting(Transform parent, string label, List<object> options, object defaultValue,
            Action<object> onChangeAction)
        {
            var gameObject = (new ListSettingTag()).CreateObject(parent);
            
            var listSetting = gameObject.GetComponent<ListSetting>();
            listSetting.values = options;
            listSetting.Value = defaultValue;
            listSetting.onChange = new BSMLAction(this, onChangeAction.Method);
            
            var rectTransform = (gameObject.transform as RectTransform)!;
            rectTransform.position += new Vector3(0, .4f, 0);
            rectTransform.offsetMin += new Vector2(25f, 0);
            rectTransform.offsetMax += new Vector2(-20f, 0);

            gameObject.transform.Find("NameText")
                .GetComponent<CurvedTextMeshPro>()
                .SetText(label);
            
            return listSetting;
        }

        #endregion
    }
}