using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberAvatarExtras.Models;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Tags.Settings;
using HMUI;
using IPA.Utilities;
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
        [Inject] private readonly EditAvatarViewController _editAvatarViewController = null!;
        [Inject] private readonly EditColorController _editColorViewController = null!;

        private ListSetting? _listSetting = null;
        private EditAvatarViewController.AvatarEditPart? _selectedEditPart = null;
        private SpecialColorOption? _currentOption = null;
        
        #region Game Events

        public void Initialize()
        {
            _editAvatarViewController.didRequestColorChangeEvent += HandleColorEditBegin;
            _editColorViewController.didChangeColorEvent += HandleColorEditChange;
            _editColorViewController.didFinishEvent += HandleColorEditFinish;

            _selectedEditPart = null;
            _currentOption = null;
        }

        private void HandleColorEditBegin(Action<Color> colorCallback, Color currentColor,
            EditAvatarViewController.AvatarEditPart editPart, int uvSegment)
        {
            _selectedEditPart = editPart;
        }

        private void HandleColorEditChange(Color value)
        {
            SyncListSettingValue(value);
        }

        private void HandleColorEditFinish(bool didChange)
        {
            _selectedEditPart = null;
        }

        #endregion

        #region Game Patches

        [AffinityPatch(typeof(EditAvatarViewController), "DidActivate")]
        [AffinityPostfix]
        public void PostfixDidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                var bottomPanel = _editColorViewController.transform.Find("BottomPanel");
                
                _listSetting = CreateListSetting(bottomPanel.transform, "Extra Options",
                    SpecialColorOption.AllOptions.Cast<object>().ToList(), SpecialColorOption.Default, OnListSettingChange);
            }
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(EditColorController), nameof(EditColorController.SetColor))]
        public void PrefixSetColor(ref Color color)
        {
            SyncListSettingValue(color);
        }

        #endregion

        #region Game Helpers

        public Color GetSelectedColorValue()
        {
            var hsvPanel = _editColorViewController.GetField<HSVPanelController, EditColorController>("_hsvPanelController");
            return hsvPanel.color;
        }
        
        #endregion

        #region BSML UI
        
        private void SyncListSettingValue(Color color)
        {
            // Ensure the list setting is showing the appropriate value for the selected color
            var associatedColorOption = SpecialColorOption.DetectOptionMagically(color);
            
            if (_listSetting is not null)
                _listSetting.Value = associatedColorOption;

            _currentOption = associatedColorOption;
        }

        private void OnListSettingChange(object newValue)
        {
            // User toggled between special options
            
            if (newValue is SpecialColorOption newOption)
            {
                var selectedColor = GetSelectedColorValue();
                var wasSpecial = _currentOption != SpecialColorOption.Default;
                var isSpecial = newOption != SpecialColorOption.Default;
                
                // Back up original color if we are swapping Default -> Special
                if (!wasSpecial && _selectedEditPart is not null)
                {
                    Plugin.Config.StoreBackupColor(selectedColor, _selectedEditPart.Value);
                }

                // Set new color, restore from backup if swapping Special -> Default, or apply magic color for specials
                var setColor = Color.black;

                if (!isSpecial && _selectedEditPart is not null)
                {
                    var backupColor = Plugin.Config.GetBackupColor(_selectedEditPart.Value);

                    if (backupColor is not null)
                        setColor = backupColor.Value;
                }
                
                if (newOption.MagicColor is not null)
                    setColor = newOption.MagicColor.Value;
                else
                    setColor.a = 1;
                
                _editColorViewController.SetColor(setColor);
                _editColorViewController.ChangeColor(setColor);
                _currentOption = newOption;
            }
        }
        
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