using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace CardDataEditor.Utils.Debug {
    public class Profiler : MonoBehaviour {
        private static readonly Stack<ProfilerSection> sectionStack =
            new Stack<ProfilerSection>();

        private static readonly List<ProfilerSection> rootSections =
            new List<ProfilerSection>();

        private static Profiler instance;

        private class ProfilerSection {
            public string Name;
            public Stopwatch Stopwatch;
            public int CallCount;

            public Dictionary<string, ProfilerSection> Children;

            public ProfilerSection(string name) {
                Name = name;
                Stopwatch = new Stopwatch();
                CallCount = 0;
                Children = new Dictionary<string, ProfilerSection>();
            }

            public void Start() {
                Stopwatch.Start();
                CallCount++;
            }

            public void End() {
                Stopwatch.Stop();
            }
        }

        private void Awake() {
            if (instance != null)
                return;

            instance = this;
        }

        public static void Start(string name) {
            ProfilerSection section;

            if (sectionStack.Count == 0) {
                section = FindOrCreateRoot(name);
            } else {
                ProfilerSection parent = sectionStack.Peek();

                if (!parent.Children.TryGetValue(name, out section)) {
                    section = new ProfilerSection(name);
                    parent.Children.Add(name, section);
                }
            }

            section.Start();
            sectionStack.Push(section);
        }

        public static void End(string name) {
            if (sectionStack.Count == 0) {
                LoggerUtils.Log(
                    BepInEx.Logging.LogLevel.Warning,
                    $"Profiler.End(\"{name}\") called without a matching Start()."
                );

                return;
            }

            ProfilerSection section = sectionStack.Pop();

            if (section.Name != name) {
                LoggerUtils.Log(
                    BepInEx.Logging.LogLevel.Warning,
                    $"Profiler section mismatch. " +
                    $"Expected End(\"{section.Name}\"), got End(\"{name}\")."
                );
            }

            section.End();
        }

        private static ProfilerSection FindOrCreateRoot(string name) {
            foreach (ProfilerSection section in rootSections) {
                if (section.Name == name)
                    return section;
            }

            ProfilerSection newSection = new ProfilerSection(name);
            rootSections.Add(newSection);

            return newSection;
        }

        private void LateUpdate() {
            Log();
        }

        private static void Log() {
            if (rootSections.Count == 0)
                return;

            foreach (ProfilerSection section in rootSections) {
                LogSection(section, 0);
            }

            rootSections.Clear();
        }

        private static void LogSection(ProfilerSection section, int depth) {
            string indentation = new string(' ', depth * 2);

            double totalMilliseconds =
                section.Stopwatch.Elapsed.TotalMilliseconds;

            double averageMilliseconds =
                section.CallCount > 0
                    ? totalMilliseconds / section.CallCount
                    : 0;

            LoggerUtils.Log(
                BepInEx.Logging.LogLevel.Debug,
                $"{indentation} {section.Name}: " +
                $"Total {totalMilliseconds:F2} ms, " +
                $"Calls {section.CallCount}, " +
                $"Avg {averageMilliseconds:F4} ms"
            );

            foreach (ProfilerSection child in section.Children.Values) {
                LogSection(child, depth + 1);
            }
        }
    }
}