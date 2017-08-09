using System;

namespace PA_Project {
    public class SpecificationTransformer {
        private readonly string _specFile;

        public SpecificationTransformer(string s) {
            _specFile = s;
        }

        private static int GetIndentationLevel(ref string line) {
            var level = 0;
            if (string.IsNullOrEmpty(line)) return level;
            while (line[0].Equals(' ')) {
                line = line.Substring(1);
                level++;
            }
            return level;
        }

        public string Transform() {
            var lines = _specFile.Split('\n');
            int currentLevelIndent, previousLevelIndent = GetIndentationLevel(ref lines[0]);
            for (var i = 1; i < lines.Length; i++) {
                currentLevelIndent = GetIndentationLevel(ref lines[i]);
                var indentationDifference = currentLevelIndent - previousLevelIndent;
                AddIndentationChars(ref lines[i], indentationDifference);
                previousLevelIndent = currentLevelIndent;
            }
            return string.Concat(lines);
        }

        private static void AddIndentationChars(ref string s, int indentationDifference) {
            if (indentationDifference > 0) s = ">" + s;
            else if (indentationDifference < 0)
                for (var i = 0; i < Math.Abs(indentationDifference); i++)
                    s = "<" + s;
            else s = "><" + s;
        }
    }
}