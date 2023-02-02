﻿using UnityEditor;
using UnityEngine;
using XCharts.Runtime;

namespace XCharts.Editor
{
    [CustomPropertyDrawer(typeof(TitleStyle), true)]
    public class TitleStyleDrawer : BasePropertyDrawer
    {
        public override string ClassName { get { return "TitleStyle"; } }
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            base.OnGUI(pos, prop, label);
            if (MakeComponentFoldout(prop, "m_Show", true))
            {
                ++EditorGUI.indentLevel;
                PropertyField(prop, "m_OffsetCenter");
                PropertyField(prop, "m_TextStyle");
                --EditorGUI.indentLevel;
            }
        }
    }
}