using System;
using UnityEngine;

namespace FIMSpace.FTools
{
    public partial class FimpIK_Arm
    {
        // Foot/End Bone rotation helper with root reference
        public Quaternion HandIKBoneMapping { get; protected set; }
        [NonSerialized] public Vector3 HandMiddleOffset;
        private Vector3 shoulderForward;
        private UniRotateBone shoulderRotate;

        /// <summary> Assigning helpful reference to main root transform of body to help IK rotations </summary>
        public virtual void SetRootReference(Transform mainParentTransform)
        {
            Root = mainParentTransform;
            Quaternion preRot = Root.transform.rotation;
            Root.transform.rotation = Quaternion.identity;

            Vector3 handForwardWorld = (HandIKBone.transform.position - ForeArmIKBone.transform.position).normalized;
            Vector3 handLocalForward = HandIKBone.transform.InverseTransformDirection(handForwardWorld);
            Vector3 handLocalRight = mainParentTransform.forward;
            Vector3 handLocalUp = Vector3.Cross(handLocalForward, handLocalRight);

            Vector3 shoulderForwardWorld = (ShoulderIKBone.transform.position - ShoulderIKBone.transform.parent.position).normalized;
            shoulderForward = ShoulderIKBone.transform.InverseTransformDirection(shoulderForwardWorld);

            HandIKBoneMapping = Quaternion.FromToRotation(handLocalRight, Vector3.right);
            HandIKBoneMapping *= Quaternion.FromToRotation(handLocalUp, Vector3.up);
            shoulderRotate = new UniRotateBone(ShoulderTransform, mainParentTransform);
            Root.transform.rotation = preRot;
        }



        /// <summary>
        /// Put here any euler rotation (like 0,90,0) which will be mapped for correct hand rotation no matter how bones are rotated in skeleton rig (but root reference needed)
        /// </summary>
        /// <param name="rotation"></param>
        public void SetCustomIKRotation(Quaternion rotation, float blend = 1f, bool fromDefault = false)
        {
            if (blend == 1f)
                IKTargetRotation = rotation * HandIKBoneMapping;
            else
            {
                if (fromDefault)
                    IKTargetRotation = Quaternion.LerpUnclamped(IKTargetRotation, rotation * HandIKBoneMapping, blend);
                else
                    IKTargetRotation = Quaternion.LerpUnclamped(rotation, rotation * HandIKBoneMapping, blend);
            }
        }

        public void CaptureKeyframeAnimation()
        {
            shoulderRotate.CaptureKeyframeAnimation();

            IKBone child = IKBones[0];
            while (child != null)
            {
                child.CaptureSourceAnimation();
                child = (IKBone)child.Child;
            }
        }

        /// <summary> Reference scale for computations - active length from start bone to middle knee </summary>
        public float ScaleReference { get; protected set; }

        public void RefreshLength()
        {
            ScaleReference = (UpperArmIKBone.transform.position - ForeArmIKBone.transform.position).magnitude;
        }

        public void RefreshScaleReference()
        {
            ScaleReference = (UpperArmIKBone.transform.position - ForeArmIKBone.transform.position).magnitude;
        }


        /// <summary> Returning >= 1f when max range for IK point is reached </summary>
        public float GetStretchValue(Vector3 targetPos)
        {
            float toGoal = (UpperArmIKBone.srcPosition - targetPos).magnitude;
            return toGoal / limbLength;
        }

        protected virtual void CalculateLimbLength()
        {
            limbLength = Mathf.Epsilon;
            if (ShoulderIKBone.transform)
            {
                float shouldLen = (ShoulderIKBone.transform.position - UpperArmIKBone.transform.position).magnitude;
                limbLength += shouldLen * ShoulderBlend;
            }
            limbLength += (UpperArmIKBone.transform.position - ForeArmIKBone.transform.position).magnitude;
            limbLength += (ForeArmIKBone.transform.position - HandIKBone.transform.position).magnitude;
        }

