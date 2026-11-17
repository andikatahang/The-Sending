using System;

namespace TheSending.Runtime
{
    // Benda yang aksinya ditentukan sutradara scene (pintu pindah scene, pintu terkunci, dan sejenisnya).
    public class InteraktifAlur : Interaktif
    {
        public event Action Ditekan;

        public override void Interaksi() => Ditekan?.Invoke();
    }
}
