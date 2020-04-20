using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BuildIt.Gui
{
    public class GuiBuilder
    {
        public static GuiBuilder instance;

        private GameObject window;
        private MainPanel panel;
        private GameObject button1;
        private GameObject button2;
        private UITabstrip strip = null;
        private UIButton buildItButton;

        public void CreateUI()
        {
            window = new GameObject("Build It");

            UIView view = UIView.GetAView();

            strip = UIView.Find<UITabstrip>("MainToolstrip");

            button1 = UITemplateManager.GetAsGameObject("MainToolbarButtonTemplate");
            button2 = UITemplateManager.GetAsGameObject("ScrollablePanelTemplate");
            buildItButton = strip.AddTab("Build It!", button1, button2, new Type[] { }) as UIButton;
            buildItButton.eventClick += BuildItButtonClick;

            buildItButton.normalFgSprite = "InfoIconTerrainHeight";
            buildItButton.hoveredFgSprite = "InfoIconTerrainHeightHovered";
            buildItButton.focusedFgSprite = "InfoIconTerrainHeightFocused";
            buildItButton.pressedFgSprite = "InfoIconTerrainHeightPressed";
            buildItButton.tooltip = "Build a city from a map";

            panel = window.AddComponent<MainPanel>();
            panel.transform.parent = view.transform;
            panel.position = new Vector3(buildItButton.position.x - 240, buildItButton.position.y - 105);
            panel.Hide();
        }

        public void DestroyUI()
        {
            buildItButton.eventClick -= BuildItButtonClick;
            strip.RemoveUIComponent(buildItButton);
            UIView.Destroy(buildItButton);
            UIView.Destroy(panel);
            UIView.Destroy(window);
        }

        private void BuildItButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (panel.isVisible)
            {
                panel.isVisible = false;
                BuildTool.instance.enabled = false;
                panel.Hide();
            }
            else
            {
                panel.isVisible = true;
                BuildTool.instance.enabled = true;
                panel.BringToFront();
                panel.Show();
            }
        }

    }
}