        public bool PreventShoulderThirdQuat { get; set; } = true;
        /// <summary> By default value is 0.75 </summary>
        public float ShoulderSensitivity { get; set; } = 0.75f;
        public float PreventShoulderThirdQuatFactor { get; set; } = 0.01f;
        public float limbLength { get; private set; } = 0.1f;
        // Shoulder -----------------------
        void ComputeShoulder()
        {
            if (!Initialized) return;
            if (ShoulderBlend <= 0f) return;

            //shoulderRotate.RefreshCustomAxis(Vector3.up, Vector3.forward);
            //Quaternion preHandRotation = ShoulderIKBone.srcRotation;
            //Quaternion perfectRotation = preHandRotation;

            Vector3 toGoal = (IKTargetPosition - shoulderRotate.transform.position);
            Quaternion nRot = (ShoulderIKBone.GetRotation(toGoal.normalized, ShoulderIKBone.srcRotation * shoulderRotate.upReference));
            //Quaternion nRot = ShoulderIKBone.srcRotation * Quaternion.FromToRotation((ShoulderIKBone.srcRotation * shoulderRotate.fromParentForward).normalized, toGoal.normalized);

            float blend = IKWeight * ShoulderBlend;
            float armStretch = GetStretchValue(IKTargetPosition);
            armStretch *= 0.85f;
            if (armStretch > 1f) armStretch = 1f;
            //UnityEngine.Debug.Log("stretch = " + armStretch);

            blend *= Mathf.InverseLerp(0.6f, 1f, armStretch) * 0.9f;

            ShoulderIKBone.transform.rotation = Quaternion.Slerp(shoulderRotate.transform.rotation, nRot, blend);

            #region Backup

            ////UnityEngine.Debug.DrawLine(shoulderRotate.transform.position, IKTargetPosition, Color.green, 0.1f);
            ////Vector3 toTarget = IKTargetPosition - shoulderRotate.transform.position;
            ////Vector2 lookAngles = shoulderRotate.GetCustomLookAngles(toTarget, shoulderRotate);
            ////shoulderRotate.transform.rotation = shoulderRotate.RotateCustomAxis(lookAngles.x * ShoulderBlend, lookAngles.y * ShoulderBlend, shoulderRotate) * shoulderRotate.transform.rotation;
            ////return;


            //Quaternion r = Quaternion.FromToRotation(-shoulderRotate.Dir(shoulderRotate.right).normalized, toGoal.normalized);
            //Vector3 wrap = FEngineering.WrapVector(r.eulerAngles);
            //float mirror = (MirrorMaths ? -1f : 1f);

            //float blend = IKWeight * ShoulderBlend;
            //float shldLength = (ShoulderTransform.transform.position - UpperarmTransform.position).magnitude;

            //// Rotating when goal too high or too low
            //float yDist = (Mathf.Abs(ShoulderTransform.position.y - IKTargetPosition.y) - shldLength) / limbLength;
            //if (yDist < 0f) yDist = 0f; if (yDist > 1f) yDist = 1f;
            //float yAngle = -wrap.z * yDist;
            //yAngle = Mathf.Clamp(yAngle * ShoulderSensitivity, -35f, 20f);
            //yAngle *= blend;

            //if (yAngle != 0f) shoulderRotate.RotateYBy(yAngle);

            //// Rotating when goal too far in flat space ---------------------
            //if (PreventShoulderThirdQuat)
            //{
            //    float zDot = Vector3.Dot(toGoal.normalized, -shoulderRotate.Dir(shoulderRotate.right).normalized);
            //    if (zDot < -PreventShoulderThirdQuatFactor) wrap.y = -140f * mirror; // Avoiding shoulder rotation in third quart
            //}

            //Vector2 shldFlat = new Vector2(shoulderRotate.transform.position.x, shoulderRotate.transform.position.z);
            //float zDist = (Vector2.Distance(shldFlat, new Vector2(IKTargetPosition.x, IKTargetPosition.z)) - shldLength) / limbLength;
            //if (zDist < 0f) zDist = 0f; if (zDist > 1f) zDist = 1f;
            //float zAngle = -wrap.y * zDist;
            //zAngle = Mathf.Clamp(zAngle * ShoulderSensitivity, -35f, 90f);
            //zAngle *= blend * mirror;

            //if (zAngle != 0f) shoulderRotate.RotateZBy(zAngle);

            ////Debug.DrawRay(shoulderRotate.transform.position, shoulderRotate.Dir(shoulderRotate.forward), Color.blue);
            ////Debug.DrawRay(shoulderRotate.transform.position, shoulderRotate.Dir(shoulderRotate.up), Color.green);
            ////Debug.DrawRay(shoulderRotate.transform.position, shoulderRotate.Dir(shoulderRotate.right), Color.red);

            #endregion

        }

    }
}
