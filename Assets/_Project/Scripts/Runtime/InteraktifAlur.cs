using System;

namespace TheSending.Runtime
{
    // Benda yang aksinya ditentukan sutradara scene (pintu pindah scene, pintu terkunci, dan sejenisnya).
    public class InteraktifAlur : Interaktif
    {
        string promptAlur;

        public event Action Ditekan;

        public override string Prompt => string.IsNullOrEmpty(promptAlur) ? base.Prompt : promptAlur;

        // Sutradara mengganti teks prompt sesuai keadaan (misalnya pintu gudang setelah kunci didapat). null = teks asli.
        public void UbahPrompt(string teks) => promptAlur = teks;

        public override void Interaksi() => Ditekan?.Invoke();
    }
}
