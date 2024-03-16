using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    public partial class AnimationDesignerWindow : EditorWindow
    {
        void DrawSetupTab()
        {
            DrawPreSetupTab();
            DrawLimbsSetup();

            if (latestAnimator != null)
                if (Limbs.Count == 0)
                    if (latestAnimator.IsHuman() == false)
                    {
                        GUILayout.Space(4);

                        if (GUILayout.Button(new GUIContent("  Run Auto-Limb Detection Algorithm\n  <size=10>(Character must contain correct T-Pose)\n(And be Facing it's Z-Axis)</size>", FGUI_Resources.TexWaitIcon, "Left mouse button to use all child transforms of root bone.\n\nRight mouse button to use just setted up armature bones."), FGUI_Resources.ButtonStyleR, GUILayout.Height(52)))
                            AutoDetection_Limbs();

                        GUILayout.Space(4);
                    }

            EditorGUILayout.EndVertical();
            GUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUI.color = new Color(1f, 1f, 1f, 0.6f);
            EditorGUILayout.HelpBox("Prepare Limbs - Few transform game objects, to use them with IK or Limb Elasticness", MessageType.None);
            GUI.color = preGuiC;
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
            EditorGUILayout.BeginVertical();



            if (S)
            {
                ADClipSettings_Main mainSetup = S.GetSetupForClip(S.MainSetupsForClips, TargetClip, _toSet_SetSwitchToHash);
                if (mainSetup != null)
                {
                    EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

                    GUILayout.Space(3);
                    EditorGUILayout.BeginHorizontal();
                    if (DrawTargetClipField(FGUI_Resources.GetFoldSimbol(additionalSetupSettingsFoldout, true) + "  Additional Settings For:", true)) additionalSetupSettingsFoldout = !additionalSetupSettingsFoldout;
                    EditorGUILayout.EndHorizontal();

                    if (additionalSetupSettingsFoldout)
                    {
                        GUILayout.Space(6);
                        EditorGUILayout.LabelField("This are experimental parameters!", EditorStyles.centeredGreyMiniLabel);
                        GUILayout.Space(3);
                       

                        EditorGUILayout.BeginHorizontal();
                        EditorGUIUtility.fieldWidth = 32;
                        mainSetup.ClipDurationMultiplier = EditorGUILayout.Slider(new GUIContent("Clip Duration Multiplier:", "If you want to make modified output clip slower or faster"), mainSetup.ClipDurationMultiplier, 0.25f, (3f * (1 + Mathf.Max(0, mainSetup.AdditionalAnimationCycles))));
                        EditorGUIUtility.fieldWidth = 0;

                        if (mainSetup.ClipTimeReverse) GUI.backgroundColor = Color.white * 0.8f;
                        if ( GUILayout.Button(new GUIContent( FGUI_Resources.Tex_Refresh, "Reverse Clip Time"), FGUI_Resources.ButtonStyle, GUILayout.Height(18), GUILayout.Width(20))) { mainSetup.ClipTimeReverse = !mainSetup.ClipTimeReverse; }
                        GUI.backgroundColor = preBG;
                        EditorGUILayout.EndHorizontal();


                        DrawCurve(ref mainSetup.ClipSampleTimeCurve, "Clip Time Modify:", 0, 0, 0, 1, 3);
                        GUILayout.Space(7);

                        Vector2 trim = new Vector2(mainSetup.ClipTrimFirstFrames, 1f - mainSetup.ClipTrimLastFrames);
                        EditorGUILayout.MinMaxSlider(new GUIContent("Include Frames:", "If you want to export just part of original animation"), ref trim.x, ref trim.y, 0f, 1f);
                        mainSetup.ClipTrimFirstFrames = trim.x;
                        mainSetup.ClipTrimLastFrames = 1f - trim.y;

                        EditorGUILayout.LabelField("From " + mainSetup.GetStartFrame() + " frame   to   " + mainSetup.GetEndFrame() + " frame", EditorStyles.centeredGreyMiniLabel);
                        //mainSetup.ClipTrimFirstFrames = Mathf.Abs(trim.y - 1f);
                        GUILayout.Space(3);
                        EditorGUIUtility.labelWidth = 180;
                        mainSetup.ResetRootPosition = EditorGUILayout.Toggle(new GUIContent("Reset Root Position:", "Discarding root motion"), mainSetup.ResetRootPosition);
                        mainSetup.Export_DisableRootMotionExport = EditorGUILayout.Toggle(new GUIContent("Disable RootMotion Export:", "Ignore saving any root motion in the exported clip"), mainSetup.Export_DisableRootMotionExport);
                        GUILayout.Space(3);
                        mainSetup.AdditionalAnimationCycles = EditorGUILayout.IntSlider("Additional Cycles:", mainSetup.AdditionalAnimationCycles, 0, 6);
                        if (mainSetup.AdditionalAnimationCycles > 0) EditorGUILayout.HelpBox("With additional cycles some features can work wrong! Like 'Grounding Mode' for legs IK or the origianal clip Root Motion", MessageType.Warning);
                        EditorGUIUtility.labelWidth = 0;
                        GUILayout.Space(6);

                        //if (TargetClip.isLooping == false || (TargetClip.legacy && TargetClip.wrapMode != WrapMode.Loop))
                        //{
                        //    if (GUILayout.Button("Set original clip 'Loop'"))
                        //    {
                        //        string path = AssetDatabase.GetAssetPath(TargetClip);
                        //        if (!string.IsNullOrEmpty(path))
                        //        {
                        //            var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
                        //            if (mainAsset)
                        //            {
                        //                var sett = AnimationUtility.GetAnimationClipSettings(TargetClip);
                        //                sett.loopTime = true;
                        //                if (TargetClip.legacy) TargetClip.wrapMode = WrapMode.Loop;

                        //                AnimationUtility.SetAnimationClipSettings(TargetClip, sett);

                        //                EditorUtility.SetDirty(TargetClip);
                        //                EditorUtility.SetDirty(mainAsset);
                        //                AssetDatabase.Refresh();
                        //                AssetDatabase.SaveAssets();
                        //                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        //            }
                        //        }

                        //    }
                        //}
                    }

                    GUILayout.Space(3);
                    EditorGUILayout.EndVertical();
                }
            }
        }

        bool additionalSetupSettingsFoldout = false;

        #region GUI Related


        void DrawPreSetupTab()
        {
            GUILayout.Space(6);
            EditorGUILayout.LabelField(new GUIContent("Armature Setup", "Setup for the animated character armature"), FGUI_Resources.HeaderStyleBig);
            GUILayout.Space(4);
            FGUI_Inspector.DrawUILine(0.3f, 0.5f, 1, 8, 0.975f);

            if (S.SkelRootBone == latestAnimator) GUI.color = new Color(1f, 1f, 0f, 1f);

            EditorGUILayout.BeginHorizontal();
            Transform skelRoot = (Transform)EditorGUILayout.ObjectField(new GUIContent("Skeleton Root"), S.Armature.RootBoneReference.TempTransform, typeof(Transform), true);
            if (skelRoot != S.SkelRootBone) { S.Armature.SetRootBoneRef(skelRoot); S._SetDirty(); }

            GUI.color = preGuiC;
            if (S.SkelRootBone == latestAnimator) GUILayout.Label(new GUIContent(FGUI_Resources.Tex_Warning, "Skeleton root is same as animator transform, it probably will produce glitches!"), GUILayout.Width(16));

            EditorGUILayout.EndHorizontal();

            Transform pelvRef = (Transform)EditorGUILayout.ObjectField(new GUIContent("Skeleton Pelvis"), S.Armature.PelvisBoneReference.TempTransform, typeof(Transform), true);
            if (pelvRef != S.ReferencePelvis) { S.Armature.SetPelvisRef(pelvRef); S._SetDirty(); }

            if (S._Tips_RootAndHipsMakeSureCounter < 8)
            {
                GUILayout.Space(5);
                MessageType rootHipsWarnType = MessageType.Warning;
                if (S._Tips_RootAndHipsMakeSureCounter > 2) rootHipsWarnType = MessageType.Info;
                if (S._Tips_RootAndHipsMakeSureCounter > 4) rootHipsWarnType = MessageType.None;
                EditorGUILayout.HelpBox("MAKE SURE that Skeleton Root  (first skeleton bone - NOT Animator object)\nand Skeleton Pelvis  (parent object of legs)  is assigned correctly!", rootHipsWarnType);
                Rect mesR = GUILayoutUtility.GetLastRect();
                if (GUI.Button(mesR, GUIContent.none, GUIStyle.none)) { S._Tips_RootAndHipsMakeSureCounter += 3; }
            }

        }


        int _sel_SetupDisplayLimb = -1;

        void _LimbsRefresh()
        {

            if (DisplaySave.Armature.LatestAnimator == null)
                DisplaySave.Armature.LatestAnimator = DisplaySave.LatestAnimator;

            if (Limbs.Count == 0)
            {
                DisplaySave.PrepareAutoLimbs(DisplaySave.Armature);
            }

            for (int i = 0; i < DisplaySave.Limbs.Count; i++)
            {
                DisplaySave.Limbs[i].GizmosBlend = 1f;
                DisplaySave.Limbs[i].Index = i;
                DisplaySave.Limbs[i].RefresTransformReferences(DisplaySave.Armature);
            }
        }

        void DrawLimbsSetup()
        {
            GUILayout.Space(3);
            if (isReady == false) { EditorGUILayout.HelpBox("First prepare Armature", MessageType.Info); return; }

            if (Limbs.Count == 0)
            {
                EditorGUILayout.HelpBox("No Limbs To Display", MessageType.Info);
                GUILayout.Space(3);
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Add First Limb")) { Limbs.Add(new ADArmatureLimb()); }
                GUI.backgroundColor = preBG;
                _LimbsRefresh();
                return;
            }

            _LimbsRefresh();

            FGUI_Inspector.DrawUILine(0.3f, 0.5f, 1, 8, 0.975f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);

            EditorGUILayout.LabelField(new GUIContent("Limb Setups", "Limbs are parented transform chains. Multiple transform to apply some effects on"), FGUI_Resources.HeaderStyleBig);

            if (Limbs.Count > 0)
                if (GUILayout.Button("+", FGUI_Resources.ButtonStyle, GUILayout.Width(22), GUILayout.Height(16)))
                    Limbs.Add(new ADArmatureLimb());

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(6);

            DrawSelectorGUI(DisplaySave.Limbs, ref _sel_SetupDisplayLimb, 22, position.width - 30);


            if (_sel_SetupDisplayLimb > -1)
                if (Limbs.ContainsIndex(_sel_SetupDisplayLimb, true))
                {
                    var selectedLimb = Limbs[_sel_SetupDisplayLimb];

                    GUILayout.Space(3);
                    EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

                    selectedLimb.DisplaySetupGUI(S);

                    EditorGUILayout.EndVertical();


                    if (selectedLimb.RemoveMe)
                    {
                        Limbs.RemoveAt(_sel_SetupDisplayLimb);
                        _sel_SetupDisplayLimb = -1;
                    }

                }
                else
                {
                    GUILayout.Space(5);
                    EditorGUILayout.HelpBox("No Limb Selected", MessageType.Info);
                    GUILayout.Space(5);
                }

        }

        #endregion


        #region Gizmos Related

        void _Gizmos_SetupCategory()
        {
            if (_sel_SetupDisplayLimb != -1)
                if (Limbs.ContainsIndex(_sel_SetupDisplayLimb, true))
                {
                    Handles.color = new Color(0.2f, 0.9f, 0.7f, 0.4f + timeSin01 * 0.5f);
                    Limbs[_sel_SetupDisplayLimb].DrawGizmos(1.5f);
                }
        }

        #endregion


        #region Update Loop Related

        void _Update_SetupCategory()
        {

        }

        #endregion

    }

}