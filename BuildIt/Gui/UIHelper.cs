using ColossalFramework.UI;
using System.Collections.Generic;
using UnityEngine;

namespace BuildIt.Gui
{
    public static class UIHelper
    {
        public static UIButton MakeTab(UITabstrip tabStrip, string txt, int width, UIPanel p, ColossalFramework.UI.MouseEventHandler eventClick)
        {
            UIButton tab = tabStrip.AddTab(txt, null, true);
            tab.name = txt;
            tab.text = txt;
            tab.size = new Vector2(width, 22);
            tab.textScale = 0.8f;
            tab.tabStrip = true;
            tab.eventClick += eventClick;
            tab.hoveredTextColor = new Color(1, 1, .45f, 1);
            tab.normalBgSprite = "SubBarButtonBase";
            tab.hoveredBgSprite = "SubBarButtonBaseHovered";
            tab.disabledBgSprite = "SubBarButtonBaseDisabled";
            tab.focusedBgSprite = "SubBarButtonBaseFocused";
            tab.pressedBgSprite = "SubBarButtonBasePressed";
            tab.objectUserData = p;
            return tab;
        }

        public static UIButton MakeButton(UIPanel panel, string txt, int x, int y, int width)
        {
            UIButton b = panel.AddUIComponent<UIButton>();
            b.text = txt;
            b.normalBgSprite = "ButtonMenu";
            b.hoveredBgSprite = "ButtonMenuHovered";
            b.disabledBgSprite = "ButtonMenuDisabled";
            b.focusedBgSprite = "ButtonMenuFocused";
            b.pressedBgSprite = "ButtonMenuPressed";
            b.size = new Vector2(width, 32);
            b.relativePosition = new Vector3(x, y);
            b.textScale = 0.9f;
            b.hoveredTextColor = new Color(1, 1, .45f, 1);
            return b;
        }

        public static CustomCheckbox MakeCheckBox(UIPanel panel, string txt, float y, ColossalFramework.UI.MouseEventHandler eventClick, string tooltip = "", float x = 15.0f)
        {
            UILabel label = panel.AddUIComponent<UILabel>();
            label.name = txt + "Label";
            label.text = txt;
            label.relativePosition = new Vector3(35.0f, y);
            label.textScale = 0.8f;

            CustomCheckbox cb = panel.AddUIComponent<CustomCheckbox>();
            cb.relativePosition = new Vector3(x, y);
            cb.size = new Vector2(12, 12);
            cb.eventClick += eventClick;
            cb.Show();
            cb.color = new Color32(185, 221, 254, 255);
            cb.enabled = true;
            cb.isChecked = true;
            cb.tooltip = tooltip;
            return cb;
        }

        public delegate void SliderSetValue(float value);

        public static UISlider MakeSlider(UIPanel panel, string name, string text, float y, float value, float min, float max, float step, SliderSetValue setValue, string tooltip = "")
        {
            UILabel label = panel.AddUIComponent<UILabel>();
            label.name = name + "Label";
            label.text = text;
            label.relativePosition = new Vector3(15.0f, y);
            label.textScale = 0.8f;

            UISlider slider = panel.AddUIComponent<UISlider>();
            slider.name = name + "Slider";
            slider.minValue = min;
            slider.maxValue = max;
            slider.stepSize = step;
            slider.value = value;
            slider.relativePosition = new Vector3(15.0f, y + 16);
            slider.size = new Vector2(170.0f, 16.0f);
            slider.tooltip = tooltip;

            UISprite thumbSprite = slider.AddUIComponent<UISprite>();
            thumbSprite.name = "ScrollbarThumb";
            thumbSprite.spriteName = "ScrollbarThumb";
            thumbSprite.Show();
            thumbSprite.size = new Vector2(8, 17);

            slider.backgroundSprite = "ScrollbarTrack";
            slider.thumbObject = thumbSprite;
            slider.orientation = UIOrientation.Horizontal;
            slider.isVisible = true;
            slider.enabled = true;
            slider.canFocus = true;
            slider.isInteractive = true;

            UILabel valueLabel = panel.AddUIComponent<UILabel>();
            valueLabel.name = name + "ValueLabel";
            valueLabel.text = slider.value.ToString("0.00");
            valueLabel.relativePosition = new Vector3(200.0f, y + 16);
            valueLabel.textScale = 0.8f;

            slider.eventValueChanged += (component, f) =>
            {
                setValue(f);
                valueLabel.text = slider.value.ToString("0.00");
            };

            return slider;
        }

