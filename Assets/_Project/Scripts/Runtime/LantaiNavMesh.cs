using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TheSending.Runtime
{
    // NavMesh lantai scene, dibangun saat Sosok Santet pertama kali muncul (runtime), supaya pembuat scene di batchmode
    // tidak perlu menyimpan aset NavMesh. Setelan agen sendiri (radius 0,25 m) supaya pintu rumah bisa dilewati tanpa
    // mengubah ProjectSettings. Hanya collider padat yang dihitung: trigger (zona), daun pintu kamar (PintuKamar), dan objek yang diabaikan tidak.
    public class LantaiNavMesh : MonoBehaviour
    {
        public const float RadiusAgen = 0.25f, TinggiAgen = 2f;

        [SerializeField] Vector3 ukuran = new Vector3(30f, 8f, 30f);

        NavMeshDataInstance instance;
        bool siap;

        public int AgentTypeID { get; private set; } = -1;

        // abaikan: pemain (CharacterController) dan Sosok sendiri, supaya tidak meninggalkan lubang di NavMesh.
        public void Siapkan(params Transform[] abaikan)
        {
            if (siap) return;
            var setelan = NavMesh.CreateSettings();
            setelan.agentRadius = RadiusAgen;
            setelan.agentHeight = TinggiAgen;
            setelan.agentClimb = 0.3f;
            setelan.agentSlope = 45f;
            AgentTypeID = setelan.agentTypeID;

            var batas = new Bounds(transform.position, ukuran);
            var sumber = new List<NavMeshBuildSource>();
            NavMeshBuilder.CollectSources(batas, ~0, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), sumber);
            sumber.RemoveAll(x => x.component is Collider c && (c.isTrigger || c.GetComponentInParent<PintuKamar>() != null
                || Diabaikan(c.transform, abaikan)));
            var data = NavMeshBuilder.BuildNavMeshData(setelan, sumber, batas, Vector3.zero, Quaternion.identity);
            if (data == null)
            {
                Debug.LogError("NavMesh gagal dibangun.", this);
                return;
            }
            instance = NavMesh.AddNavMeshData(data);
            siap = true;
        }

        static bool Diabaikan(Transform t, Transform[] abaikan)
        {
            foreach (var a in abaikan)
                if (a != null && t.IsChildOf(a)) return true;
            return false;
        }

        void OnDestroy()
        {
            if (siap) instance.Remove();
            if (AgentTypeID != -1) NavMesh.RemoveSettings(AgentTypeID);
        }
    }
}
