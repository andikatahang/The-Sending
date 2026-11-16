using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace TheSending.EditorTools
{
    // Build Windows (Mono) dari Mac, untuk uji. Build rilis non-development dibuat di Fase 8.
    // Batchmode: Unity -batchmode -quit -projectPath <proyek> -executeMethod TheSending.EditorTools.PembuatBuild.Windows
    public static class PembuatBuild
    {
        const string Keluaran = "Builds/Windows/TheSending.exe";

        [MenuItem("The Sending/Build Windows (uji, Mono)")]
        public static void Windows()
        {
            if (PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone) != ScriptingImplementation.Mono2x)
                throw new BuildFailedException("Scripting backend Standalone harus Mono (CLAUDE.md).");

            var opsi = new BuildPlayerOptions
            {
                scenes = Array.ConvertAll(EditorBuildSettings.scenes, s => s.path),
                locationPathName = Keluaran,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development,   // supaya tombol uji F5/F9 hidup di laptop Windows
            };
            var laporan = BuildPipeline.BuildPlayer(opsi);
            if (laporan.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException($"Build Windows gagal: {laporan.summary.result}, {laporan.summary.totalErrors} error.");
            Debug.Log($"Build Windows selesai: {Keluaran} ({laporan.summary.totalSize / (1024 * 1024)} MB)");
        }
    }
}
