// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;

namespace XazeAPI.API.Helpers
{
    public static class TextToyHelper
    {
        public static IEnumerator<float> FadeOutText(TextToy text, float speed = 5f)
        {
            if (text == null || text.Base == null) yield break;
            var textMesh = text.Base._textMesh;

            while (textMesh is { alpha: > 0f })
            {
                textMesh.alpha = Mathf.Max(0f, textMesh.alpha - Time.deltaTime * speed);
                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> FadeInText(TextToy text, float speed = 5f)
        {
            if (text == null || text.Base == null) yield break;
            var textMesh = text.Base._textMesh;

            while (textMesh is { alpha: < 1f })
            {
                textMesh.alpha = Mathf.Min(1f, textMesh.alpha + Time.deltaTime * speed);
                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> MoveTextUp(TextToy text, int steps = 10)
        {
            for (int i = 0; i < steps; i++)
            {
                if (text == null) yield break;
                text.Position += Vector3.up * 0.1f;
                yield return Timing.WaitForSeconds(0.05f);
            }
        }

        public static IEnumerator<float> FadeAnimation(TextToy text, float wait = 4f, bool move = true)
        {
            yield return Timing.WaitUntilDone(FadeInText(text, 2f));
            yield return Timing.WaitForSeconds(wait);
        
            if (move)
            {
                yield return Timing.WaitUntilDone(MoveTextUp(text));
            }

            yield return Timing.WaitUntilDone(FadeOutText(text, 2f));
            text?.Destroy();
        }

        public static IEnumerator<float> MoveAnimation(TextToy text, int steps = 10)
        {
            yield return Timing.WaitUntilDone(MoveTextUp(text, steps));
            text?.Destroy();
        }
    }
}
