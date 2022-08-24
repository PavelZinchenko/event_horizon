using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(ListScrollRect))]
public class ListScrollRectEditor : Editor
{
	private string[] paddingLabels = { "Left", "Right", "Top", "Bottom" };

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.Space();

		DrawListScrollRect();

		EditorGUILayout.Space();

		DrawDefaultScrollRect();
		
		EditorGUILayout.Space();

		serializedObject.ApplyModifiedProperties();
	}
	
	private void DrawListScrollRect()
	{
		if (UnityEngine.Application.isPlaying)
		{
			EditorGUILayout.HelpBox("Values not editable in play mode. If you would like to make changes to them while the game is running use the RebuildContent or RefreshContent method.", MessageType.Info);
			UnityEngine.GUI.enabled = false;
		}

		EditorGUILayout.PropertyField(serializedObject.FindProperty("initializeOnStart"), new GUIContent("Initialize On Start", "If true, ListScrollRect will call Initialize in the Start method."));

		DrawContentFillerInterface();
		
		EditorGUILayout.PropertyField(serializedObject.FindProperty("scrollDir"), new GUIContent("Scroll Direction", "The direction that the list will scroll."));

		EditorGUILayout.PropertyField(serializedObject.FindProperty("spacing"), new GUIContent("Spacing", "The amount of space in between each item in the list."));

		DrawPadding();

		UnityEngine.GUI.enabled = true;
	}

	private void DrawContentFillerInterface()
	{
		SerializedProperty	contentFillerInterfaceProperty	= serializedObject.FindProperty("contentFillerInterface");
		GameObject			contentFillerInterface			= contentFillerInterfaceProperty.objectReferenceValue as GameObject;
		
		EditorGUILayout.PropertyField(contentFillerInterfaceProperty, new GUIContent("Content Filler Interface", "The GameObject that has the component that implements the IContentFiller interface."));
		
		if (contentFillerInterface != null && contentFillerInterface.GetComponent(typeof(IContentFiller)) == null)
		{
			EditorGUILayout.HelpBox("GameObject does not have an attached component that implements the IContentFiller interface.", MessageType.Error);
		}
	}

	private void DrawPadding()
	{
		ListScrollRect		listScrollRect		= target as ListScrollRect;
		SerializedProperty	paddingProperty		= serializedObject.FindProperty("padding");
		SerializedProperty	editorShowPadding	= serializedObject.FindProperty("editorShowPadding");

		editorShowPadding.boolValue = EditorGUILayout.Foldout(editorShowPadding.boolValue, new GUIContent("Padding", "The padding that will be used in the list."));

		if (editorShowPadding.boolValue)
		{
			EditorGUI.indentLevel++;

			int start	= (listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Vertical) ? 2 : 0;
			int end		= (listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Vertical) ? 4 : 2;

			for (int i = start; i < end; i++)
			{

				paddingProperty.GetArrayElementAtIndex(i).floatValue = EditorGUILayout.FloatField(paddingLabels[i], paddingProperty.GetArrayElementAtIndex(i).floatValue);
			}
			
			EditorGUI.indentLevel--;
		}
	}

	private void DrawDefaultScrollRect()
	{
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Content"));

		ListScrollRect		listScrollRect		= target as ListScrollRect;
		SerializedProperty	horizontalProperty	= serializedObject.FindProperty("m_Horizontal");
		SerializedProperty	verticalProperty	= serializedObject.FindProperty("m_Vertical");

		if (listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Horizontal)
		{
			horizontalProperty.boolValue = true;
		}

		if (listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Vertical)
		{
			verticalProperty.boolValue = true;
		}

		GUI.enabled = (listScrollRect.ScrollDir != ListScrollRect.ScrollDirection.Horizontal);
		EditorGUILayout.PropertyField(horizontalProperty);
		GUI.enabled = (listScrollRect.ScrollDir != ListScrollRect.ScrollDirection.Vertical);
		EditorGUILayout.PropertyField(verticalProperty);
		GUI.enabled = true;

		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Elasticity"));
		EditorGUI.indentLevel = 0;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Inertia"));
		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_DecelerationRate"));
		EditorGUI.indentLevel = 0;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ScrollSensitivity"));
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Viewport"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HorizontalScrollbar"));
		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HorizontalScrollbarVisibility"), new GUIContent("Visibility"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HorizontalScrollbarSpacing"), new GUIContent("Spacing"));
		EditorGUI.indentLevel = 0;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_VerticalScrollbar"));
		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_VerticalScrollbarVisibility"), new GUIContent("Visibility"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_VerticalScrollbarSpacing"), new GUIContent("Spacing"));
		EditorGUI.indentLevel = 0;
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnValueChanged"));
	}
}
