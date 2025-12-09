using System.Collections.Generic;
using UnityEngine;

namespace RePunkk.ReUtility
{
    using System.Collections;

    public class AnimatorClipChanger : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private List<AnimationClip> clips = new List<AnimationClip>();

        [Space][SerializeField] private bool usingBlendTime = true;
        [SerializeField] private float blendTime = 0.1f;

        [SerializeField] private bool usingDelayOnStart = true;
        [SerializeField] private Vector2 delayOnStart = new Vector2(0, 1);


        [SerializeField][HideInInspector] private int index;
        [SerializeField][HideInInspector] public int selectedClipIndex;

        public string CurrentClipName => index >= 0 && index < clips.Count ? clips[index].name : "None";
        public List<AnimationClip> Clips => clips;
        public Animator GetAnimator() => anim;

        public void ChangeClip(int index)
        {
            if (index < clips.Count)
            {
                if (usingBlendTime) anim.CrossFade(clips[index].name, blendTime);
                else anim.Play(clips[index].name);
                this.index = index;
                selectedClipIndex = index;
            }
        }

        public void ChangeClip(int index, bool delayed, float delay, float blendTime = 0)
        {
            if (index < clips.Count)
            {
                if (usingBlendTime) anim.CrossFade(clips[index].name, blendTime);
                else anim.Play(clips[index].name, -1, delay);
                this.index = index;
                selectedClipIndex = index;
            }
        }

        public void ChangeClipIndex(int newIndex)
        {
            if (newIndex >= 0 && newIndex < clips.Count)
            {
                index = newIndex;
                selectedClipIndex = newIndex;
            }
        }

        public void NextClip()
        {
            index++;
            if (index >= clips.Count) index = 0;
            selectedClipIndex = index;
            PlayCurrentClip();
        }

        public void PreviousClip()
        {
            index--;
            if (index < 0) index = clips.Count - 1;
            selectedClipIndex = index;
            PlayCurrentClip();
        }

        private void PlayCurrentClip()
        {
            if (usingBlendTime) anim.CrossFade(clips[index].name, blendTime);
            else anim.Play(clips[index].name);
        }

        public string[] GetClipNames()
        {
            string[] names = new string[clips.Count];
            for (int i = 0; i < clips.Count; i++) names[i] = clips[i].name;
            return names;
        }

        public void LoadClipsFromAnimator()
        {
            if (anim == null)
            {
                anim = GetComponent<Animator>();
                if (anim == null) { Debug.LogError("No Animator found!"); return; }
            }

            clips.Clear();

            RuntimeAnimatorController controller = anim.runtimeAnimatorController;
            if (controller != null) foreach (var clip in controller.animationClips) if (!clips.Contains(clip)) clips.Add(clip);
        }

        private void Start()
        {
            StartCoroutine(PlayOnAwake(usingDelayOnStart ? Random.Range(delayOnStart.x, delayOnStart.y) : 0));
        }

        private IEnumerator PlayOnAwake(float delay)
        {
            if (selectedClipIndex >= 0 && selectedClipIndex < clips.Count)
            {
                index = selectedClipIndex;
                if (!usingDelayOnStart) ChangeClip(index);
                else ChangeClip(index, true, Random.Range(delayOnStart.x, delayOnStart.y));
            }
            yield return null;
        }
    }
}