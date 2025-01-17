// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using LibreLancer;
using LibreLancer.ImUI;

namespace LancerEdit
{
    public class ScriptRunner
    {
        public class ScriptArgumentInstance
        {
            public EditScriptArgument Argument;
            public TextBuffer InputText = new TextBuffer();
            public List<string> StringArray = new List<string>();
            public bool BooleanValue = false;
            public int IntegerValue = 0;

            public string GetValue()
            {
                if (Argument.Type == ScriptArgumentType.Integer) return IntegerValue.ToString();
                if (Argument.Type == ScriptArgumentType.Boolean) return BooleanValue.ToString();
                if (Argument.Type == ScriptArgumentType.Dropdown)
                    return Argument.Options[IntegerValue];
                if (Argument.Type == ScriptArgumentType.FileArray)
                    return string.Join("\n", StringArray);
                return InputText.GetText().Trim();
            }

            public void Draw(int i)
            {
                ImGui.PushID($"__argument_{i}");
                switch (Argument.Type)
                {
                    case ScriptArgumentType.Boolean:
                        ImGui.Checkbox(Argument.Name, ref BooleanValue);
                        break;
                    case ScriptArgumentType.Integer:
                        ImGui.InputInt(Argument.Name, ref IntegerValue, 1);
                        break;
                    case ScriptArgumentType.String:
                        InputText.InputText(Argument.Name, ImGuiInputTextFlags.None, 100);
                        break;
                    case ScriptArgumentType.Dropdown:
                        ImGui.Combo(Argument.Name, ref IntegerValue, Argument.Options.ToArray(),
                            Argument.Options.Count);
                        break;
                    case ScriptArgumentType.Folder:
                        InputText.InputText(Argument.Name, ImGuiInputTextFlags.None, 100);
                        ImGui.SameLine();
                        if (ImGui.Button(".."))
                        {
                            var result = FileDialog.ChooseFolder();
                            if(!string.IsNullOrEmpty(result)) InputText.SetText(result);
                        }
                        break;
                    case ScriptArgumentType.File:
                        InputText.InputText(Argument.Name, ImGuiInputTextFlags.None, 100);
                        ImGui.SameLine();
                        if (ImGui.Button(".."))
                        {
                            var result = FileDialog.Open();
                            if(!string.IsNullOrEmpty(result)) InputText.SetText(result);
                        }
                        break;
                    case ScriptArgumentType.FileArray:
                        ImGui.Text(Argument.Name);
                        ImGui.Separator();
                        if (StringArray.Count == 0)
                        {
                            ImGui.Text("[empty]");
                        }
                        else
                        {
                            ImGui.Columns(2);
                            List<string> toRemove = new List<string>();
                            for (int j = 0; j < StringArray.Count; j++)
                            {
                                ImGui.Text(StringArray[j]);
                                ImGui.NextColumn();
                                if (ImGui.Button($"Remove##{j}"))
                                {
                                    toRemove.Add(StringArray[j]);
                                }
                                ImGui.NextColumn();
                            }
                            foreach (var f in toRemove) StringArray.Remove(f);
                            ImGui.Columns(1);
                        }

                        ImGui.Separator();
                        if (ImGui.Button("Add File"))
                        {
                            var result = FileDialog.Open();
                            if(!string.IsNullOrEmpty(result)) StringArray.Add(result);
                        }
                       
                        break;
                }
                ImGui.PopID();
            }
        }

        private static int _unique;

        private int unique = 0;
        
        private EditScript script;
        private List<ScriptArgumentInstance> arguments = new List<ScriptArgumentInstance>();
        private MainWindow main;
        
        public ScriptRunner(EditScript script, MainWindow main)
        {
            this.script = script;
            unique = _unique++;
            arguments = script.Arguments.Select(x => new ScriptArgumentInstance() { Argument = x }).ToList();
            this.main = main;
        }
       
        private string GetBasePath()
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }

        private bool running = false;
        private bool doUpdate = false;
        private List<string> lines = new List<string>();
        void Invoke()
        {
            #if DEBUG
            var lleditscript = Path.Combine(GetBasePath(), "../../../../lleditscript/bin/Debug/net5.0/lleditscript");
            #else
            var lleditscript = Path.Combine(GetBasePath(), "lleditscript");
            #endif
            if (Platform.RunningOS == OS.Windows) lleditscript += ".exe";
            var args = $"--args-stdin \"{script.Filename}\"";
            var pi = new ProcessStartInfo(lleditscript, args);
            pi.UseShellExecute = false;
            pi.RedirectStandardInput = true;
            pi.RedirectStandardOutput = true;
            pi.RedirectStandardError = true;
            var proc = Process.Start(pi);
            proc.EnableRaisingEvents = true;
            doUpdate = true;
            proc.OutputDataReceived += (sender, eventArgs) =>
            {
                main.QueueUIThread(() => lines.Add(eventArgs.Data ?? ""));
            };
            proc.ErrorDataReceived += (sender, eventArgs) =>
            {
                main.QueueUIThread(() => lines.Add(eventArgs.Data ?? ""));
            };
            proc.Exited += (sender, eventArgs) =>
            {
                main.QueueUIThread(() =>
                {
                    doUpdate = false;
                });
            };
            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();
            if (arguments.Count > 0)
            {
                proc.StandardInput.Write(arguments[0].GetValue());
                for (int i = 1; i < arguments.Count; i++) {
                    proc.StandardInput.WriteLine();
                    proc.StandardInput.Write(arguments[i].GetValue());
                }
                proc.StandardInput.Close();
            }
            running = true;
        }

        private bool isOpen = true;
        private int lastCount = 0;
        public bool Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(500,400), ImGuiCond.FirstUseEver);
            if (ImGui.Begin($"Script##{unique}", ref isOpen))
            {
                if(doUpdate) ImGuiHelper.AnimatingElement(); //Stops sleeping when running task on background thread
                ImGui.Text(script.Info.Name);
                ImGui.Separator();
                if (running)
                {
                    if (doUpdate)
                    {
                        string text = "Running.";
                        var t = main.TotalTime - Math.Truncate(main.TotalTime);
                        if (t > 0.66)
                            text += "..";
                        else if (t > 0.33)
                            text += ".";
                        ImGui.Text(text);
                    }
                    else
                    {
                        ImGui.Text("Finished");
                    }
                    ImGui.BeginChild($"##SCRIPTlog{unique}");
                    ImGui.PushFont(ImGuiHelper.SystemMonospace);
                    foreach (var line in lines)
                        ImGui.TextWrapped(line);
                    if(lines.Count != lastCount && ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                        ImGui.SetScrollHereY(1.0f);
                    lastCount = lines.Count;
                    ImGui.PopFont();
                    ImGui.EndChild();
                }
                else
                {
                    for(int i =0; i < arguments.Count; i++)
                        arguments[i].Draw(i);
                    ImGui.Separator();
                    if (ImGui.Button("Run"))
                    {
                        Invoke();
                    }
                }
                ImGui.End();
            }
            return isOpen;
        }
    }
}