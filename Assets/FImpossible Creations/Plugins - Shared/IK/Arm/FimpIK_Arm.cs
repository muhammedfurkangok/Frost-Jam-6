using System;
using UnityEngine;

namespace FIMSpace.FTools
{
    // TODO -> Limiting, Weights, Goal Modes

    /// <summary>
    /// FC: Class for processing IK logics for 3-bones inverse kinematics
    /// </summary>
    [System.Serializable]
    public partial class FimpIK_Arm
    {
        [Range(0f, 1f)] public float IKWeight = 1f;
        [Tooltip("Blend value for goal position")] [Space(4)] [Range(0f, 1f)] public float IKPositionWeight = 1f;
        [Tooltip("Blend value hand rotation")] [Range(0f, 1f)] public float HandRotationWeight = 1f;
        [Tooltip("Blend value for shoulder rotation")] [Range(0f, 1f)] public float ShoulderBlend = 1f;
        [Tooltip("Flex style algorithm for different limbs")] public FIK_HintMode AutoHintMode = FIK_HintMode.MiddleForward;
        [Tooltip("If left limb behaves wrong in comparison to right one")] public bool MirrorMaths = false;

        [FPD_Header("Bones References")]
        public Transform ShoulderTransform;
        public Transform UpperarmTransform;
        public Transform LowerarmTransform;
        public Transform HandTransform;

        [SerializeField] [HideInInspector] private IKBone[] IKBones;

        public Vector3 TargetElbowNormal { get; private set; }


        /// <summary> Updating processor with 3-bones oriented inverse kinematics </summary>
        public void Update()
        {
            if (!Initialized) return;

            CalculateLimbLength();
            Refresh();

            ComputeShoulder();

            // Arm IK Position ---------------------------------------------------

            float posWeight = IKPositionWeight * IKWeight;
            UpperArmIKBone.sqrMagn = (ForeArmIKBone.transform.position - UpperArmIKBone.transform.position).sqrMagnitude;
            ForeArmIKBone.sqrMagn = (HandIKBone.transform.position - ForeArmIKBone.transform.position).sqrMagnitude;

            TargetElbowNormal = GetDefaultFlexNormal();

            Vector3 orientationDirection = GetOrientationDirection(IKTargetPosition, TargetElbowNormal);
            if (orientationDirection == Vector3.zero) orientationDirection = ForeArmIKBone.transform.position - UpperArmIKBone.transform.position;

            if (posWeight > 0f)
            {
                Quaternion sBoneRot = UpperArmIKBone.GetRotation(orientationDirection, TargetElbowNormal);
                if (posWeight < 1f) sBoneRot = Quaternion.LerpUnclamped(UpperArmIKBone.transform.rotation, sBoneRot, posWeight);
                UpperArmIKBone.transform.rotation = sBoneRot;

                Quaternion sMidBoneRot = ForeArmIKBone.GetRotation(IKTargetPosition - ForeArmIKBone.transform.position, ForeArmIKBone.GetCurrentOrientationNormal());
                if (posWeight < 1f) sMidBoneRot = Quaternion.LerpUnclamped(ForeArmIKBone.transform.rotation, sMidBoneRot, posWeight);
                ForeArmIKBone.transform.rotation = sMidBoneRot;
            }

            HandBoneRotation();
        }


        /// <summary>
        /// Calculating IK pole position normal for desired flexing bend
        /// </summary>
        private Vector3 GetAutomaticFlexNormal()
        {
            Vector3 bendNormal = UpperArmIKBone.GetCurrentOrientationNormal();

            switch (AutoHintMode)
            {
                case FIK_HintMode.MiddleForward:
                    return Vector3.LerpUnclamped(bendNormal.normalized, ForeArmIKBone.srcRotation * ForeArmIKBone.forward, 0.5f);


                case FIK_HintMode.MiddleBack: return ForeArmIKBone.srcRotation * -ForeArmIKBone.right;

                case FIK_HintMode.EndForward:

                    Vector3 hintPos = ForeArmIKBone.srcPosition + HandIKBone.srcRotation * HandIKBone.forward * (MirrorMaths ? -1f : 1f);
                    Vector3 normal = Vector3.Cross(hintPos - UpperArmIKBone.srcPosition, IKTargetPosition - UpperArmIKBone.srcPosition);
                    if (normal == Vector3.zero) return bendNormal;

                    return normal;

                case FIK_HintMode.OnGoal: return Vector3.LerpUnclamped(bendNormal, IKTargetRotation * HandIKBone.right, 0.5f);
            }

            return bendNormal;
        }



        // Drawing helper gizmos for identifying IK process and setup
        public void OnDrawGizmos()
        {
            if (!Initialized) return;
        }


    }
}
