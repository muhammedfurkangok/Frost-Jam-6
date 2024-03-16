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


        void DrawMorphingTab()
        {
            GUILayout.Space(6);
            EditorGUILayout.LabelField(new GUIContent("Morph Limbs With Other Animations", " "), FGUI_Resources.HeaderStyleBig);
            GUILayout.Space(6);

            EditorGUILayout.HelpBox("Not Yet Implemented in the current version of Animation Designer", MessageType.Info);
            GUILayout.Space(6);

            //if (isReady == false) { EditorGUILayout.HelpBox("First prepare Armature", MessageType.Info); return; }

            //_LimbsRefresh();

            //FGUI_Inspector.DrawUILine(0.3f, 0.5f, 1, 8, 0.975f);
            //EditorGUILayout.LabelField(new GUIContent("Blend With Other Animations", " "), FGUI_Resources.HeaderStyleBig);
            //GUILayout.Space(6);

            //DrawSelectorGUI(S.Limbs, ref _sel_elasticLimb, 18, position.width - 22);

            //if (_sel_elasticLimb > -1)
            //{
            //    var selectedLimb = Limbs[_sel_elasticLimb];

            //    GUILayout.Space(3);
            //    EditorGUILayout.BeginVertical(FGUI_Resources.BGInBoxStyle);

            //    EditorGUILayout.HelpBox("asd", MessageType.None);

            //    EditorGUILayout.EndVertical();
            //}
            //else
            //{
            //    GUILayout.Space(5);
            //    EditorGUILayout.HelpBox("No Limb Selected", MessageType.Info);
            //    GUILayout.Space(5);
            //}

        }


        #endregion


        #region Gizmos Related

        void _Gizmos_MorphingCategory()
        {

        }

        #endregion


        #region Update Loop Related

        void _Update_MorphingCategory()
        {

        }

        #endregion

    }

}