using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Saat tim mengubah Scale balok (mesh KubusMeter di Art/Models/Kubus) di Inspector, Scene view, atau Prefab Mode,
    // mesh diganti KubusMeter(ukuran baru) supaya tekstur tetap per meter (keramik, gedek tidak melar). Diproses setelah
    // seretan selesai. Mati di Play Mode dan batchmode: Bangun Ulang Semua tetap memakai angka skrip.
    [InitializeOnLoad]
    static class PemulihUvBalok
    {
        static readonly HashSet<EntityId> Antre = new HashSet<EntityId>();

        static PemulihUvBalok()
        {
            if (Application.isBatchMode) return;
            ObjectChangeEvents.changesPublished += SaatBerubah;
            EditorApplication.update += Proses;
        }

        static void SaatBerubah(ref ObjectChangeEventStream s)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            for (int i = 0; i < s.length; i++)
                if (s.GetEventType(i) == ObjectChangeKind.ChangeGameObjectOrComponentProperties)
                {
                    s.GetChangeGameObjectOrComponentPropertiesEvent(i, out var e);
                    Antre.Add(e.entityId);
                }
        }

        static void Proses()
        {
            if (Antre.Count == 0 || GUIUtility.hotControl != 0 || EditorApplication.isPlayingOrWillChangePlaymode) return;
            foreach (var id in Antre)
            {
                var o = EditorUtility.EntityIdToObject(id);
                var g = o as GameObject ?? (o as Component)?.gameObject;
                if (g != null) Rapikan(g);
            }
            Antre.Clear();
        }

        static Vector3 Ukuran(Transform t)
        {
            var s = t.localScale;
            return new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));
        }

        // True jika mesh diganti. Hanya mesh KubusMeter yang namanya tidak cocok dengan localScale (dibulatkan 1 cm).
        internal static bool Rapikan(GameObject g)
        {
            var mf = g.GetComponent<MeshFilter>();
            if (mf == null || !PembuatAsetAsli.MeshKubus(mf.sharedMesh)) return false;
            var ukuran = Ukuran(g.transform);
            if (mf.sharedMesh.name == PembuatAsetAsli.NamaKubus(ukuran)) return false;
            Undo.RecordObject(mf, "Rapikan tekstur balok");
            mf.sharedMesh = PembuatAsetAsli.KubusMeter(ukuran);
            PrefabUtility.RecordPrefabInstancePropertyModifications(mf);
            if (g.scene.IsValid()) EditorSceneManager.MarkSceneDirty(g.scene);
            return true;
        }

        [MenuItem("The Sending/Rapikan Tekstur Balok Terpilih")]
        static void RapikanTerpilih()
        {
            int n = 0;
            foreach (var akar in Selection.gameObjects)
                foreach (var t in akar.GetComponentsInChildren<Transform>(true))
                    if (Rapikan(t.gameObject)) n++;
            Debug.Log($"Rapikan tekstur balok: {n} mesh diganti.");
        }
    }
}
