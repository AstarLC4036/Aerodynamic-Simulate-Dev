using System.Collections;
using UnityEditor;
using UnityEngine;
using Flight;
using System;
using BA.Flight;
using System.Diagnostics;
using BA.Utility;
using UnityEditor.AnimatedValues;

namespace BA.Editors
{
    [CustomEditor(typeof(AeroSurface))]
    public class AeroSurfaceEditor : Editor
    {
        AnimBool fadeBool = new AnimBool(false);

        bool foldout = true;

        Editor subEditor;

        public void OnEnable()
        {
            fadeBool.speed = 1.5f;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            AeroSurface surface = (AeroSurface)target;
            AeroSurfaceData data = ((AeroSurface)target).surfaceData;

            EditorGUILayout.Space(15);

            fadeBool.target = surface.isControlSurface;
            EditorGUILayout.BeginFadeGroup(fadeBool.faded);
            if (fadeBool.faded != 0)
            {
                EditorGUILayout.LabelField("Control Surface Settings");
                EditorGUILayout.Space(5);

                //Dropdown(enum: ControlSurfaceType)
                surface.surfaceType = (ControlSurfaceType)EditorGUILayout.EnumPopup("Control Surface Type", surface.surfaceType);

                //Toggle(bool: invertInput)
                surface.invertInput = EditorGUILayout.Toggle("Invert Input", surface.invertInput);

                //Object(Transform: controlSurface)
                surface.flapTransform = (Transform)EditorGUILayout.ObjectField("Flap Transform", surface.flapTransform, typeof(Transform), true);
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.Space(10);

            if (data != null)
            {
                if (subEditor == null)
                    subEditor = CreateEditor(data);

                foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, new GUIContent("Aerodynamics Surface Data"));
                if (foldout)
                    subEditor.OnInspectorGUI();
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Refresh"))
            {
                ((AeroSurface)target).UpdateData();
            }

            if(!(fadeBool.faded == 0 || fadeBool.faded == 1))
            {
                Repaint();
            }
        }
    }
}