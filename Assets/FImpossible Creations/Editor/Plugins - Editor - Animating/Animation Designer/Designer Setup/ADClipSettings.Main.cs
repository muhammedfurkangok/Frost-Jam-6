﻿using FIMSpace.FTools;
using System;
using UnityEngine;


namespace FIMSpace.AnimationTools
{
    [System.Serializable]
    public class ADClipSettings_Main : IADSettings
    {
        public AnimationClip settingsForClip;
        public bool TurnOnElasticness = true;
        public AnimationCurve ElasticnessEvaluation = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);
        public bool TurnOnIK = true;

        public int Export_LoopAdditionalKeys = 0;
        public bool Export_ForceRootMotion = false;
        public bool Export_DisableRootMotionExport = false;

        public Transform Pelvis;
        public Transform GetPelvis(AnimationDesignerSave save, Transform limbParent)
        {
            if (Pelvis == null)
            {

                if (save.LatestAnimator.IsHuman()) Pelvis = save.LatestAnimator.GetAnimator().GetBoneTransform(HumanBodyBones.Hips);
                else
                {
                    if (limbParent == null) if (save) if (save.Armature != null) if (save.Armature.PelvisBoneReference != null) limbParent = save.Armature.PelvisBoneReference.TempTransform;
                    Pelvis = limbParent;
                }
            }

            return Pelvis;
        }

        public float PelvisOffsetsBlend = 1f;

        public float PelvisConstantYOffset = 0f;

