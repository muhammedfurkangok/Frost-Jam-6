﻿using System;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Just for generic rigs
    /// </summary>
    public class ADRootMotionBakeHelper
    {
        public Transform AnimatorTransform { get; private set; }
        public Transform RootMotionTransform { get; private set; }
        public AnimationDesignerSave Save { get; private set; }
        public ADClipSettings_Main MainSet { get; private set; }
        public Animator Mecanim { get; private set; }
        public ADBoneReference RootRef { get; private set; }
        public bool KeepMotionKeyframesOnRoot = false;
        Quaternion rootMapping;
        private float ScaleOffset = 1f;

        public ADRootMotionBakeHelper(Transform animatorTr, ADBoneReference rootRef, AnimationDesignerSave save, ADClipSettings_Main main)
        {
            AnimatorTransform = animatorTr;
            RootRef = rootRef;
            RootMotionTransform = rootRef.TempTransform;
            if (RootMotionTransform == animatorTr) if (save) if (save.ReferencePelvis) RootMotionTransform = save.ReferencePelvis;
            Save = save;
            MainSet = main;
            KeepMotionKeyframesOnRoot = false;
            DetectedMotionInRootInsteadOfMotion = false;
            Mecanim = animatorTr.GetAnimator();
            ScaleOffset = 1f;
        }

        public void ResetForBaking()
        {
            AnimationDesignerWindow.ForceTPose();

            PrepareRootMotionPosition();
            PrepareRootMotionRotation();

            startBakePos = RootMotionTransform.position;
            startBakeRot = RootMotionTransform.rotation;
            latestRootMotionPos = Vector3.zero;
            latestRootMotionRot = Quaternion.identity;
            latestRootMotionRotEnsure = Quaternion.identity;

            rootMapping = Quaternion.FromToRotation(RootMotionTransform.InverseTransformDirection(AnimatorTransform.right), Vector3.right);
            rootMapping *= Quaternion.FromToRotation(RootMotionTransform.InverseTransformDirection(AnimatorTransform.up), Vector3.up);
        }

        Vector3 startBakePos;
        Quaternion startBakeRot;

        Vector3 latestAnimatorPos;
        Quaternion latestAnimatorRot;

        Vector3 latestPos;
        Quaternion latestRot;

        public Vector3 latestRootMotionPos { get; private set; }
        public Quaternion latestRootMotionRot { get; private set; }

        // Just infiuriating exception handling... 
        public bool DetectedMotionInRootInsteadOfMotion { get; internal set; }

        Quaternion latestRootMotionRotEnsure;

        public static Vector3 RootModsOffsetAccumulation = Vector3.zero;
        public static Vector3 RootModsRotOffsetAccumulation = Vector3.zero;

        public void PostAnimator()
        {
            latestAnimatorPos = RootMotionTransform.position;
            latestAnimatorRot = RootMotionTransform.rotation;
        }

        public void PostRootMotion()
        {
            if (Mecanim) { if (Mecanim.isHuman && AnimationDesignerWindow._forceExportGeneric == false) { ScaleOffset = Mecanim.humanScale; } else ScaleOffset = 1f; } else ScaleOffset = 1f;
            Vector3 diff = RootMotionTransform.position - (latestAnimatorPos); 
            Vector3 local = AnimatorTransform.InverseTransformVector(diff / ScaleOffset);

            //UnityEngine.Debug.Log("diff = " + diff);
            latestRootMotionPos = local;

            //Quaternion rDiff = RootMotionTransform.rotation * Quaternion.Inverse(latestAnimatorRot);
            //rDiff = RootMotionTransform.rotation;
            //latestRootMotionRot = (rDiff) * Quaternion.Inverse(rootMapping);
            latestRootMotionRot = RootMotionTransform.rotation * Quaternion.Inverse(latestAnimatorRot);
            //Debug.Log("latestRootMotionRot = " + latestRootMotionRot.eulerAngles + " rootEul = " + latestRootMotionRot);
            //latestRootMotionRot = latestRootMotionRot;// * Quaternion.Inverse(rootMapping);
            latestPos = RootMotionTransform.position;

            if (KeepMotionKeyframesOnRoot == false) // Restoring state before root modificator
            {
                // Stripping root motion out of keyframed animation
                RootMotionTransform.position = latestAnimatorPos;
                RootMotionTransform.rotation = latestAnimatorRot;
            }
        }


        #region Just initializing curves


        [NonSerialized] public AnimationCurve _Bake_RootMPosX;
        [NonSerialized] public AnimationCurve _Bake_RootMPosY;
        [NonSerialized] public AnimationCurve _Bake_RootMPosZ;

        /// <summary> Just for generic rigs root motion </summary>
        void PrepareRootMotionPosition()
        {
            _Bake_RootMPosX = new AnimationCurve();
            _Bake_RootMPosY = new AnimationCurve();
            _Bake_RootMPosZ = new AnimationCurve();
        }


        [NonSerialized] public AnimationCurve _Bake_RootMRotX;
        [NonSerialized] public AnimationCurve _Bake_RootMRotY;
        [NonSerialized] public AnimationCurve _Bake_RootMRotZ;
        [NonSerialized] public AnimationCurve _Bake_RootMRotW;


        /// <summary> Just for generic rigs root motion </summary>
        void PrepareRootMotionRotation()
        {
            _Bake_RootMRotX = new AnimationCurve();
            _Bake_RootMRotY = new AnimationCurve();
            _Bake_RootMRotZ = new AnimationCurve();
            _Bake_RootMRotW = new AnimationCurve();
        }

        #endregion


        /// <summary> Just for generic rigs </summary>
        public void SaveRootMotionPositionCurves(ref AnimationClip clip, string motionStr = "Motion")
        {
            if (_Bake_RootMPosX == null || _Bake_RootMPosY == null || _Bake_RootMPosZ == null) return;

            float magn = ADBoneReference.ComputePositionMagn(_Bake_RootMPosX);
            magn += ADBoneReference.ComputePositionMagn(_Bake_RootMPosY);
            magn += ADBoneReference.ComputePositionMagn(_Bake_RootMPosZ);

            if (magn < 0.0001f) return;
            clip.SetCurve("", typeof(Animator), motionStr + "T.x", _Bake_RootMPosX);
            clip.SetCurve("", typeof(Animator), motionStr + "T.y", _Bake_RootMPosY);
            clip.SetCurve("", typeof(Animator), motionStr + "T.z", _Bake_RootMPosZ);
        }

        /// <summary> Just for generic rigs </summary>
        public void SaveRootMotionRotationCurves(ref AnimationClip clip, string motionStr = "Motion")
        {
            if (_Bake_RootMRotX == null || _Bake_RootMRotY == null || _Bake_RootMRotZ == null || _Bake_RootMRotW == null) return;

            float magn = ADBoneReference.ComputePositionMagn(_Bake_RootMRotX);
            magn += ADBoneReference.ComputePositionMagn(_Bake_RootMRotY);
            magn += ADBoneReference.ComputePositionMagn(_Bake_RootMRotZ);
            magn += ADBoneReference.ComputePositionMagn(_Bake_RootMRotW);
            //UnityEngine.Debug.Log("rot magn " + magn);

            if (magn < 0.0001f) return;
            clip.SetCurve("", typeof(Animator), motionStr + "Q.x", _Bake_RootMRotX);
            clip.SetCurve("", typeof(Animator), motionStr + "Q.y", _Bake_RootMRotY);
            clip.SetCurve("", typeof(Animator), motionStr + "Q.z", _Bake_RootMRotZ);
            clip.SetCurve("", typeof(Animator), motionStr + "Q.w", _Bake_RootMRotW);
        }

        internal void BakeCurrentState(float keyTime)
        {
            Vector3 pos = latestRootMotionPos;

            _Bake_RootMPosX.AddKey(keyTime, pos.x);
            _Bake_RootMPosY.AddKey(keyTime, pos.y);
            _Bake_RootMPosZ.AddKey(keyTime, pos.z);

            Quaternion rot = AnimationGenerateUtils.EnsureQuaternionContinuity(latestRootMotionRotEnsure, latestRootMotionRot);
            latestRootMotionRotEnsure = rot;
            _Bake_RootMRotX.AddKey(keyTime, rot.x);
            _Bake_RootMRotY.AddKey(keyTime, rot.y);
            _Bake_RootMRotZ.AddKey(keyTime, rot.z);
            _Bake_RootMRotW.AddKey(keyTime, rot.w);
        }

        internal void CopyRootMotionFrom(AnimationClip clip, bool allowRoot = false)
        {
            string motionStr = "Motion";
            if (ClipContainsRootPositionCurves(clip) == false)
            {
                if (allowRoot) if (ClipContainsRootPositionCurves(clip, "Root"))
                    {
                        motionStr = "Root";
                        DetectedMotionInRootInsteadOfMotion = true;
                    }
            }

            _Bake_RootMPosX = AnimationDesignerWindow.CopyCurve(GetEditorCurve(clip, motionStr + "T.x"));
            _Bake_RootMPosY = AnimationDesignerWindow.CopyCurve(GetEditorCurve(clip, motionStr + "T.y"));
            _Bake_RootMPosZ = AnimationDesignerWindow.CopyCurve(GetEditorCurve(clip, motionStr + "T.z"));

            _Bake_RootMRotX = AnimationDesignerWindow.CopyCurve(GetEditorCurve(clip, motionStr + "Q.x"));
            _Bake_RootMRotY = AnimationDesignerWindow.CopyCurve(GetEditorCurve(clip, motionStr + "Q.y"));
            _Bake_RootMRotZ = AnimationDesignerWindow.CopyCurve(GetEditorCurve(clip, motionStr + "Q.z"));
            _Bake_RootMRotW = AnimationDesignerWindow.CopyCurve(GetEditorCurve(clip, motionStr + "Q.w"));
        }

        internal bool BakedSomePositionRootMotion()
        {
            if (_Bake_RootMPosX == null || _Bake_RootMPosY == null || _Bake_RootMPosZ == null) return false;

            float magn = ADBoneReference.ComputePositionMagn(_Bake_RootMPosX);
            magn += ADBoneReference.ComputePositionMagn(_Bake_RootMPosY);
            magn += ADBoneReference.ComputePositionMagn(_Bake_RootMPosZ);
            if (magn < 0.0001f) return false;
            return true;
        }

        internal bool BakedSomeRotationRootMotion()
        {
            if (_Bake_RootMRotX == null || _Bake_RootMRotY == null || _Bake_RootMRotZ == null || _Bake_RootMRotW == null) return false;

            float magn = ADBoneReference.ComputePositionMagn(_Bake_RootMRotX);
            magn += ADBoneReference.ComputePositionMagn(_Bake_RootMRotY);
            magn += ADBoneReference.ComputePositionMagn(_Bake_RootMRotZ);
            magn += ADBoneReference.ComputePositionMagn(_Bake_RootMRotW);
            if (magn < 0.0001f) return false;
            return true;
        }

        private static AnimationCurve GetEditorCurve(AnimationClip clip, string propertyPath)
        {
            EditorCurveBinding binding = EditorCurveBinding.FloatCurve(string.Empty, typeof(Animator), propertyPath);
            return AnimationUtility.GetEditorCurve(clip, binding);
        }

        internal static bool ClipContainsAnyRootCurves(AnimationClip originalClip, string motionStr = "Motion")
        {
            return ClipContainsRootPositionCurves(originalClip, motionStr) || ClipContainsRootRotationCurves(originalClip, motionStr);
        }

        static string GetMotionString(bool isHumanoid)
        {
            if (isHumanoid)
                return "Motion";
            else
                return "Root";
        }

        internal static bool ClipContainsRootPositionCurves(AnimationClip clip, string motionStr = "Motion")
        {
            var tX = GetEditorCurve(clip, motionStr + "T.x");
            if (tX == null) return false;
            var tY = GetEditorCurve(clip, motionStr + "T.y");
            if (tY == null) return false;
            var tZ = GetEditorCurve(clip, motionStr + "T.z");
            if (tZ == null) return false;

            float magn = ADBoneReference.ComputePositionMagn(tX);
            magn += ADBoneReference.ComputePositionMagn(tY);
            magn += ADBoneReference.ComputePositionMagn(tZ);
            if (magn < 0.0001f) return false;

            return true;
        }

        internal static bool ClipContainsRootRotationCurves(AnimationClip clip, string motionStr = "Motion")
        {
            var tX = GetEditorCurve(clip, motionStr + "Q.x");
            if (tX == null) return false;
            var tY = GetEditorCurve(clip, motionStr + "Q.y");
            if (tY == null) return false;
            var tZ = GetEditorCurve(clip, motionStr + "Q.z");
            if (tZ == null) return false;
            var tW = GetEditorCurve(clip, motionStr + "Q.w");
            if (tW == null) return false;

            float magn = ADBoneReference.ComputePositionMagn(tX);
            magn += ADBoneReference.ComputePositionMagn(tY);
            magn += ADBoneReference.ComputePositionMagn(tZ);
            magn += ADBoneReference.ComputePositionMagn(tW);
            if (magn < 0.0001f) return false;

            return true;
        }
    }
}