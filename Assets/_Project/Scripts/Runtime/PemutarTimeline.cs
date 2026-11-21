using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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
        // Sinyal yang sudah terkirim pada pemutaran ini, supaya Spasi (lewati) tidak mengirimnya dua kali.
        readonly HashSet<SignalEmitter> terkirim = new HashSet<SignalEmitter>();

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

        // Taruh objek yang digerakkan Timeline di pose awal (t=0) tanpa memutar, supaya tidak melompat saat PutarDanTunggu mulai.
        public void EvaluasiAwal()
        {
            if (director == null || director.playableAsset == null) return;
            director.time = 0;
            director.Evaluate();
        }

        // Dipanggil sutradara: yield return pemutar.PutarDanTunggu();
        public IEnumerator PutarDanTunggu()
        {
            if (director == null || director.playableAsset == null)
            {
                Debug.LogError($"PemutarTimeline '{name}' tanpa aset Timeline; cutscene dilewati.", this);
                yield break;
            }
            selesai = false;
            terkirim.Clear();
            director.time = 0;
            director.Play();
            yield return null; // Spasi dari frame yang sama (adegan sebelumnya) tidak boleh ikut melewati ini
            // Cadangan state: jika wrap mode bukan None, stopped tidak pernah terpicu dan adegan akan terkunci.
            // director != null: objeknya bisa dihancurkan di tengah cutscene (scene dibongkar).
            while (director != null && !selesai && director.state == PlayState.Playing)
            {
                if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) { Lewati(); yield break; }
                yield return null;
            }
        }

        // Spasi: kirim sisa sinyal berurutan (efek cerita tetap terjadi), lompat ke akhir, lalu berhenti.
        void Lewati()
        {
            if (director.playableAsset is TimelineAsset tl)
            {
                var sisa = tl.GetOutputTracks().SelectMany(tr => tr.GetMarkers())
                    .Concat(tl.markerTrack != null ? tl.markerTrack.GetMarkers() : Enumerable.Empty<IMarker>())
                    .OfType<SignalEmitter>().Where(e => e.asset != null && !terkirim.Contains(e))
                    .Distinct().OrderBy(e => e.time).ToList();
                foreach (var e in sisa) { terkirim.Add(e); Sinyal?.Invoke(e.asset.name); }
            }
            if (director == null) return;
            director.time = director.duration;
            director.Evaluate();
            director.Stop();
            selesai = true;
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is SignalEmitter e && e.asset != null && terkirim.Add(e)) Sinyal?.Invoke(e.asset.name);
        }
    }
}
