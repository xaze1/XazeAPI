// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

using AdminToys;
using Mirror;
using UnityEngine;

namespace XazeAPI.API.AudioCore.Speakers
{
    public class CustomSpeakerAudio : MonoBehaviour
    {
        public static CustomSpeakerAudio Create(byte controllerId, Vector3 position, float volume = 1f, bool isSpatial = true, float minDistance = 5f, float maxDistance = 5f)
        {
            SpeakerToy target = null;
            foreach (GameObject pref in NetworkClient.prefabs.Values)
            {
                if (!pref.TryGetComponent(out target))
                    continue;

                break;
            }

            // This should never happen but safety.
            if (target == null)
                return null;

            SpeakerToy newInstance = Instantiate(target, position, Quaternion.identity);

            newInstance.NetworkControllerId = controllerId;

            newInstance.NetworkVolume = volume;
            newInstance.IsSpatial = isSpatial;
            newInstance.MinDistance = minDistance;
            newInstance.MaxDistance = maxDistance;

            CustomSpeakerAudio speaker = newInstance.gameObject.AddComponent<CustomSpeakerAudio>();
            speaker.Base = newInstance;

            NetworkServer.Spawn(newInstance.gameObject);

            return speaker;
        }

        public SpeakerToy Base;
        public AudioPlayer Owner;

        public bool IsInUse { get; set; }
        public string Name { get; set; }
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
        public float Volume
        {
            get => Base.NetworkVolume;
            set => Base.NetworkVolume = value;
        }

        public bool IsSpatial
        {
            get => Base.NetworkIsSpatial;
            set => Base.NetworkIsSpatial = value;
        }

        public float MaxDistance
        {
            get => Base.NetworkMaxDistance;
            set => Base.NetworkMaxDistance = value;
        }

        public float MinDistance
        {
            get => Base.NetworkMinDistance;
            set => Base.NetworkMinDistance = value;
        }

        public void Destroy() => UnityEngine.Object.Destroy(gameObject);

        void OnDestroy()
        {
            if (Owner == null)
                return;

            Owner.SpeakersByName.Remove(Name);
        }
    }
}
