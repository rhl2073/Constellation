﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Constellation;
using ConstellationEditor;

public class ConstellationEditorWindowV2 : EditorWindow, ILoadable, IUndoable, ICopyable, ICompilable
{
    public NodeWindow NodeWindow;
    public NodeSelectorPanel NodeSelector;
    public ConstellationsTabPanel NodeTabPanel;
    private const string editorPath = "Assets/Constellation/Editor/EditorAssets/";
    public NodesFactory NodesFactory;
    public ConstellationEditorDataService ScriptDataService;
    public ConstellationScript ConstellationScript;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/My Window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ConstellationEditorWindowV2 window = (ConstellationEditorWindowV2)EditorWindow.GetWindow(typeof(ConstellationEditorWindowV2));
        window.Show();
    }

    public void OnEnable()
    {
        if (NodeSelector == null)
        {
            NodeSelector = new NodeSelectorPanel(/*, scriptDataService.GetAllCustomNodesNames()*/);
            NodeSelector.SetupNamespaceData();
        }

        if (ScriptDataService == null)
        {
            ScriptDataService = new ConstellationEditorDataService();
            ScriptDataService.Initialize();
        }
            
        if(NodeTabPanel == null)
            NodeTabPanel = new ConstellationsTabPanel();
    }


    void NodeAdded(string _nodeName, string _namespace)
    {
        NodeWindow.AddNode(_nodeName, _namespace);
    }

    void OnGUI()
    {
        if (!IsConstellationSelected())
        {
            StartPanel.Draw(this);
            return;
        } else
        {
            TopBarPanel.Draw(this, this, this, this);
            var constellationName = NodeTabPanel.Draw(ScriptDataService.currentPath.ToArray(), null);
            if (constellationName != null)
                Open(constellationName);
            var constellationToRemove = NodeTabPanel.ConstellationToRemove();
            ScriptDataService.CloseOpenedConstellation(constellationToRemove);
            EditorGUILayout.BeginHorizontal();
            NodeWindow.Draw(RequestRepaint, position.width, position.height);
            NodeSelector.Draw(300, position.height, NodeAdded);
            EditorGUILayout.EndHorizontal();
        }
    }

    void RequestRepaint()
    {
        Repaint();
    }

    public void Open(string _path)
    {
        ConstellationScript = ScriptDataService.OpenConstellation(_path);
        NodeWindow = new NodeWindow(editorPath, ConstellationScript);
    }

    public void Save()
    {
        ScriptDataService.Save();
    }

    public void New()
    {
        ConstellationScript = ScriptDataService.New();
        NodeWindow = new NodeWindow(editorPath, ConstellationScript);
    }

    protected bool IsConstellationSelected()
    {
        if (ScriptDataService != null)
        {
            if (ScriptDataService.GetCurrentScript() != null)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public void AddAction()
    {
        throw new System.NotImplementedException();
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }

    public void Redo()
    {
        throw new System.NotImplementedException();
    }

    public void Copy()
    {
        throw new System.NotImplementedException();
    }

    public void Paste()
    {
        throw new System.NotImplementedException();
    }

    public void Cut()
    {
        throw new System.NotImplementedException();
    }

    public void ParseScript()
    {
        throw new System.NotImplementedException();
    }
}