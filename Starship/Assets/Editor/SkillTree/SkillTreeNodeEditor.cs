using UnityEngine;
using UnityEditor;
using ViewModel.Skills;

[CustomEditor(typeof(SkillTreeNode))]
[CanEditMultipleObjects]
public class SkillTreeNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(16);
        GUILayout.BeginHorizontal();

        if (targets.Length > 1)
        {
            if (GUILayout.Button("Connect"))
            {
                for (var i = 0; i + 1 < targets.Length; ++i)
                {
                    for (var j = i + 1; j < targets.Length; ++j)
                    {
                        ((SkillTreeNode)targets[i]).AddLink((SkillTreeNode)targets[j]);
                        ((SkillTreeNode)targets[j]).AddLink((SkillTreeNode)targets[i]);
                        EditorUtility.SetDirty(targets[i]);
                        EditorUtility.SetDirty(targets[j]);
                    }
                }
            }
            if (GUILayout.Button("Disconnect"))
            {
                for (var i = 0; i + 1 < targets.Length; ++i)
                {
                    for (var j = i + 1; j < targets.Length; ++j)
                    {
                        ((SkillTreeNode)targets[i]).RemoveLink((SkillTreeNode)targets[j]);
                        ((SkillTreeNode)targets[j]).RemoveLink((SkillTreeNode)targets[i]);
                        EditorUtility.SetDirty(targets[i]);
                        EditorUtility.SetDirty(targets[j]);
                    }
                }
            }
        }

        if (GUILayout.Button("Clear"))
        {
            foreach (var item in targets)
            {
                var node = (SkillTreeNode)item;
                foreach (var child in node.LinkedNodes)
                {
                    var linkedNode = (SkillTreeNode)child;
                    if (linkedNode == null)
                        continue;

                    linkedNode.RemoveLink(node);
                    EditorUtility.SetDirty(linkedNode);
                }

                node.ClearLinks();
                EditorUtility.SetDirty(item);
            }
        }

        if (GUILayout.Button("Refresh"))
        {
            EditorUtility.SetDirty(target);
            ((SkillTreeNode)target).SendMessage("OnValidate", SendMessageOptions.DontRequireReceiver);
        }

        GUILayout.EndHorizontal();
    }

    private void OnSceneGUI()
    {
        var node = target as SkillTreeNode;
        foreach (var item in node.LinkedNodes)
            DrawLine(node, item);
    }

    private static void DrawLine(SkillTreeNode first, SkillTreeNode second)
    {
        if (first == null || second == null) return;

        var begin = first.transform.position;
        var end = second.transform.position;
        Handles.DrawDottedLine(begin, end, 1f);
    }
}
