using System;
using System.IO;
using TheSending.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TheSending.EditorTools
{
    // Aset Timeline cutscene dibuat dari kode supaya bisa dibangun ulang lewat batchmode tanpa membuka editor (K1).
    // Aset ditimpa setiap scene pemakainya dibangun ulang: setel waktu dan bidikan di skrip pembuat scene, bukan di jendela Timeline.
    internal static class PembuatTimeline
    {
        const string Folder = "Assets/_Project/Timeline";
        const string FolderSinyal = Folder + "/Sinyal";

        // Aset .playable baru + objek "Timeline <nama>" berisi PlayableDirector dan PemutarTimeline.
        internal static (TimelineAsset Tl, PlayableDirector Director, PemutarTimeline Pemutar) Buat(string nama, float durasi)
        {
            Directory.CreateDirectory(FolderSinyal);
            string jalur = $"{Folder}/{nama}.playable";
            AssetDatabase.DeleteAsset(jalur);   // scene pemakainya dibangun ulang di run yang sama, jadi GUID baru tidak memutus apa pun
            var tl = ScriptableObject.CreateInstance<TimelineAsset>();
            AssetDatabase.CreateAsset(tl, jalur);   // harus tersimpan dulu: track, klip, dan marker lalu masuk sebagai sub-aset
            tl.durationMode = TimelineAsset.DurationMode.FixedLength;
            tl.fixedDuration = durasi;

            var g = new GameObject("Timeline " + nama);
            var director = g.AddComponent<PlayableDirector>();
            director.playableAsset = tl;
            director.playOnAwake = false;
            // Adegan menjeda gim (Time.timeScale 0) selama cutscene; dengan Game Time timeline beku di frame pertama.
            director.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
            director.extrapolationMode = DirectorWrapMode.None;   // berhenti di akhir: event stopped terpicu
            return (tl, director, g.AddComponent<PemutarTimeline>());
        }

        // Gerak lurus antar titik kunci (posisi dan arah hadap titik). Dua kunci berselisih 0,01 detik = potongan kamera.
        // Benda harus di akar scene, supaya posisi lokal = posisi dunia.
        internal static void Gerak(TimelineAsset tl, PlayableDirector director, GameObject benda, params (float detik, Transform titik)[] kunci)
        {
            if (benda.transform.parent != null) throw new ArgumentException($"'{benda.name}' harus di akar scene.");
            if (kunci.Length < 2) throw new ArgumentException("Gerak butuh minimal dua kunci.");
            var animator = benda.GetComponent<Animator>();
            if (animator == null) animator = benda.AddComponent<Animator>();   // tanpa controller: hanya Timeline yang menggerakkan

            var klip = new AnimationClip { name = "Gerak " + benda.name };
            for (int sumbu = 0; sumbu < 3; sumbu++)
            {
                var posisi = new AnimationCurve();
                var sudut = new AnimationCurve();
                float sudutSebelum = 0f;
                for (int i = 0; i < kunci.Length; i++)
                {
                    float s = kunci[i].titik.eulerAngles[sumbu];
                    sudutSebelum = i == 0 ? s : sudutSebelum + Mathf.DeltaAngle(sudutSebelum, s);   // jangan berputar lewat 360
                    posisi.AddKey(kunci[i].detik, kunci[i].titik.position[sumbu]);
                    sudut.AddKey(kunci[i].detik, sudutSebelum);
                }
                char huruf = "xyz"[sumbu];
                klip.SetCurve("", typeof(Transform), "m_LocalPosition." + huruf, Lurus(posisi));
                klip.SetCurve("", typeof(Transform), "localEulerAnglesRaw." + huruf, Lurus(sudut));
            }
            AssetDatabase.AddObjectToAsset(klip, tl);

            var track = tl.CreateTrack<AnimationTrack>("Gerak " + benda.name);
            var aset = (AnimationPlayableAsset)track.CreateClip(klip).asset;
            aset.removeStartOffset = false;   // kunci adalah posisi dunia, bukan relatif terhadap posisi awal
            director.SetGenericBinding(track, animator);
        }

        // Sinyal di marker track timeline. Diterima PemutarTimeline di objek director, diteruskan ke sutradara dengan nama ini.
        // Jangan di detik 0 tepat; pakai 0,05 supaya pasti terlewati saat diputar.
        internal static void Sinyal(TimelineAsset tl, float detik, string nama)
        {
            string jalur = $"{FolderSinyal}/{nama}.asset";
            var sinyal = AssetDatabase.LoadAssetAtPath<SignalAsset>(jalur);
            if (sinyal == null)
            {
                sinyal = ScriptableObject.CreateInstance<SignalAsset>();
                AssetDatabase.CreateAsset(sinyal, jalur);   // nama aset = nama file = nama yang diterima sutradara
            }
            tl.CreateMarkerTrack();   // tidak berbuat apa-apa jika sudah ada
            var emitter = tl.markerTrack.CreateMarker<SignalEmitter>(detik);
            emitter.asset = sinyal;
            emitter.retroactive = false;
            emitter.emitOnce = true;
        }

        // Dipanggil sekali setelah semua track dan sinyal dibuat, sebelum scene disimpan.
        internal static void Simpan(TimelineAsset tl)
        {
            foreach (var o in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tl))) EditorUtility.SetDirty(o);
            AssetDatabase.SaveAssets();
        }

        static AnimationCurve Lurus(AnimationCurve kurva)
        {
            for (int i = 0; i < kurva.length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(kurva, i, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(kurva, i, AnimationUtility.TangentMode.Linear);
            }
            return kurva;
        }
    }
}
