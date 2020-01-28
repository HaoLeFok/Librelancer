// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.IO;
using ImGuiNET;
using LibreLancer;
using LibreLancer.ImUI;
using LibreLancer.Interface;

namespace InterfaceEdit
{
    public class StylesheetEditor : SaveableTab
    {
        private Stylesheet currentStylesheet;
        private UiContext uiContext;
        private string path;
        private ColorTextEdit textEditor;
        private bool validXml = false;
        private string exceptionText = "Error: Nothing typed yet";
        public StylesheetEditor(string xmlFolder, UiContext context)
        {
            Title = "Stylesheet";
            textEditor = new ColorTextEdit();
            uiContext = context;
            path = Path.Combine(xmlFolder, "stylesheet.xml");
            textEditor.SetText(File.ReadAllText(path));
            TextChanged();
        }

        public override void Save()
        {
            if (validXml)
            {
                File.WriteAllText(path, textEditor.GetText());
                uiContext.Stylesheet = currentStylesheet;
            }
            else
            {
                Bell.Play();
            }
        }

        public override void Draw()
        {
            if (!validXml) {
                ImGui.TextColored(new Vector4(1,0,0,1), exceptionText);
            }
            textEditor.Render("##stylesheeteditor");
            if (textEditor.TextChanged()) TextChanged();
        }

        void TextChanged()
        {
            try
            {
                var text = textEditor.GetText();
                if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                {
                    validXml = false;
                    exceptionText = "Error: Nothing typed yet";
                    return;
                }
                currentStylesheet = (Stylesheet) uiContext.XmlLoader.FromString(text, null);
                validXml = true;
            }
            catch (Exception e)
            {
                validXml = false;
                if (e is NullReferenceException)
                    exceptionText = $"Error: {e.Message}\n{e.StackTrace}";
                else
                    exceptionText = $"Error: {e.Message}";
            }
        }

    }
}