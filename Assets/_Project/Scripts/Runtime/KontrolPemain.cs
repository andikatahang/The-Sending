using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace TheSending.Runtime
{
    // Semua tombol gim sesuai GDD, didefinisikan lewat kode supaya tidak ada aset input yang harus diedit.
    public class KontrolPemain
    {
        public readonly InputAction Gerak = new InputAction("Gerak", InputActionType.Value);
        public readonly InputAction Lihat = new InputAction("Lihat", InputActionType.Value, "<Mouse>/delta");
        public readonly InputAction Lari = new InputAction("Lari", InputActionType.Button, "<Keyboard>/leftShift");
        public readonly InputAction Interaksi = new InputAction("Interaksi", InputActionType.Button, "<Keyboard>/e");
        public readonly InputAction Senter = new InputAction("Senter", InputActionType.Button, "<Keyboard>/f");
        public readonly InputAction Kamera = new InputAction("Kamera", InputActionType.Button, "<Keyboard>/q");
        public readonly InputAction Kompas = new InputAction("Kompas", InputActionType.Button, "<Keyboard>/c");
        public readonly InputAction TahanNapas = new InputAction("TahanNapas", InputActionType.Button, "<Keyboard>/space");
        public readonly InputAction Jurnal = new InputAction("Jurnal", InputActionType.Button, "<Keyboard>/tab");
        public readonly InputAction Inventaris = new InputAction("Inventaris", InputActionType.Button, "<Keyboard>/i");
        public readonly InputAction JedaMenu = new InputAction("JedaMenu", InputActionType.Button, "<Keyboard>/escape");
        public readonly InputAction Klik = new InputAction("Klik", InputActionType.Button, "<Mouse>/leftButton");
        public readonly InputAction[] PilihanAngka =
        {
            new InputAction("Pilihan1", InputActionType.Button, "<Keyboard>/1"),
            new InputAction("Pilihan2", InputActionType.Button, "<Keyboard>/2"),
            new InputAction("Pilihan3", InputActionType.Button, "<Keyboard>/3"),
            new InputAction("Pilihan4", InputActionType.Button, "<Keyboard>/4"),
        };

        public KontrolPemain()
        {
            Gerak.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            Lari.AddBinding("<Keyboard>/rightShift");
        }

        IEnumerable<InputAction> Semua()
        {
            var daftar = new List<InputAction>
            {
                Gerak, Lihat, Lari, Interaksi, Senter, Kamera, Kompas, TahanNapas, Jurnal, Inventaris, JedaMenu, Klik,
            };
            daftar.AddRange(PilihanAngka);
            return daftar;
        }

        public void Aktifkan()
        {
            foreach (var a in Semua()) a.Enable();
        }

        public void Buang()
        {
            foreach (var a in Semua()) a.Dispose();
        }
    }
}
