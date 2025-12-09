#if UNITY_EDITOR
using System.Collections.Generic;
using RePunkk.ReUtility;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimatorClipChanger))]
[CanEditMultipleObjects]
public class AnimatorClipChangerEditor : Editor
{
    private bool isPlayingInEditMode = false;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        AnimatorClipChanger[] targets = new AnimatorClipChanger[serializedObject.targetObjects.Length];
        for (int i = 0; i < serializedObject.targetObjects.Length; i++)
            targets[i] = (AnimatorClipChanger)serializedObject.targetObjects[i];

        AnimatorClipChanger primaryTarget = (AnimatorClipChanger)target;

        EditorGUILayout.LabelField("Current Clip:", primaryTarget.CurrentClipName);

        if (primaryTarget.Clips.Count > 0)
        {
            string[] clipNames = primaryTarget.GetClipNames();
            int previousIndex = primaryTarget.selectedClipIndex;
            primaryTarget.selectedClipIndex = EditorGUILayout.Popup("Select Clip", primaryTarget.selectedClipIndex, clipNames);

            if (previousIndex != primaryTarget.selectedClipIndex)
            {
                foreach (var targetObj in targets)
                {
                    if (primaryTarget.selectedClipIndex < targetObj.Clips.Count)
                    {
                        targetObj.ChangeClipIndex(primaryTarget.selectedClipIndex);
                        EditorUtility.SetDirty(targetObj);
                    }
                }
            }

            if (GUILayout.Button("Force Clip on PlayMode"))
            {
                foreach (var targetObj in targets)
                    if (primaryTarget.selectedClipIndex < targetObj.Clips.Count) targetObj.ChangeClip(primaryTarget.selectedClipIndex);
            }
        }

        if (!isPlayingInEditMode)
        {
            if (GUILayout.Button("Play Animation in Edit Mode"))
            {
                isPlayingInEditMode = true;
                foreach (var targetObj in targets) StartPreviewInEditor(targetObj);
            }
        }
        else
        {
            if (GUILayout.Button("Stop Animation"))
            {
                isPlayingInEditMode = false;
                foreach (var targetObj in targets) StopPreviewInEditor(targetObj);
            }
        }

        if (Application.isPlaying)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("PlayMode Controls", EditorStyles.boldLabel);

            if (GUILayout.Button("Next Clip"))
                foreach (var targetObj in targets) targetObj.NextClip();

            if (GUILayout.Button("Previous Clip"))
                foreach (var targetObj in targets) targetObj.PreviousClip();
        }

        if (GUILayout.Button("Load Clips from Animator"))
        {
            foreach (var targetObj in targets)
            {
                targetObj.LoadClipsFromAnimator();
                EditorUtility.SetDirty(targetObj);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private Dictionary<AnimatorClipChanger, float> previewTimes = new Dictionary<AnimatorClipChanger, float>();
    private List<AnimatorClipChanger> previewTargets = new List<AnimatorClipChanger>();

    private void StartPreviewInEditor(AnimatorClipChanger clipChanger)
    {
        if (clipChanger.Clips.Count == 0 || clipChanger.selectedClipIndex >= clipChanger.Clips.Count) return;

        Animator animator = clipChanger.GetAnimator();
        if (animator == null) return;

        AnimationClip clip = clipChanger.Clips[clipChanger.selectedClipIndex];
        if (clip == null) return;

        if (!previewTargets.Contains(clipChanger))
        {
            previewTargets.Add(clipChanger);
            previewTimes[clipChanger] = 0f;
        }

        if (!AnimationMode.InAnimationMode())
            AnimationMode.StartAnimationMode();

        AnimationMode.BeginSampling();
        AnimationMode.SampleAnimationClip(animator.gameObject, clip, 0f);
        AnimationMode.EndSampling();

        if (previewTargets.Count == 1)
            EditorApplication.update += UpdateAnimationPreview;
    }

    private void StopPreviewInEditor(AnimatorClipChanger clipChanger)
    {
        if (previewTargets.Contains(clipChanger))
        {
            previewTargets.Remove(clipChanger);
            if (previewTimes.ContainsKey(clipChanger))
                previewTimes.Remove(clipChanger);
        }

        if (previewTargets.Count == 0)
        {
            EditorApplication.update -= UpdateAnimationPreview;
            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();
        }
    }

    private void UpdateAnimationPreview()
    {
        if (!AnimationMode.InAnimationMode())
            AnimationMode.StartAnimationMode();

        AnimationMode.BeginSampling();

        for (int i = previewTargets.Count - 1; i >= 0; i--)
        {
            AnimatorClipChanger clipChanger = previewTargets[i];

            if (clipChanger == null || clipChanger.Clips.Count == 0 ||
                clipChanger.selectedClipIndex >= clipChanger.Clips.Count)
            {
                previewTargets.RemoveAt(i);
                continue;
            }

            Animator animator = clipChanger.GetAnimator();
            if (animator == null)
            {
                previewTargets.RemoveAt(i);
                continue;
            }

            AnimationClip clip = clipChanger.Clips[clipChanger.selectedClipIndex];
            if (clip == null)
            {
                previewTargets.RemoveAt(i);
                continue;
            }

            if (!previewTimes.ContainsKey(clipChanger))
                previewTimes[clipChanger] = 0f;

            previewTimes[clipChanger] += Time.deltaTime;
            if (previewTimes[clipChanger] > clip.length)
                previewTimes[clipChanger] = 0f;

            AnimationMode.SampleAnimationClip(animator.gameObject, clip, previewTimes[clipChanger]);
        }

        AnimationMode.EndSampling();
        SceneView.RepaintAll();

        if (previewTargets.Count == 0)
        {
            EditorApplication.update -= UpdateAnimationPreview;
            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();
            isPlayingInEditMode = false;
        }
    }

    private void OnDisable()
    {
        if (isPlayingInEditMode)
        {
            EditorApplication.update -= UpdateAnimationPreview;
            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();
            isPlayingInEditMode = false;
            previewTargets.Clear();
            previewTimes.Clear();
        }
    }
}
#endif
