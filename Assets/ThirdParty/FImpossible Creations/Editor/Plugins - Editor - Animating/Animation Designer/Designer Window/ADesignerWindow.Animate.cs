using System;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    public partial class AnimationDesignerWindow : EditorWindow
    {

        ADClipSettings_Main _anim_MainSet;
        ADClipSettings_Elasticness _anim_elSet;
        ADClipSettings_Modificators _anim_modSet;
        ADClipSettings_IK _anim_ikSet;
        ADClipSettings_Springs _anim_springsSet;
        ADClipSettings_Morphing _anim_blendSet;


        //ADClipSettings_Main _clipSettings_Main;
        //ADClipSettings_Elasticness _clipSettings_Elasticness;
        //ADClipSettings_Modificators _clipSettings_Modificators;
        //ADClipSettings_IK _clipSettings_IK;
        //ADClipSettings_Springs _clipSettings_Springs;
        //ADClipSettings_Blending _clipSettings_Blending;


        public void ResetComponentsStates(bool reInitialize)
        {
            CheckComponentsInitialization(reInitialize);

            for (int i = 0; i < Limbs.Count; i++)
                Limbs[i].ResetLimbComponentsState();

            _anim_MainSet.ResetState(S);
            _anim_modSet.ResetStates();
            _anim_ikSet.ResetState();
        }


        void UpdateSimulationAfterAnimators(ADRootMotionBakeHelper rootBaker)
        {
            if (rootBaker != null)
            {
                rootBaker.PostAnimator();
            }

            _anim_modSet.PreLateUpdateModificators(deltaTime);
            _anim_modSet.BeforeLateUpdateModificators(deltaTime, animationProgress, S, _anim_MainSet);

            if (rootBaker != null)
            {
                rootBaker.PostRootMotion();
                _anim_MainSet.LatestInternalRootMotionOffset = rootBaker.latestRootMotionPos;
            }
            else
            {
                _anim_MainSet.LatestInternalRootMotionOffset = ADRootMotionBakeHelper.RootModsOffsetAccumulation;
            }

            if (_anim_MainSet.TurnOnIK)
            {
                for (int i = 0; i < Limbs.Count; i++)
                    Limbs[i].IKCapture(_anim_ikSet.GetIKSettingsForLimb(Limbs[i], S));
            }

            _anim_MainSet.PreUpdateSimulation(S);

            _anim_modSet.PreElasticnessLateUpdateModificators(deltaTime, animationProgress, S, _anim_MainSet);

            if (_anim_MainSet.TurnOnElasticness)
                for (int i = 0; i < Limbs.Count; i++)
                    Limbs[i].ElasticnessPreLateUpdate(_anim_elSet);


            if (_anim_MainSet.TurnOnIK)
            {
                for (int i = 0; i < Limbs.Count; i++)
                    Limbs[i].IKUpdateSimulation(_anim_ikSet.GetIKSettingsForLimb(Limbs[i], S), deltaTime, animationProgress, 1f);
            }
        }


        void LateUpdateSimulation()
        {
            for (int i = 0; i < Limbs.Count; i++)
                Limbs[i].ComponentsBlendingLateUpdate(_anim_blendSet, deltaTime, animationProgress);


            if (_anim_MainSet.TurnOnElasticness)
            {
                for (int i = 0; i < Limbs.Count; i++)
                {
                    if (Limbs[i].ExecuteFirst == false) continue;
                    Limbs[i].ElasticnessComponentsLateUpdate(_anim_elSet, _anim_MainSet, deltaTime, animationProgress);
                }

                for (int i = 0; i < Limbs.Count; i++)
                {
                    if (Limbs[i].ExecuteFirst == true) continue;
                    Limbs[i].ElasticnessComponentsLateUpdate(_anim_elSet, _anim_MainSet, deltaTime, animationProgress);
                }
            }

            _anim_modSet.LateUpdateModificators(deltaTime, animationProgress, S, _anim_MainSet);

            _anim_MainSet.LateUpdateSimulation(deltaTime, deltaTime, animationProgress, S);


            if (_anim_MainSet.TurnOnIK)
            {
                for (int i = 0; i < Limbs.Count; i++)
                    Limbs[i].IKLateUpdateSimulation(_anim_ikSet.GetIKSettingsForLimb(Limbs[i], S), dt, animationProgress, 1f, _anim_MainSet);
            }

            _anim_modSet.LastLateUpdateModificators(deltaTime, animationProgress, S, _anim_MainSet);

            _anim_MainSet.LateUpdateAfterAllSimulation();

        }


        void CheckComponentsInitialization(bool reInitialize)
        {
            bool hChanged = false;
            for (int i = 0; i < Limbs.Count; i++)
                if (Limbs[i].CheckIfHierarchyChanged()) hChanged = true;

            if (hChanged) reInitialize = true;

            _anim_MainSet = S.GetSetupForClip(S.MainSetupsForClips, TargetClip, _toSet_SetSwitchToHash);
            //_anim_MainSet = S.GetMainSetupForClip(TargetClip);
            _anim_MainSet.CheckForInitialization(S, reInitialize);

            for (int i = 0; i < Limbs.Count; i++) Limbs[i].RefreshLimb(S);

            _anim_elSet = S.GetSetupForClip(S.ElasticnessSetupsForClips, TargetClip, _toSet_SetSwitchToHash); //S.GetElasticnessSetupForClip(TargetClip);
            for (int i = 0; i < Limbs.Count; i++) Limbs[i].CheckLimbElasticnessComponentsInitialization(S, reInitialize);

            _anim_modSet = S.GetSetupForClip(S.ModificatorsSetupsForClips, TargetClip, _toSet_SetSwitchToHash); //_anim_modSet = S.GetModificatorsSetupForClip(TargetClip);
            _anim_modSet.CheckInitialization(S, reInitialize, _anim_MainSet);

            _anim_ikSet = S.GetSetupForClip(S.IKSetupsForClips, TargetClip, _toSet_SetSwitchToHash); //_anim_ikSet = S.GetIKSetupForClip(TargetClip);
            for (int i = 0; i < Limbs.Count; i++) Limbs[i].CheckForIKInitialization(S, _anim_ikSet.GetIKSettingsForLimb(Limbs[i], S), _anim_MainSet, animationProgress, dt, 1f, reInitialize);

            _anim_springsSet = S.GetSetupForClip(S.SpringSetupsForClips, TargetClip, _toSet_SetSwitchToHash); //_anim_springsSet = S.GetSpringSetupForClip(TargetClip);
            _anim_springsSet.CheckInitialization(S);

            _anim_blendSet = S.GetSetupForClip(S.BlendingSetupsForBlendings, TargetClip, _toSet_SetSwitchToHash); // _anim_blendSet = S.GetBlendingSetupForClip(TargetClip);
            for (int i = 0; i < Limbs.Count; i++) Limbs[i].CheckComponentsBlendingInitialization(reInitialize);
        }

        internal static Rect GetMenuDropdownRect(int width = 300)
        {
            return new Rect(Event.current.mousePosition + Vector2.left * 100, new Vector2(width, 340));
        }

    }
}