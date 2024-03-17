using FIMSpace.FEditor;
using FIMSpace.FTools;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace FIMSpace.AnimationTools
{
    [System.Serializable]
    public class ADClipSettings_Morphing : IADSettings
    {
        public AnimationClip settingsForClip;
        public List<MorphingSet> LimbBlendings = new List<MorphingSet>();

        public ADClipSettings_Morphing() { }

        /// <summary> Used for identifying different variants when modifying the same AnimationClip multiple times </summary>
        [SerializeField, HideInInspector] private string setId = "";
        [SerializeField, HideInInspector] private int setIdHash = 0;
        public void AssignID(string name) { setId = name; setIdHash = setId.GetHashCode(); }
        public string SetID { get { return setId; } }
        public int SetIDHash { get { return setIdHash; } }
        public AnimationClip SettingsForClip { get { return settingsForClip; } }
        public void OnConstructed(AnimationClip clip, int hash) { settingsForClip = clip; setIdHash = hash; }


        public void RefreshWithSetup(AnimationDesignerSave save) { }


        internal static void PasteValuesTo(ADClipSettings_Morphing from, ADClipSettings_Morphing to)
        {
            for (int i = 0; i < from.LimbBlendings.Count; i++)
                if (from.LimbBlendings[i].Index == to.LimbBlendings[i].Index)
                    MorphingSet.PasteValuesTo(from.LimbBlendings[i], to.LimbBlendings[i]);
        }

        [System.Serializable]
        public class MorphingSet
        {
            public int Index;
            public string ID;

            public bool Enabled = false;
            public float Blend = 1f;
            public AnimationCurve BlendEvaluation = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);

            public float TestBlend = 1f;

            public static MorphingSet CopyingFrom = null;

            public static void PasteValuesTo(MorphingSet from, MorphingSet to)
            {
                Keyframe[] evalKeys = new Keyframe[from.BlendEvaluation.keys.Length];
                from.BlendEvaluation.keys.CopyTo(evalKeys, 0);
                to.BlendEvaluation = new AnimationCurve(evalKeys);
                to.Blend = from.Blend;
                to.Enabled = from.Enabled;
            }

            public MorphingSet(bool enabled = false, string id = "", int index = -1, float blend = 1f)
            {
                Enabled = enabled;
                Index = index;
                ID = id;
                Blend = blend;
                //BlendEvaluation = AnimationDesignerWindow.GetExampleCurve(0f, 1f, 0.3f, 1f, 0.3f);
            }

            internal void DrawTopGUI(string title = "")
            {
                EditorGUILayout.BeginHorizontal();

                Enabled = EditorGUILayout.Toggle(Enabled, GUILayout.Width(24));

                if (!string.IsNullOrEmpty(title))
                {
                    EditorGUILayout.LabelField(title + " : Settings", FGUI_Resources.HeaderStyle);
                }

                #region Copy Paste Buttons

                if (CopyingFrom != null)
                {
                    if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("Clipboard").image, "Paste copied values"), FGUI_Resources.ButtonStyle, GUILayout.Width(22), GUILayout.Height(19)))
                    {
                        PasteValuesTo(CopyingFrom, this);
                    }
                }

                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Duplicate").image, "Copy morphing parameters values below to paste them into other limb"), FGUI_Resources.ButtonStyle, GUILayout.Width(22), GUILayout.Height(19)))
                {
                    CopyingFrom = this;
                }

                #endregion

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);

                if (!Enabled) GUI.enabled = false;

                Blend = EditorGUILayout.Slider("Full Blend:", Blend, 0f, 1f);
                AnimationDesignerWindow.DrawCurve(ref BlendEvaluation, "Blend Along Clip Time");


                GUI.enabled = true;
            }

            internal void DrawParamsGUI(float optionalBlendGhost)
            {
                if (!Enabled) GUI.enabled = false;
                Color preC = GUI.color;

                AnimationDesignerWindow.GUIDrawFloatPercentage(ref TestBlend, new GUIContent("  TestBlend", FGUI_Resources.Tex_Rotation));

                #region Value ghost

                if (TestBlend > 0f)
                    if (optionalBlendGhost > 0f)
                    {
                        Rect r = GUILayoutUtility.GetLastRect();
                        r.position += new Vector2(188, 0);
                        r.width -= 188 + 56;

                        GUI.color = new Color(1f, 1f, 1f, 0.055f);
                        GUI.HorizontalSlider(r, TestBlend * optionalBlendGhost, 0f, 1f);
                        GUI.color = preC;
                    }

                #endregion

                GUI.enabled = true;
            }

        }

    }
}