        public static UITextField MakeTextField(UIPanel panel, string text, float y)
        {
            UILabel label = panel.AddUIComponent<UILabel>();

            label.name = text + "Label";
            label.text = text;
            label.relativePosition = new Vector3(15.0f, y);

            UITextField textField = panel.AddUIComponent<UITextField>();

            textField.atlas = GetAtlas("Ingame");
            textField.size = new Vector2(90f, 20f);
            textField.padding = new RectOffset(6, 6, 3, 3);
            textField.relativePosition = new Vector3(60.0f, y);
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;
            textField.horizontalAlignment = UIHorizontalAlignment.Center;
            textField.selectionSprite = "EmptySprite";
            textField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            textField.normalBgSprite = "TextFieldPanelHovered";
            textField.disabledBgSprite = "TextFieldPanelHovered";
            textField.textColor = new Color32(0, 0, 0, 255);
            textField.disabledTextColor = new Color32(80, 80, 80, 128);
            textField.color = new Color32(255, 255, 255, 255);

            return textField;
        }

        public static UIDropDown MakeDropDown(UIPanel panel, float y, string txt, string[] items, ColossalFramework.UI.PropertyChangedEventHandler<int> eventClick)
        {
            UILabel label = panel.AddUIComponent<UILabel>();
            UIDropDown dd = panel.AddUIComponent<UIDropDown>();
            UIButton ddb = panel.AddUIComponent<UIButton>();

            label.name = txt + "Label";
            label.text = txt;
            label.relativePosition = new Vector3(15.0f, y);
            label.textScale = 0.8f;
            y += 18;

            ddb.normalFgSprite = "ButtonPlay"; // PropertyGroupOpen
            ddb.width = 15;
            ddb.height = 16;
            ddb.verticalAlignment = UIVerticalAlignment.Middle;
            ddb.horizontalAlignment = UIHorizontalAlignment.Right;
            ddb.relativePosition = new Vector3(14, y + 2);

            dd.triggerButton = ddb;

            dd.name = txt;
            dd.size = new Vector2(200.0f, 20.0f);
            dd.textScale = 0.8f;
            dd.items = items;
            dd.relativePosition = new Vector3(33, y);
            dd.isVisible = true;
            dd.enabled = true;
            dd.isInteractive = true;
            dd.listBackground = "Servicebar"; //"BrushBackgroundDisabled";
            dd.itemHover = "ListItemHover";
            dd.itemHighlight = "ListItemHighlight";
            dd.normalBgSprite = "BrushBackgroundDisabled";
            dd.width = 200;
            dd.height = 20;
            dd.listWidth = 200;
            dd.itemHeight = 20;
            dd.itemPadding = new RectOffset(4, 20, 4, 4);
            dd.textFieldPadding = new RectOffset(4, 20, 4, 4);
            dd.selectedIndex = 0;

            dd.eventSelectedIndexChanged += eventClick;
            return dd;
        }

        private static Dictionary<string, UITextureAtlas> atlases;

        public static UITextureAtlas GetAtlas(string name)
        {
            if (atlases == null)
            {
                UIHelper.atlases = new Dictionary<string, UITextureAtlas>();

                UITextureAtlas[] atlases = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
                for (int i = 0; i < atlases.Length; i++)
                {
                    if (!UIHelper.atlases.ContainsKey(atlases[i].name))
                        UIHelper.atlases.Add(atlases[i].name, atlases[i]);
                }
            }

            return atlases[name];
        }
    }
}