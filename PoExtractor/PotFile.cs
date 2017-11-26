using System;
using System.Collections.Generic;
using System.IO;

namespace PoExtractor.Core {
    public class PotFile : IDisposable {
        private TextWriter _writer;

        public PotFile(string path) {
            _writer = new StreamWriter(File.Create(path));
        }

        public void WriteRecord(LocalizableString record) {
            foreach (var location in record.Locations) {
                _writer.WriteLine($"#: {location.SourceFile}:{location.SourceFileLine}");
                _writer.WriteLine($"#. {location.Comment}");
            }

            if (!string.IsNullOrEmpty(record.Context)) {
                _writer.WriteLine($"msgctxt \"{record.Context}\"");
            }

            _writer.WriteLine($"msgid \"{record.Text}\"");
            if (string.IsNullOrEmpty(record.TextPlural)) {
                _writer.WriteLine($"msgstr \"\"");
            } else {
                _writer.WriteLine($"msgid_plural \"{record.TextPlural}\"");
                _writer.WriteLine($"msgstr[0] \"\"");
            }


            _writer.WriteLine();
        }

        public void WriteRecord(IEnumerable<LocalizableString> records) {
            foreach (var record in records) {
                this.WriteRecord(record);
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                _writer.Close();
                _writer.Dispose();
            }
        }
    }
}