        public float PelvisYOffset = 0f;
        public AnimationCurve PelvisOffsetYEvaluate = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);

        public float PelvisXOffset = 0f;
        public AnimationCurve PelvisOffsetXEvaluate = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);

        public float PelvisZOffset = 0f;
        public AnimationCurve PelvisOffsetZEvaluate = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);

        [SerializeField, HideInInspector] private bool _Tip_WasDisplaying = false;
        [SerializeField, HideInInspector] private int _Tip_DisplayCount = -1;

        public ADClipSettings_Main Copy(ADClipSettings_Main to)
        {
            ADClipSettings_Main cpy = (ADClipSettings_Main)MemberwiseClone();

            cpy.setId = to.setId;
            cpy.setIdHash = to.setIdHash;
            cpy.PelvisOffsetXEvaluate = AnimationDesignerWindow.CopyCurve(PelvisOffsetXEvaluate);
            cpy.PelvisOffsetYEvaluate = AnimationDesignerWindow.CopyCurve(PelvisOffsetYEvaluate);
            cpy.PelvisOffsetZEvaluate = AnimationDesignerWindow.CopyCurve(PelvisOffsetZEvaluate);
            cpy.TurnOnElasticness = TurnOnElasticness;

            cpy.ElasticnesSettings = new ADClipSettings_Elasticness.ElasticnessSet();
            cpy.ElasticnesSettings.Enabled = to.ElasticnesSettings.Enabled;
            cpy.ElasticnesSettings.MoveDamping = to.ElasticnesSettings.MoveDamping;
            cpy.ElasticnesSettings.MoveMildRotate = to.ElasticnesSettings.MoveMildRotate;
            cpy.ElasticnesSettings.MoveRapidity = to.ElasticnesSettings.MoveRapidity;
            cpy.ElasticnesSettings.MoveSmoothing = to.ElasticnesSettings.MoveSmoothing;

            return cpy;
        }


        public float OffsetHandsIKBlend = 1f;

        public float IKGroundLevel = 0f;
        public Vector3 IKGroundNormal = Vector3.up;

        public ADClipSettings_Elasticness.ElasticnessSet ElasticnesSettings;

        [NonSerialized] public Transform RootMotionTransform = null;
        [NonSerialized] public Vector3 LatestInternalRootMotionOffset = Vector3.zero;

        internal float GetStartFrame(bool useDurMult = true)
        {
            if (SettingsForClip == null) return 0f;
            float frame = SettingsForClip.frameRate * (SettingsForClip.length * (useDurMult ? ClipDurationMultiplier : 1f)) * (ClipTrimFirstFrames);
            return Mathf.Floor(frame);
        }

        internal float GetEndFrame(bool useDurMult = true)
        {
            if (SettingsForClip == null) return 0f;
            float frame = SettingsForClip.frameRate * (SettingsForClip.length * (useDurMult ? ClipDurationMultiplier : 1f)) * (1f - ClipTrimLastFrames);
            return Mathf.Ceil(frame);
        }

        internal int GetClipFramesCount(bool useDurMult = true)
        {
            return Mathf.RoundToInt(SettingsForClip.frameRate * (SettingsForClip.length * (useDurMult ? ClipDurationMultiplier : 1f)));
        }

        [NonSerialized] public Vector3 LatestWorldPosition = Vector3.zero;
        [NonSerialized] public Quaternion LatestWorldRotation = Quaternion.identity;
        [NonSerialized] public Vector3 MotionInfluenceOffset = Vector3.zero;
        [NonSerialized] public Quaternion MotionRotationInfluenceOffset = Quaternion.identity;

        [NonSerialized] public FElasticTransform HipsElasticness = null;
        public FMuscle_Quaternion RotationMuscle { get; private set; }
        public FMuscle_Eulers EulerAnglesMuscle { get; private set; }


        public Transform LatestAnimator = null;

        public ADClipSettings_Main() { }


        /// <summary> Used for identifying different variants when modifying the same AnimationClip multiple times </summary>
        [SerializeField, HideInInspector] private string setId = "";
        [SerializeField, HideInInspector] private int setIdHash = 0;
        public void AssignID(string name) { setId = name; setIdHash = setId.GetHashCode(); }
        public string SetID { get { return setId; } }
        public int SetIDHash { get { return setIdHash; } }
        public AnimationClip SettingsForClip { get { return settingsForClip; } }

        public float ClipDurationMultiplier = 1f;
        public AnimationCurve ClipSampleTimeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);
        public bool ClipTimeReverse = false;
        public float ClipTrimFirstFrames = 0f;
        public float ClipTrimLastFrames = 0f;
        public bool ResetRootPosition = false;
        public int AdditionalAnimationCycles = 0;

        public void OnConstructed(AnimationClip clip, int hash)
        {
            settingsForClip = clip; setIdHash = hash;

            ElasticnesSettings = new ADClipSettings_Elasticness.ElasticnessSet(false, "", -1, 1f);
        }

        public void RefreshWithSetup(AnimationDesignerSave save) { }


        internal void ResetState(AnimationDesignerSave save)
        {
            if (save.Armature != null)
                if (save.Armature.RootBoneReference != null)
                    RootMotionTransform = save.SkelRootBone;

            if (!RootMotionTransform)
                if (save.LatestAnimator)
                    RootMotionTransform = save.LatestAnimator.transform;

            if (RootMotionTransform)
            {
                LatestWorldPosition = RootMotionTransform.position;
                LatestWorldRotation = RootMotionTransform.rotation;
            }

            if (Pelvis)
            {
                if (HipsElasticness != null) HipsElasticness.OverrideProceduralPositionHard(Pelvis.position);
                if (HipsElasticness != null) HipsElasticness.OverrideProceduralRotation(Pelvis.rotation);
            }

            LatestInternalRootMotionOffset = Vector3.zero;
        }

        internal void CheckForInitialization(AnimationDesignerSave save, bool reInitialize)
        {
            LatestAnimator = save.LatestAnimator;

            if (HipsElasticness == null || HipsElasticness.transform == null || reInitialize)
            {
                if (Pelvis)
                {
                    AnimationDesignerWindow.ForceZeroFramePose();
                    HipsElasticness = new FElasticTransform();
                    HipsElasticness.Initialize(Pelvis);
                }
                else
                {
                    GetPelvis(save, null);
                }
            }
        }


        void UpdateHipsElasticMotion(float dt)
        {
            if (ElasticnesSettings == null) return;
            if (HipsElasticness == null) return;
            if (HipsElasticness.PositionMuscle == null) return;
            if (ElasticnesSettings.Enabled == false) { return; }
            if (TurnOnIK == false) { return; }
            float blend = ElasticnesSettings.OnMoveBlend;
            if (blend <= 0f) return;

            if (blend > 0f)
            {
                UpdateElasticnessParams();

                Vector3 motInfl = MotionInfluenceOffset * (1f - ElasticnesSettings.MotionInfluence);

                #region Movement Based Elasticness

                FElasticTransform bone = HipsElasticness;

                bone.PositionMuscle.MotionInfluence(motInfl);
                bone.PositionMuscle.Update(dt, Pelvis.position);
                Pelvis.position = Vector3.LerpUnclamped(Pelvis.position, bone.PositionMuscle.ProceduralPosition, blend);

                #endregion
            }

        }


        #region Elasticness Related Utils


        void UpdateElasticnessParams()
        {
            if (TurnOnIK == false) { return; }
            if (HipsElasticness != null && HipsElasticness.transform != null)
            {
                HipsElasticness.PositionMuscle.Acceleration = ElasticnesSettings.MoveRapidity * 10000f;
                HipsElasticness.PositionMuscle.AccelerationLimit = ElasticnesSettings.MoveRapidity * 5000f;

                HipsElasticness.RotationRapidness = 1f - ElasticnesSettings.MoveMildRotate;

                HipsElasticness.PositionMuscle.BrakePower = 1f - ElasticnesSettings.MoveSmoothing;
                HipsElasticness.PositionMuscle.Damping = ElasticnesSettings.MoveDamping * 40f;
            }
        }


        #region Backup for elastic rotation

        //public void UpdateHipsElasticMotionP3(ADClipSettings_Elasticness.ElasticnessSet elastic, float dt, float progr, float boneBlend)
        //{
        //    if (elastic.Enabled == false) return;
        //    if (elasticProcBlend <= 0f) return;

        //    // Rotation Based Elasticness
        //    float rotBlend = elasticProcBlend * elastic.RotationsBlend;
        //    Transform T = Pelvis;

        //    if (rotBlend > 0f)
        //    {
        //        if (elastic.EulerMode)
        //        {
        //            #region Euler Update

        //            EulerAnglesMuscle.Update(dt, T.eulerAngles);

        //            float blendC = rotBlend * boneBlend;

        //            Quaternion targetRot = Quaternion.Euler(EulerAnglesMuscle.ProceduralEulerAngles);

        //            if (blendC >= 1f) T.rotation = targetRot;
        //            else T.rotation = Quaternion.LerpUnclamped(T.rotation, targetRot, blendC);

        //            #endregion
        //        }
        //        else
        //        {
        //            #region Quaternion Update

        //            RotationMuscle.Update(dt, T.transform.rotation);

        //            float blendC = rotBlend * boneBlend;

        //            if (blendC >= 1f) T.rotation = RotationMuscle.ProceduralRotation;
        //            else T.rotation = Quaternion.LerpUnclamped(T.rotation, RotationMuscle.ProceduralRotation, blendC);

        //            #endregion
        //        }
        //    }

        //}

        #endregion



        #endregion


        /// <summary>
        /// Motion influence compute
        /// </summary>
        internal void PreUpdateSimulation(AnimationDesignerSave save)
        {
            if (RootMotionTransform)
            {
                ComputeMotionInfluenceOffset();
            }
            else
            {
                ResetState(save);
            }

            if (TurnOnIK == false) { return; }
            if (HipsElasticness != null)
            {
                HipsElasticness.CaptureSourceAnimation();
            }
        }

        internal void LateUpdateSimulation(float dt, float elasticDt, float progr, AnimationDesignerSave save)
        {
            if (TurnOnIK == false) { return; }
            if (!Pelvis) GetPelvis(save, null);

            if (Pelvis)
            {
                //if (System.Single.IsNaN(Pelvis.transform.position.x)) Pelvis.transform.localPosition = Vector3.zero;
                Pelvis.transform.position += save.LatestAnimator.transform.TransformVector(GetHipsOffset(progr));
            }

            UpdateHipsElasticMotion(elasticDt);
        }

        internal void LateUpdateAfterAllSimulation()
        {
            //if (TurnOnIK == false) { return; }

        }

        void ComputeMotionInfluenceOffset()
        {
            if (RootMotionTransform)
            {
                Transform t = RootMotionTransform;
                MotionInfluenceOffset = (t.position - LatestWorldPosition);
                MotionRotationInfluenceOffset = t.rotation * Quaternion.Inverse(LatestWorldRotation);

                LatestWorldPosition = t.position;
                LatestWorldRotation = t.rotation;
            }
        }

        internal Vector3 GetHipsOffset(float progr)
        {
            Vector3 posOffset = new Vector3(0,0,0);

            posOffset.x = PelvisXOffset * PelvisOffsetXEvaluate.Evaluate(progr) * PelvisOffsetsBlend;
            posOffset.y = PelvisYOffset * PelvisOffsetYEvaluate.Evaluate(progr) * PelvisOffsetsBlend + PelvisConstantYOffset;
            posOffset.z = PelvisZOffset * PelvisOffsetZEvaluate.Evaluate(progr) * PelvisOffsetsBlend;

            return posOffset;
        }

        /// <summary> Converting desired seconds to animation clip progress amount </summary>
        internal float SecondsToProgress(float seconds)
        {
            return SecondsToProgress(seconds, settingsForClip.length);
        }

        public static float SecondsToProgress(float seconds, float clipLen)
        {
            return seconds / clipLen;
        }

        internal void DampSessionReferences()
        {
            Pelvis = null;
            LatestAnimator = null;
            HipsElasticness = null;
        }

        internal int GUI_OnDisplay(bool visible, int addDisplays = 0, AnimationDesignerSave save = null)
        {
            _Tip_DisplayCount += addDisplays;

            if (visible)
            {
                if (_Tip_WasDisplaying == false)
                {
                    _Tip_WasDisplaying = true;
                    _Tip_DisplayCount += 1;
                    if (save != null) if (_Tip_DisplayCount < 2) RefreshPelvisCheck(save.Armature);
                }
            }
            else
            {
                _Tip_WasDisplaying = false;
            }

            return _Tip_DisplayCount;
        }

        internal void RefreshPelvisCheck(ADArmatureSetup arm)
        {
            if (arm != null)
                if (_Tip_DisplayCount == 0)
                {
                    if (arm.PelvisBoneReference != null)
                    {
                        if (arm.PelvisBoneReference.TempTransform != null)
                        {
                            if (arm.PelvisBoneReference.TempTransform != Pelvis)
                            {
                                Pelvis = arm.PelvisBoneReference.TempTransform;
                            }
                        }
                    }
                }
        }
    }
}