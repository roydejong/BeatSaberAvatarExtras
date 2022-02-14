using System;
using HMUI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeatSaberAvatarExtras.UI
{
    public class CustomAvatarOptionField : MonoBehaviour
    {
        private const string CloneRootName = "Hands";
        private const string CloneInnerName = CloneRootName;

        private const float HorizontalOffset = -.02f;
        private const float VerticalOffset = -.14f;

        public static CustomAvatarOptionField Create(Transform editPanelRoot, string name, int offsetPosition)
        {
            var handsComponent = editPanelRoot.Find(CloneRootName);

            var clonedComponent = Object.Instantiate(handsComponent, editPanelRoot);
            clonedComponent.name = name;
            
            (clonedComponent.transform as RectTransform)!.position +=
                new Vector3(HorizontalOffset * offsetPosition, VerticalOffset * offsetPosition, 0);
            
            return clonedComponent.gameObject.AddComponent<CustomAvatarOptionField>();
        }

        public ImageView? Icon { get; private set; }
        public NamedIntListController? ValueController { get; private set; }
        public ColorPickerButtonController? PrimaryColorController { get; private set; }

        private void Awake()
        {
            Icon = transform.Find($"{CloneInnerName}/Icon").GetComponent<ImageView>();
            ValueController = transform.Find($"{CloneInnerName}/ValuePicker").GetComponent<NamedIntListController>();
            PrimaryColorController = transform.Find($"ColorPickerButton").GetComponent<ColorPickerButtonController>();
        }
    }
}