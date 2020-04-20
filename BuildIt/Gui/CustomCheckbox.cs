using ColossalFramework.UI;

namespace BuildIt.Gui
{
    public class CustomCheckbox : UISprite
    {
        public bool isChecked { get; set; }

        public override void Update()
        {
            base.Update();
            spriteName = isChecked ? "BrushBackgroundFocused" : "BrushBackgroundDisabled";
        }
    }
}
