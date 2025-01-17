// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LibreLancer;
using LibreLancer.ContentEdit;
using LibreLancer.ImUI;
using LibreLancer.Media;
using ImGuiNET;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace LancerEdit
{
	public class MainWindow : Game
	{
		public ImGuiHelper guiHelper;
		public AudioManager Audio;
		public GameResourceManager Resources;
		public PolylineRender Polyline;
		public PhysicsDebugRenderer DebugRender;
		public CommandBuffer Commands; //This is a huge object - only have one
		public MaterialMap MaterialMap;
        public RichTextEngine RichText;
        public FontManager Fonts;
        public string Version;
        TextBuffer logBuffer;
        StringBuilder logText = new StringBuilder();
        private RecentFilesHandler recentFiles;
        bool openError = false;
        bool finishLoading = false;

        FileDialogFilters UtfFilters = new FileDialogFilters(
            new FileFilter("All Utf Files","utf","cmp","3db","dfm","vms","sph","mat","txm","ale","anm"),
            new FileFilter("Utf Files","utf"),
            new FileFilter("Anm Files","anm"),
            new FileFilter("Cmp Files","cmp"),
            new FileFilter("3db Files","3db"),
            new FileFilter("Dfm Files","dfm"),
            new FileFilter("Vms Files","vms"),
            new FileFilter("Sph Files","sph"),
            new FileFilter("Mat Files","mat"),
            new FileFilter("Txm Files","txm"),
            new FileFilter("Ale Files","ale")
        );
        FileDialogFilters ImportModelFilters = new FileDialogFilters(
            new FileFilter("Model Files","dae","gltf","glb","obj"),
            new FileFilter("Collada Files", "dae"),
            new FileFilter("glTF Files", "gltf"),
            new FileFilter("glTF Binary Files", "glb"),
            new FileFilter("Wavefront Obj Files", "obj")
        );
        FileDialogFilters FreelancerIniFilter = new FileDialogFilters(
            new FileFilter("Freelancer.ini","freelancer.ini")
        );
        FileDialogFilters ImageFilter = new FileDialogFilters(
            new FileFilter("Images", "bmp", "png", "tga", "dds", "jpg", "jpeg")
        );
        public EditorConfiguration Config;
        OptionsWindow options;
        public MainWindow() : base(800,600,false)
		{
            Version = "LancerEdit " + Platform.GetInformationalVersion<MainWindow>();
			MaterialMap = new MaterialMap();
			MaterialMap.AddRegex(new LibreLancer.Ini.StringKeyValue("^nomad.*$", "NomadMaterialNoBendy"));
			MaterialMap.AddRegex(new LibreLancer.Ini.StringKeyValue("^n-texture.*$", "NomadMaterialNoBendy"));
            FLLog.UIThread = this;
            FLLog.AppendLine = (x,severity) =>
            {
                logText.AppendLine(x);
                if (logText.Length > 16384)
                {
                    logText.Remove(0, logText.Length - 16384);
                }
                logBuffer.SetText(logText.ToString());
                if (severity == LogSeverity.Error)
                {
                    errorTimer = 9;
                    Bell.Play();
                }
            };
            Config = EditorConfiguration.Load();
            logBuffer = new TextBuffer(32768);
            recentFiles = new RecentFilesHandler(OpenFile);
        }
        double errorTimer = 0;
        private int logoTexture;
		protected override void Load()
        {
            DefaultMaterialMap.Init();
			Title = "LancerEdit";
            guiHelper = new ImGuiHelper(this, DpiScale * Config.UiScale);
            guiHelper.PauseWhenUnfocused = Config.PauseWhenUnfocused;
            Audio = new AudioManager(this);
            FileDialog.RegisterParent(this);
            options = new OptionsWindow(this);
            Resources = new GameResourceManager(this);
			Commands = new CommandBuffer();
			Polyline = new PolylineRender(Commands);
			DebugRender = new PhysicsDebugRenderer();
            RenderContext.PushViewport(0, 0, 800, 600);
            Keyboard.KeyDown += Keyboard_KeyDown;
            //TODO: Icon-setting code very messy
            using (var stream = typeof(MainWindow).Assembly.GetManifestResourceStream("LancerEdit.reactor_64.png"))
            {
                var icon = LibreLancer.ImageLib.Generic.BytesFromStream(stream);
                SetWindowIcon(icon.Width, icon.Height, icon.Data);
            }
            using (var stream = typeof(MainWindow).Assembly.GetManifestResourceStream("LancerEdit.reactor_128.png"))
            {
                var icon = (Texture2D)LibreLancer.ImageLib.Generic.FromStream(stream);
                logoTexture = ImGuiHelper.RegisterTexture(icon);
            }
            //Open passed in files!
            if(InitOpenFile != null)
                foreach(var f in InitOpenFile) 
                    OpenFile(f);
            RichText = RenderContext.Renderer2D.CreateRichTextEngine(); 
            Fonts = new FontManager();
            Fonts.ConstructDefaultFonts();
            Services.Add(Fonts);
            Make3dbDlg = new CommodityIconDialog(this);
            LoadScripts();
        }

        void Keyboard_KeyDown(KeyEventArgs e)
        {
            var mods = e.Modifiers;
            mods &= ~KeyModifiers.Numlock;
            mods &= ~KeyModifiers.Capslock;
            if ((mods == KeyModifiers.LeftControl || mods == KeyModifiers.RightControl) && e.Key == Keys.S) {
                if (ActiveTab != null) Save();
            }
            if((mods == KeyModifiers.LeftControl || mods == KeyModifiers.RightControl) && e.Key == Keys.D) {
                if (selected != null) ((EditorTab)selected).OnHotkey(Hotkeys.Deselect);
            }
            if((mods == KeyModifiers.LeftControl || mods == KeyModifiers.RightControl) && e.Key == Keys.R) {
                if (selected != null) ((EditorTab)selected).OnHotkey(Hotkeys.ResetViewport);
            }
            if((mods == KeyModifiers.LeftControl || mods == KeyModifiers.RightControl) && e.Key == Keys.G) {
                if (selected != null) ((EditorTab)selected).OnHotkey(Hotkeys.ToggleGrid);
            }
        }


		bool openAbout = false;
		public List<DockTab> tabs = new List<DockTab>();
		public List<MissingReference> MissingResources = new List<MissingReference>();
		public List<uint> ReferencedMaterials = new List<uint>();
		public List<string> ReferencedTextures = new List<string>();
		public bool ClipboardCopy = true;
		public LUtfNode Clipboard;
		List<DockTab> toAdd = new List<DockTab>();
		public UtfTab ActiveTab;
		double frequency = 0;
		int updateTime = 10;
        public CommodityIconDialog Make3dbDlg;
		public void AddTab(DockTab tab)
		{
			toAdd.Add(tab);
		}
		protected override void Update(double elapsed)
        {
            if (!guiHelper.DoUpdate()) return;
			foreach (var tab in tabs)
				tab.Update(elapsed);
            if (errorTimer > 0) errorTimer -= elapsed;
            Audio.Update();
        }
        public string[] InitOpenFile;
        public void OpenFile(string f)
        {
            if (f != null && System.IO.File.Exists(f) && DetectFileType.Detect(f) == FileType.Utf)
            {
                var t = new UtfTab(this, new EditableUtf(f), System.IO.Path.GetFileName(f));
                recentFiles.FileOpened(f);
                t.FilePath = f;
                ActiveTab = t;
                AddTab(t);
                guiHelper.ResetRenderTimer();
            }
        }
        DockTab selected;
        TextBuffer errorText;
        bool showLog = false;
        float h1 = 200, h2 = 200;
        Vector2 errorWindowSize = Vector2.Zero;
        public double TimeStep;
        private RenderTarget2D lastFrame;
        private bool loadingSpinnerActive = false;
        bool openLoading = false;
        
        public void StartLoadingSpinner()
        {
            QueueUIThread(() =>
            {
                openLoading = true;
                finishLoading = false;
                loadingSpinnerActive = true;
            });
        }

        public void FinishLoadingSpinner()
        {
            QueueUIThread(() =>
            {
                loadingSpinnerActive = false;
                finishLoading = true;
            });
        }

        public List<EditScript> Scripts = new List<EditScript>();

        IEnumerable<string> GetScriptFiles(IEnumerable<string> directories)
        {
            foreach (var dir in directories)
            {
                if(!Directory.Exists(dir)) continue;
                foreach (var f in Directory.GetFiles(dir, "*.cs-script"))
                {
                    yield return f;
                }
            }
        }
        
        private string GetBasePath()
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }

        string GetAssemblyFolder()
        {
            return Path.GetDirectoryName(typeof(MainWindow).Assembly.Location);
        }
        public void LoadScripts()
        {
            Scripts = new List<EditScript>();
            var scriptDirs = new List<string>(2);
            var baseDir = Path.Combine(GetBasePath(), "editorscripts");
            scriptDirs.Add(baseDir);
            var asmDir = Path.Combine(GetAssemblyFolder(), "editorscripts");
            if (asmDir != baseDir) scriptDirs.Add(asmDir);
            foreach (var file in GetScriptFiles(scriptDirs))
            {
                try
                {
                    var sc = new EditScript(file);
                    if(sc.Validate()) Scripts.Add(sc);
                    else FLLog.Error("Scripts", $"Failed to Validate {file}");
                }
                catch (Exception)
                {
                    FLLog.Error("Scripts", $"Failed to Validate {file}");
                }
            }
        }
        
        private List<ScriptRunner> activeScripts = new List<ScriptRunner>();
        public void RunScript(EditScript sc)
        {
            activeScripts.Add(new ScriptRunner(sc, this));
        }
        
		protected override void Draw(double elapsed)
        {
            //Don't process all the imgui stuff when it isn't needed
            if (!loadingSpinnerActive && !guiHelper.DoRender(elapsed))
            {
                if (lastFrame != null) lastFrame.BlitToScreen();
                WaitForEvent(); //Yield like a regular GUI program
                return;
            }
            TimeStep = elapsed;
			RenderContext.ReplaceViewport(0, 0, Width, Height);
			RenderContext.ClearColor = new Color4(0.2f, 0.2f, 0.2f, 1f);
			RenderContext.ClearAll();
			guiHelper.NewFrame(elapsed);
			ImGui.PushFont(ImGuiHelper.Noto);
			ImGui.BeginMainMenuBar();
			if (ImGui.BeginMenu("File"))
            {
                var lst = ImGui.GetWindowDrawList();
				if (Theme.IconMenuItem(Icons.File, "New", true))
				{
					var t = new UtfTab(this, new EditableUtf(), "Untitled");
					ActiveTab = t;
                    AddTab(t);
				}
                if (Theme.IconMenuItem(Icons.Open, "Open", true))
				{
                    var f = FileDialog.Open(UtfFilters);
                    OpenFile(f);
				}

                recentFiles.Menu();
				if (ActiveTab == null)
                {
                    Theme.IconMenuItem(Icons.Save, "Save", false);
                    Theme.IconMenuItem(Icons.Save, "Save As", false);
                }
				else
				{
					if (Theme.IconMenuItem(Icons.Save, string.Format("Save '{0}'", ActiveTab.DocumentName), true))
                    {
                        Save();
                    }
                    if (Theme.IconMenuItem(Icons.Save, "Save As",  true))
                    {
                        SaveAs();
                    }
				}
				if (Theme.IconMenuItem(Icons.Quit, "Quit", true))
				{
					Exit();
				}
				ImGui.EndMenu();
			}
            if (ImGui.BeginMenu("View"))
            {
                Theme.IconMenuToggle(Icons.Log, "Log", ref showLog, true);
                ImGui.EndMenu();
            }
			if (ImGui.BeginMenu("Tools"))
			{
                if(Theme.IconMenuItem(Icons.Cog, "Options",true))
                {
                    options.Show();
                }
               
				if (Theme.IconMenuItem(Icons.Palette, "Resources",true))
				{
					AddTab(new ResourcesTab(this, Resources, MissingResources, ReferencedMaterials, ReferencedTextures));
				}
                if(Theme.IconMenuItem(Icons.FileImport, "Import Model",true))
                {
                    string input;
                    if((input = FileDialog.Open(ImportModelFilters)) != null)
                    {
                        StartLoadingSpinner();
                        new Thread(() =>
                        {
                            SimpleMesh.Model model = null;
                            try
                            {
                                using var stream = File.OpenRead(input);
                                model = SimpleMesh.Model.FromStream(stream).AutoselectRoot(out _).ApplyRootTransforms(false).CalculateBounds();
                                foreach (var x in model.Geometries)
                                {
                                    if (x.Vertices.Length >= 65534) throw new Exception("Too many vertices");
                                    if (x.Indices.Length >= 65534) throw new Exception("Too many indices");
                                }
                                EnsureUIThread(() => FinishImporterLoad(model, System.IO.Path.GetFileName(input)));
                            }
                            catch (Exception ex)
                            {
                                EnsureUIThread(() => ImporterError(ex));
                            }
                        }).Start();
                    }
                }
                if (Theme.IconMenuItem(Icons.SprayCan, "Generate Icon", true))
                {
                    string input;
                    if ((input = FileDialog.Open(ImageFilter)) != null) {
                        Make3dbDlg.Open(input);
                    }
                }
                if(Theme.IconMenuItem(Icons.BookOpen, "Infocard Browser",true))
                {
                    string input;
                    if((input = FileDialog.Open(FreelancerIniFilter)) != null) {
                        AddTab(new InfocardBrowserTab(input, this));
                    }
                }
                if (ImGui.MenuItem("Projectile Viewer"))
                {
                    if(ProjectileViewer.Create(this, out var pj))
                        tabs.Add(pj);
                }

                if (ImGui.MenuItem("ParamCurve Visualiser"))
                {
                    tabs.Add(new ParamCurveVis());
                }
                ImGui.EndMenu();
			}
            if (ImGui.BeginMenu("Window"))
            {
                if (ImGui.MenuItem("Close All Tabs", tabs.Count > 0))
                {
                    Confirm("Are you sure you want to close all tabs?", () =>
                    {
                        selected = null;
                        foreach (var t in tabs) t.Dispose();
                        tabs.Clear();
                    });
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Scripts"))
            {
                if (ImGui.MenuItem("Refresh")) {
                    LoadScripts();
                }
                ImGui.Separator();
                int k = 0;
                foreach (var sc in Scripts)
                {
                    var n = ImGuiExt.IDWithExtra(sc.Info.Name, k++);
                    if (ImGui.MenuItem(n)) {
                        RunScript(sc);
                    }
                }
                ImGui.EndMenu();
            }
			if (ImGui.BeginMenu("Help"))
			{
                if(Theme.IconMenuItem(Icons.Book, "Topics", true))
                {
                    var selfPath = Path.GetDirectoryName(typeof(MainWindow).Assembly.Location);
                    var helpFile = Path.Combine(selfPath, "Docs", "index.html");
                    Shell.OpenCommand(helpFile);
                }
				if (Theme.IconMenuItem(Icons.Info, "About", true))
				{
					openAbout = true;
				}
				ImGui.EndMenu();
			}

            options.Draw();

			if (openAbout)
			{
				ImGui.OpenPopup("About");
				openAbout = false;
			}
            if (openError)
            {
                ImGui.OpenPopup("Error");
                openError = false;
            }

            if (openLoading)
            {
                ImGui.OpenPopup("Processing");
                openLoading = false;
            }

            for (int i = activeScripts.Count - 1; i >= 0; i--)
            {
                if (!activeScripts[i].Draw()) activeScripts.RemoveAt(i);
            }
            bool pOpen = true;

            if (ImGui.BeginPopupModal("Error", ref pOpen, ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text("Error:");
                errorText.InputTextMultiline("##etext", new Vector2(430, 200), ImGuiInputTextFlags.ReadOnly);
                if (ImGui.Button("OK")) ImGui.CloseCurrentPopup();
                ImGui.EndPopup();
            }
            recentFiles.DrawErrors();
            pOpen = true;
			if (ImGui.BeginPopupModal("About", ref pOpen, ImGuiWindowFlags.AlwaysAutoResize))
			{
                ImGui.SameLine(ImGui.GetWindowWidth() / 2 - 64);
                ImGui.Image((IntPtr) logoTexture, new Vector2(128), new Vector2(0, 1), new Vector2(1, 0));
                CenterText(Version);
				CenterText("Callum McGing 2018-2022");
                ImGui.Separator();
                var btnW = ImGui.CalcTextSize("OK").X + ImGui.GetStyle().FramePadding.X * 2;
                ImGui.Dummy(Vector2.One);
                ImGui.SameLine(ImGui.GetWindowWidth() / 2 - (btnW / 2));
				if (ImGui.Button("OK")) ImGui.CloseCurrentPopup();
				ImGui.EndPopup();
			}
            pOpen = true;
            if(ImGuiExt.BeginModalNoClose("Processing", ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGuiExt.Spinner("##spinner", 10, 2, ImGui.GetColorU32(ImGuiCol.ButtonHovered, 1));
                ImGui.SameLine();
                ImGui.Text("Processing");
                if (finishLoading) ImGui.CloseCurrentPopup();
                ImGui.EndPopup();
            }
            //Confirmation
            if (doConfirm)
            {
                ImGui.OpenPopup("Confirm?##mainwindow");
                doConfirm = false;
            }
            pOpen = true;
            if (ImGui.BeginPopupModal("Confirm?##mainwindow", ref pOpen, ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text(confirmText);
                if (ImGui.Button("Yes"))
                {
                    confirmAction();
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("No")) ImGui.CloseCurrentPopup();
                ImGui.EndPopup();
            }
            var menu_height = ImGui.GetWindowSize().Y;
			ImGui.EndMainMenuBar();
			var size = ImGui.GetIO().DisplaySize;
			size.Y -= menu_height;
			//Window
			MissingResources.Clear();
			ReferencedMaterials.Clear();
			ReferencedTextures.Clear();
			foreach (var tab in tabs)
			{
                ((EditorTab)tab).DetectResources(MissingResources, ReferencedMaterials, ReferencedTextures);
			}
            ImGui.SetNextWindowSize(new Vector2(size.X, size.Y - (25 * ImGuiHelper.Scale)), ImGuiCond.Always);
            ImGui.SetNextWindowPos(new Vector2(0, menu_height), ImGuiCond.Always, Vector2.Zero);
            bool childopened = true;
            ImGui.Begin("tabwindow", ref childopened,
                              ImGuiWindowFlags.NoTitleBar |
                              ImGuiWindowFlags.NoSavedSettings |
                              ImGuiWindowFlags.NoBringToFrontOnFocus |
                              ImGuiWindowFlags.NoMove |
                              ImGuiWindowFlags.NoResize);
            TabHandler.TabLabels(tabs, ref selected);
            var totalH = ImGui.GetWindowHeight();
            if (showLog)
            {
                ImGuiExt.SplitterV(2f, ref h1, ref h2, 8, 8, -1);
                h1 = totalH - h2 - 24f * ImGuiHelper.Scale;
                if (tabs.Count > 0) h1 -= 20f * ImGuiHelper.Scale;
                ImGui.BeginChild("###tabcontent" + (selected != null ? selected.RenderTitle : ""),new Vector2(-1,h1),false,ImGuiWindowFlags.None);
            } else
                ImGui.BeginChild("###tabcontent" + (selected != null ? selected.RenderTitle : ""));
            if (selected != null)
            {
                selected.Draw();
                ((EditorTab)selected).SetActiveTab(this);
            }
            else
                ActiveTab = null;
            ImGui.EndChild();
            if(showLog) {
                ImGui.BeginChild("###log", new Vector2(-1, h2), false, ImGuiWindowFlags.None);
                ImGui.Text("Log");
                ImGui.SameLine(ImGui.GetWindowWidth() - 30 * ImGuiHelper.Scale);
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
                if (ImGui.Button(Icons.X.ToString())) showLog = false;
                ImGui.PopStyleVar();
                logBuffer.InputTextMultiline("##logtext", new Vector2(-1, h2 - 28 * ImGuiHelper.Scale), ImGuiInputTextFlags.ReadOnly);
                ImGui.EndChild();
            }
            ImGui.End();
            Make3dbDlg.Draw();
			//Status bar
			ImGui.SetNextWindowSize(new Vector2(size.X, 25f * ImGuiHelper.Scale), ImGuiCond.Always);
			ImGui.SetNextWindowPos(new Vector2(0, size.Y - 6f), ImGuiCond.Always, Vector2.Zero);
			bool sbopened = true;
			ImGui.Begin("statusbar", ref sbopened, 
			                  ImGuiWindowFlags.NoTitleBar | 
			                  ImGuiWindowFlags.NoSavedSettings | 
			                  ImGuiWindowFlags.NoBringToFrontOnFocus | 
			                  ImGuiWindowFlags.NoMove | 
			                  ImGuiWindowFlags.NoResize);
			if (updateTime > 9)
			{
				updateTime = 0;
				frequency = RenderFrequency;
			}
			else { updateTime++; }
			string activename = ActiveTab == null ? "None" : ActiveTab.DocumentName;
			string utfpath = ActiveTab == null ? "None" : ActiveTab.GetUtfPath();
            #if DEBUG
            const string statusFormat = "FPS: {0} | {1} Materials | {2} Textures | Active: {3} - {4}";
            #else
            const string statusFormat = "{1} Materials | {2} Textures | Active: {3} - {4}";
            #endif
			ImGui.Text(string.Format(statusFormat,
									 (int)Math.Round(frequency),
									 Resources.MaterialDictionary.Count,
									 Resources.TextureDictionary.Count,
									 activename,
									 utfpath));
			ImGui.End();
            if(errorTimer > 0) {
                ImGuiExt.ToastText("An error has occurred\nCheck the log for details",
                                   new Color4(21, 21, 22, 128),
                                   Color4.Red);
            }
            ImGui.PopFont();
            if (lastFrame == null ||
                lastFrame.Width != Width ||
                lastFrame.Height != Height)
            {
                if (lastFrame != null) lastFrame.Dispose();
                lastFrame = new RenderTarget2D(Width, Height);
            }
            RenderContext.RenderTarget = lastFrame;
            RenderContext.ClearColor = new Color4(0.2f, 0.2f, 0.2f, 1f);
            RenderContext.ClearAll();
			guiHelper.Render(RenderContext);
            RenderContext.RenderTarget = null;
            lastFrame.BlitToScreen();
            foreach (var tab in toAdd)
            {
                tabs.Add(tab);
                selected = tab;
            }
            toAdd.Clear();
		}
        
        void Save()
        {
            var at = ActiveTab;
            Action save = () =>
            {
                if (!string.IsNullOrEmpty(at.FilePath))
                {
                    string errText = "";
                    if (!at.Utf.Save(at.FilePath, 0, ref errText))
                    {
                        openError = true;
                        if (errorText == null) errorText = new TextBuffer();
                        errorText.SetText(errText);
                    }
                }
                else
                    RunSaveDialog(at);
            };
            if (at.DirtyCountHp > 0 || at.DirtyCountPart > 0)
            {
                Confirm("This model has unapplied changes. Continue?", save);
            }
            else
                save();
        }

        void SaveAs()
        {
            var at = ActiveTab;
            Action save = () =>  RunSaveDialog(at);
            if (at.DirtyCountHp > 0 || at.DirtyCountPart > 0)
            {
                Confirm("This model has unapplied changes. Continue?", save);
            }
            else
                save();
        }

        void RunSaveDialog(UtfTab at)
        {
            var f = FileDialog.Save(UtfFilters);
            if (f != null)
            {
                string errText = "";
                if (!at.Utf.Save(f, 0, ref errText))
                {
                    openError = true;
                    if (errorText == null) errorText = new TextBuffer();
                    errorText.SetText(errText);
                }
                else
                {
                    at.DocumentName = System.IO.Path.GetFileName(f);
                    at.UpdateTitle();
                    at.FilePath = f;
                }
            }
        }
         

        string confirmText;
        bool doConfirm = false;
        Action confirmAction;

        void Confirm(string text, Action action)
        {
            doConfirm = true;
            confirmAction = action;
            confirmText = text;
        }

        void CenterText(string text)
        {
            ImGui.Dummy(new Vector2(1));
            var win = ImGui.GetWindowWidth();
            var txt = ImGui.CalcTextSize(text).X;
            ImGui.SameLine(Math.Max((win / 2f) - (txt / 2f),0));
            ImGui.Text(text);
        }
        void FinishImporterLoad(SimpleMesh.Model model, string tabName)
        {
           FinishLoadingSpinner();
            AddTab(new ImportModelTab(model, tabName, this));
        }
        void ImporterError(Exception ex)          
        {
            FinishLoadingSpinner();
            ErrorDialog("Import Error:\n" + ex.Message + "\n" + ex.StackTrace);
        }

        public void ErrorDialog(string text)
        {
            errorText?.Dispose();
            errorText = new TextBuffer();
            errorText.SetText(text);
            openError = true;
        }
        protected override void OnDrop(string file)
        {
            if (DetectFileType.Detect(file) == FileType.Utf)
            {
                var t = new UtfTab(this, new EditableUtf(file), System.IO.Path.GetFileName(file));
                ActiveTab = t;
                AddTab(t);
            }
        }
        protected override void Cleanup()
		{
			Audio.Dispose();
		}
	}
}
