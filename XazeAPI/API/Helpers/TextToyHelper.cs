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
            var textMesh = text.Base._textMesh;
            while (textMesh.alpha > 0.0)
            {
                textMesh.alpha -= Time.deltaTime * speed;
                textMesh.alpha = Mathf.Clamp(textMesh.alpha, 0, 1);

                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> FadeIntText(TextToy text, float speed = 5f)
        {
            var textMesh = text.Base._textMesh;
            while (textMesh.alpha < 1.0)
            {
                textMesh.alpha += Time.deltaTime * speed;
                textMesh.alpha = Mathf.Clamp(textMesh.alpha, 0, 1);

                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> MoveTextUp(TextToy text, int steps = 10)
        {
            for(int i=0; i < steps; i++)
            {
                text.Position += Vector3.one * 0.1f;
            }
            yield return Timing.WaitForSeconds(0.05f);
        }

        public static IEnumerator<float> FadeAnimation(TextToy text, float wait = 4f, bool move = true)
        {
            yield return Timing.WaitUntilDone(FadeIntText(text, 2));
            yield return Timing.WaitForSeconds(wait);
            if (move)
            {
                Timing.RunCoroutine(MoveTextUp(text));
            }
            yield return Timing.WaitUntilDone(FadeOutText(text, 2));
            text.Destroy();
        }

        public static IEnumerator<float> MoveAnimation(TextToy text)
        {
            yield return Timing.WaitUntilDone(MoveTextUp(text));
            text.Destroy();
        }
    }
}
