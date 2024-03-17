using FIMSpace.FEditor;
using FIMSpace.Generating;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.AnimationTools
{



    public partial class AnimationDesignerWindow : EditorWindow
    {

        #region GUI Related

        int _sel_elasticLimb = -1;
        public static ADClipSettings_Elasticness ElasticSettingsCopyFrom = null;
        bool displayElasticnessBonesBlending = true;
        bool drawAllElasticnesEval = false;
        void DrawElasticnessTab()
        {
            if (isReady == false) { EditorGUILayout.HelpBox("First prepare Armature", MessageType.Info); return; }

            if (TargetClip == null)
            {
                EditorGUILayout.HelpBox("Animation Clip is required", MessageType.Info);
                return;
            }

            //EditorGUI.BeginChangeCheck();

            ADClipSettings_Elasticness setup = S.GetSetupForClip(S.ElasticnessSetupsForClips, TargetClip, _toSet_SetSwitchToHash);

            #region Copy Paste Buttons

            GUILayout.Space(6);

            EditorGUILayout.BeginHorizontal();

            _anim_MainSet.TurnOnElasticness = EditorGUILayout.Toggle(_anim_MainSet.TurnOnElasticness, GUILayout.Width(24));

            if (_anim_MainSet.TurnOnElasticness == false) GUI.enabled = false;

            if (DrawTargetClipField(FGUI_Resources.GetFoldSimbol(drawAllElasticnesEval, true) + "  Elasticness Set For:", true)) drawAllElasticnesEval = !drawAllElasticnesEval;

            if (ElasticSettingsCopyFrom != null)
            {
                if (ElasticSettingsCopyFrom.LimbsSets.Count == setup.LimbsSets.Count)
                    if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("Clipboard").image, "Paste copied values to all limbs of the animation clip"), FGUI_Resources.ButtonStyle, GUILayout.Width(22), GUILayout.Height(19)))
                    {
                        ADClipSettings_Elasticness.PasteValuesTo(ElasticSettingsCopyFrom, setup);
                        DisplaySave._SetDirty();
                        ElasticSettingsCopyFrom = null;
                    }
            }

            GUILayout.Space(2);

            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Duplicate").image, "Copy elasticness parameters values from all limbs below to paste them into other animation clip limbs settings"), FGUI_Resources.ButtonStyle, GUILayout.Width(22), GUILayout.Height(19)))
            {
                ElasticSettingsCopyFrom = setup;
                DisplaySave._SetDirty();
            }

            #endregion

            EditorGUILayout.EndHorizontal();


            if (drawAllElasticnesEval)
                AnimationDesignerWindow.DrawCurve(ref _anim_MainSet.ElasticnessEvaluation, "All Elasticness Blend:");


            FGUI_Inspector.DrawUILine(0.3f, 0.5f, 1, 14, 0.975f);

            _LimbsRefresh();

            //if (sectionFocusMode)
            //{
            //    GUILayout.Space(5);
            //    EditorGUILayout.LabelField(new GUIContent("Limbs Elasticness", "Add elasticness effect to the limbs of animated character"), FGUI_Resources.HeaderStyleBig);
            //    GUILayout.Space(3);
            //}

            GUILayout.Space(5);

            if (_anim_MainSet.TurnOnElasticness == false)
            {
                EditorGUILayout.HelpBox("Elasticness Turned Off", MessageType.None);
                GUILayout.Space(5);
            }
            else
            {

                #region Refresh Coloring

                for (int i = 0; i < S.Limbs.Count; i++)
                {
                    var settings = setup.GetElasticnessSettingsForLimb(S.Limbs[i], S);

                    if (settings.Enabled == false)
                        S.Limbs[i].GizmosBlend = 0.0f;
                    else
                        S.Limbs[i].GizmosBlend = settings.Blend;
                }

                #endregion

                DrawSelectorGUI(S.Limbs, ref _sel_elasticLimb, 18, position.width - 22);


                if (_sel_elasticLimb > -1 && Limbs.ContainsIndex(_sel_elasticLimb, true))
                {
                    var selectedLimb = Limbs[_sel_elasticLimb];
                    var settings = setup.GetElasticnessSettingsForLimb(selectedLimb, S);
                    GUILayout.Space(5);

                    EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

                    DrawNullTweakGUI(selectedLimb, _sel_elasticLimb);

                    settings.DrawTopGUI(selectedLimb.GetName);


                    #region Curve progress bar

                    float ghostMul = 0f;

                    // Value Ghost
                    if (drawModsGizmos)
                    {
                        Rect r = DrawCurveProgress(animationProgress, 152f, 60f);

                        ghostMul = settings.Blend * settings.BlendEvaluation.Evaluate(animationProgress);
                        DrawSliderProgress(ghostMul, 150f, 58f, r, 2, 1);

                    }

                    #endregion


                    GUILayout.Space(6);
                    FGUI_Inspector.DrawUILine(0.5f, 0.3f, 1, 20, 0.975f);

                    settings.DrawParamsGUI(ghostMul);

                    GUILayout.Space(8);
                    FGUI_Inspector.DrawUILine(0.5f, 0.3f, 1, 18, 0.975f);


                    #region Individual Limb Bones Blendings


                    float narr = position.width * 0.06f;
                    if (position.width < 420) narr = 0f;

                    //GUILayout.Space(-4);

                    string ff = FGUI_Resources.GetFoldSimbol(displayElasticnessBonesBlending, false);

                    if (GUILayout.Button(ff + "   Individual Limb Bones Blending   " + ff, FGUI_Resources.HeaderStyle))
                    {
                        displayElasticnessBonesBlending = !displayElasticnessBonesBlending;
                    }

                    if (displayElasticnessBonesBlending)
                    {
                        GUILayout.Space(4);
                        EditorGUILayout.LabelField("Limb Bones Blends Below Are Global For All Animation Clips", EditorStyles.centeredGreyMiniLabel);
                        GUILayout.Space(4);

                        GUIContent gc_b = new GUIContent(FGUI_Resources.Tex_Bone, "Elastic effect blend for individual bones");

                        for (int l = 0; l < selectedLimb.Bones.Count; l++)
                        {
                            var bone = selectedLimb.Bones[l];

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(narr);

                            if (bone.T) if (GUILayout.Button(gc_b, EditorStyles.label, GUILayout.Width(18)))
                                { EditorGUIUtility.PingObject(bone.T); }

                            GUILayout.Space(6);

                            EditorGUILayout.LabelField("Blend for " + bone.BoneName, GUILayout.Width(144));
                            GUILayout.Space(8);
                            bone.ElasticnessBlend = EditorGUILayout.Slider(bone.ElasticnessBlend, 0f, 1f);
                            GUILayout.Space(2);

                            GUILayout.Space(narr);
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        GUILayout.Space(8);
                    }

                    #endregion

                    GUILayout.Space(4);
                    EditorGUILayout.EndVertical();

                    GUILayout.Space(5);
                }
                else
                {
                    GUILayout.Space(5);
                    EditorGUILayout.HelpBox("No Limb Selected", MessageType.Info);
                    GUILayout.Space(5);
                }

                EditorGUILayout.EndVertical();

                Elasticness_DrawTooltipField();

                EditorGUILayout.BeginVertical(); // To Avoid error for ending vertical
            }

            if (_anim_MainSet.TurnOnElasticness == false) GUI.enabled = true;

            //if (EditorGUI.EndChangeCheck())
            //{
            //    Undo.RecordObject(S, "ADElasticness");
            //    S._SetDirty();
            //    so_currentSetup.ApplyModifiedProperties();
            //}

        }

        #endregion


        #region Gizmos Related

        void _Gizmos_ElasticnessCategory()
        {
            if (_sel_elasticLimb != -1)
            {
                if (Limbs.ContainsIndex(_sel_elasticLimb, true) == false) return;

                Handles.color = new Color(0.8f + timeSin01 * 0.2f, 0.4f, 0.3f, 0.6f + timeSin01 * 0.4f);
                Limbs[_sel_elasticLimb].DrawGizmos(1f + timeSin01);
            }
        }

        #endregion


        #region Tip Field


        float _tip_elast_alpha = 0f;
        string _tip_elast = "";
        float _tip_elast_elapsed = -6f;
        int _tip_index = 0;

        void Elasticness_DrawTooltipField()
        {
            if (_tip_elast_alpha > 0f)
            {
                GUI.color = new Color(1f, 1f, 1f, _tip_elast_alpha);
                EditorGUILayout.LabelField(_tip_elast, EditorStyles.centeredGreyMiniLabel, GUILayout.Height(30));
                GUI.color = preGuiC;
            }
            else
                EditorGUILayout.LabelField(" ", EditorStyles.centeredGreyMiniLabel, GUILayout.Height(30));
        }


        void Elasticness_UpdateTooltips()
        {
            _tip_elast_elapsed += dt;

            if (_tip_elast == "") Tooltip_CheckElsticnessText();

            if (_tip_elast_elapsed > 0f)
            {
                if (_tip_elast_elapsed < 8f)
                {
                    _tip_elast_alpha = Mathf.Lerp(_tip_elast_alpha, 1f, dt * 3f);
                }
                else
                {
                    _tip_elast_alpha = Mathf.Lerp(_tip_elast_alpha, -0.05f, dt * 6f);
                }

                if (_tip_elast_elapsed > 20f)
                {
                    _tip_elast_elapsed = -6f;
                    _tip_elast_alpha = 0f;
                    Tooltip_CheckElsticnessText();
                }
            }
        }

        void Tooltip_CheckElsticnessText()
        {
            if (_tip_index == 0) _tip_elast = "Elasticness can help making your animations look bouncy,\ncartoonish or more heavy";
            else if (_tip_index == 1) _tip_elast = "Elasticness can influence all motion with it's bounciness\nincluding bone modificators animations!";
            else if (_tip_index == 2) _tip_elast = "Click RIGHT Mouse Button on the Category Button for Focus Mode";


            _tip_index += 1;
            if (_tip_index == 3) _tip_index = 0;
        }

        #endregion


        #region Update Loop Related

        void _Update_ElasticnessCategory()
        {

            Elasticness_UpdateTooltips();

        }

        #endregion


    }

}