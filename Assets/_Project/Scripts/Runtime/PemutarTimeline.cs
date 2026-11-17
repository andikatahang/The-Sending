using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TheSending.Runtime
{
    // Satu cutscene Timeline. PembuatTimeline (Task 12) membuat asetnya dan menyetel director: Play On Awake mati,
    // Update Method Unscaled Game Time (adegan menjeda gim, timeScale 0), wrap mode None (event stopped terpicu di akhir).
    // Sinyal di marker track timeline sampai ke sini (komponen di objek director) dan diteruskan sebagai nama SignalAsset.
    [RequireComponent(typeof(PlayableDirector))]
    public class PemutarTimeline : MonoBehaviour, INotificationReceiver
    {
        public event Action<string> Sinyal;

        PlayableDirector director;
        bool selesai;

        void Awake()
        {
            director = GetComponent<PlayableDirector>();
            director.stopped += SaatBerhenti;
        }

        void OnDestroy()
        {
            if (director != null) director.stopped -= SaatBerhenti;
        }

        void SaatBerhenti(PlayableDirector _) => selesai = true;

        // Dipanggil sutradara: yield return pemutar.PutarDanTunggu();
        public IEnumerator PutarDanTunggu()
        {
            if (director == null || director.playableAsset == null)
            {
                Debug.LogError($"PemutarTimeline '{name}' tanpa aset Timeline; cutscene dilewati.", this);
                yield break;
            }
            selesai = false;
            director.time = 0;
            director.Play();
            // Cadangan state: jika wrap mode bukan None, stopped tidak pernah terpicu dan adegan akan terkunci.
            // director != null: objeknya bisa dihancurkan di tengah cutscene (scene dibongkar).
            while (director != null && !selesai && director.state == PlayState.Playing) yield return null;
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is SignalEmitter e && e.asset != null) Sinyal?.Invoke(e.asset.name);
        }
    }
}
