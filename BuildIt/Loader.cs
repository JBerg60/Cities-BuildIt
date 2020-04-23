using BuildIt.Gui;
using ICities;
using UnityEngine;

namespace BuildIt
{
    public class Loader : LoadingExtensionBase
    {
        public LoadMode loadMode;
        public bool IsGameLoaded { get; private set; }
     
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            loadMode = mode;
            InstallMod();
        }

        public override void OnLevelUnloading()
        {
            UninstallMod();
            base.OnLevelUnloading();
        }

        private void InstallMod()
        {
            BuildTool.instance = ToolsModifierControl.toolController.gameObject.GetComponent<BuildTool>();
            if (BuildTool.instance == null)
            {
                BuildTool.instance = ToolsModifierControl.toolController.gameObject.AddComponent<BuildTool>();
            }
            else
            {
                Debug.Log($"InstallMod with existing instance!");
            }

            Builder.instance = new Builder();
            Builder.instance.Init();

            GuiBuilder.instance = new GuiBuilder();
            GuiBuilder.instance.CreateUI();

            Debug.Log($"Mod version {ModInfo.Version} installed!");

            IsGameLoaded = true;
        }

        private void UninstallMod()
        {
            Builder.instance.Stop();
            GuiBuilder.instance.DestroyUI();

            if (BuildTool.instance != null)
            {
                BuildTool.Destroy(BuildTool.instance);
                BuildTool.instance = null;
            }

            IsGameLoaded = false;
        }
    }
}
