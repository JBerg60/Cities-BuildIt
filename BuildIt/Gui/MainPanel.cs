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
using System.Collections.Generic;
using UnityEngine;

namespace BuildIt.Gui
{
    public class MainPanel : UIPanel
    {
        private UILabel title;
        private UIDragHandle dragHandle;
        private UITabContainer tabContainer;
        private TerrainPanel terrainPanel;
        //private ResourcePanel resPanel;
        //private TreePanel treePanel;
        private UITabstrip tabStrip;
        private UIButton closeButton;

        private List<UIButton> tabs;

        public override void Awake()
        {
            title = AddUIComponent<UILabel>();
            dragHandle = AddUIComponent<UIDragHandle>();
            tabStrip = AddUIComponent<UITabstrip>();
            tabContainer = AddUIComponent<UITabContainer>();
            closeButton = AddUIComponent<UIButton>();

            terrainPanel = tabContainer.AddUIComponent<TerrainPanel>();
            //resPanel = tabContainer.AddUIComponent<ResourcePanel>();
            //treePanel = tabContainer.AddUIComponent<TreePanel>();
        }

        public override void OnDestroy()
        {
            foreach (UIButton tab in tabs)
            {
                Destroy(tab);
            }

            //Destroy(treePanel);
            //Destroy(resPanel);
            Destroy(terrainPanel);
            Destroy(closeButton);
            Destroy(tabContainer);
            Destroy(tabStrip);
            Destroy(dragHandle);
            Destroy(title);
        }

        public override void Start()
        {
            backgroundSprite = "MenuPanel";
            width = 280;
            height = 330;

            title.text = "Terrain Tools";
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

            tabContainer.relativePosition = new Vector3(0, 84);
            tabContainer.width = width;
            tabContainer.backgroundSprite = "GenericPanel";
            tabContainer.color = new Color(.4f, .4f, .4f, 1.0f);

            tabStrip.width = width - 20;
            tabStrip.height = 18;
            tabStrip.relativePosition = new Vector3(10, 50);
            tabStrip.startSelectedIndex = 0;
            tabStrip.selectedIndex = -1;

            tabs = new List<UIButton>();
            tabs.Add(UIHelper.MakeTab(tabStrip, "Terrain", 54, terrainPanel, baseTabButton_eventClick));
            //tabs.Add(UIHelper.MakeTab(tabStrip, "Resources", 74, resPanel, baseTabButton_eventClick));
            //tabs.Add(UIHelper.MakeTab(tabStrip, "Trees", 54, treePanel, baseTabButton_eventClick));

            //resPanel.Hide();
            //treePanel.Hide();
            terrainPanel.Show();
        }

        private void checkTabs()
        {
            if (tabs == null)
                return;

            foreach (UIButton tab in tabs)
            {
                if (((UIPanel)tab.objectUserData).isVisible == true && tab.name == "Resources")
                {
                    InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.NaturalResources, InfoManager.SubInfoMode.Default);
                    return;
                }
            }
            InfoManager.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();
            checkTabs();
        }

        private void closeButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            Hide();
        }

        private void baseTabButton_eventClick(UIComponent c, UIMouseEventParameter e)
        {
            foreach (UIButton tab in tabs)
            {
                UIPanel p = (UIPanel)tab.objectUserData;
                if (tab.name == c.name)
                {
                    p.Show();
                }
                else
                {
                    p.Hide();
                }
            }
            checkTabs();
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
