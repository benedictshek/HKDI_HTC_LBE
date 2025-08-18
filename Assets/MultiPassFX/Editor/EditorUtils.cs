using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MultiPassFX
{
    public class SavedBool
    {
        private string key;
        public bool value
        {
            get
            {
                return EditorPrefs.GetBool(key);
            }
            set
            {
                EditorPrefs.SetBool(key, value);
            }
        }

        public SavedBool(string k, bool v)
        {
            key = k;
            value = v;
        }
    }

    public static class EditorUtils
    {
        private static GUIStyle _foldoutHeaderGroupStyle;

        public static GUIStyle FoldoutHeaderGroupStyle
        {
            get
            {
                if (_foldoutHeaderGroupStyle == null)
                {
                    _foldoutHeaderGroupStyle = new GUIStyle("ShurikenModuleTitle");
                    _foldoutHeaderGroupStyle.border = new RectOffset(15, 7, 4, 4);
                    _foldoutHeaderGroupStyle.font = EditorStyles.boldLabel.font;
                    _foldoutHeaderGroupStyle.fontStyle = EditorStyles.boldLabel.fontStyle;
                    _foldoutHeaderGroupStyle.fontSize = EditorStyles.boldLabel.fontSize + 3;
                    _foldoutHeaderGroupStyle.fixedHeight = 30;
                }

                return _foldoutHeaderGroupStyle;
            }
        }

        public static bool BeginFoldoutHeaderGroup(SavedBool foldout, string title)
        {
            GUILayoutUtility.GetRect(0, 3, FoldoutHeaderGroupStyle);
            var res =  EditorGUILayout.BeginFoldoutHeaderGroup(foldout.value, title, FoldoutHeaderGroupStyle);
            if (!res)
            {
                EditorGUILayout.Space();
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            foldout.value = res;
            return res;
        }

        public static MaterialProperty FindMaterialProperty(string propertyName, MaterialProperty[] properties, bool propertyIsMandatory = false)
        {
            for (int index = 0; index < properties.Length; ++index)
            {
                if (properties[index] != null && properties[index].name == propertyName)
                    return properties[index];
            }

            if (propertyIsMandatory)
                throw new ArgumentException("Could not find MaterialProperty: '" + propertyName + "', Num properties: " + properties.Length);

            return null;
        }
    }
}