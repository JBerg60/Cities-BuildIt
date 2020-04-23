//
// UI Stuff mostly taken from:
// https://github.com/lxteo/Cities-Skylines-Mapper
// https://github.com/AlexanderDzhoganov/Skylines-FPSCamera
// https://github.com/viakmaky/Skylines-ExtendedRoadUpgrade
// https://github.com/JapaMala/Skylines-ExtendedEditor
// https://github.com/SamsamTS/CS-MeshInfo
//
// Feel free to take what you need from here.
//
// This UI is optimized for the Map Editor where not all game sprites are available.
// Additionally, not all Map Editor sprites might be available In Game or in the Asset Editor.
//

using ColossalFramework.UI;
using System;
using UnityEngine;

namespace BuildIt.Gui
{
    public class MainPanel : UIPanel
    {
        // Bunde, Limburg NL
        private const double lat = 50.90111;
        private const double lon = 5.73407;

        private UILabel title;
        private UIDragHandle dragHandle;
        private UIButton closeButton;
        private UITextField latTextField;
        private UITextField lonTextField;
        private UILabel status;
        private UIButton buildButton;

        public override void Awake()
        {
            title = AddUIComponent<UILabel>();
            dragHandle = AddUIComponent<UIDragHandle>();
            closeButton = AddUIComponent<UIButton>();
            buildButton = AddUIComponent<UIButton>();
            latTextField = AddUIComponent<UITextField>();
            lonTextField = AddUIComponent<UITextField>();
            status = AddUIComponent<UILabel>();
        }

        public override void OnDestroy()
        {
            Destroy(status);
            Destroy(latTextField);
            Destroy(lonTextField);
            Destroy(buildButton);
            Destroy(closeButton);
            Destroy(dragHandle);
            Destroy(title);
        }

        public override void Start()
        {
            backgroundSprite = "MenuPanel";
            width = 280;
            height = 330;

            title.text = "Build It";
            title.relativePosition = new Vector3(15, 15);
            title.textScale = 0.9f;
            title.size = new Vector2(200, 30);

            dragHandle.width = width;
            dragHandle.height = 50;
            dragHandle.target = this;
            dragHandle.relativePosition = Vector3.zero;

            closeButton.relativePosition = new Vector2(width - 34, 6);
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.focusedBgSprite = "buttonclosepressed";
            closeButton.pressedBgSprite = "buttonclosepressed";
            closeButton.color = new Color(1, 1, 1, .15f);
            closeButton.hoveredColor = new Color(1, 1, 1, .75f);
            closeButton.eventClick += closeButton_eventClick;

            latTextField = UIHelper.MakeTextField(this, "Lat:", 60);
            latTextField.text = lat.ToString("N6");

            lonTextField = UIHelper.MakeTextField(this, "Lon:", 90);
            lonTextField.text = lon.ToString("N6");

            status.text = "";
            status.relativePosition = new Vector3(15, 260);
            status.textScale = 0.9f;
            status.size = new Vector2(200, 30);

            buildButton = UIHelper.MakeButton(this, "Go", 10, 290, 60);
            buildButton.enabled = !Builder.instance.IsRunning;
            buildButton.eventClick += BuildButtonClick;

            Builder.instance.OnUpdate += BuilderOnUpdate;
        }

        private void BuilderOnUpdate(object sender, EventArgs e)
        {
            status.text = Builder.instance.Status;
            buildButton.enabled = !Builder.instance.IsRunning;
        }

        private void BuildButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {            
            Debug.Log("Go button click");
            // TODO : set params from the Gui, lat/lon etc.
            Builder.instance.Start();
        }

        private void closeButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            Hide();
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isVisible)
            {
                Hide();
            }
        }
    }
}
