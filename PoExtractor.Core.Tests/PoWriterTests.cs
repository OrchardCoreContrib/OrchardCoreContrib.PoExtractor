using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace PoExtractor.Core.Tests {
    public class PoWriterTests {
        private MemoryStream _stream = new MemoryStream();

        [Fact]
        public void WriteRecord_WritesSingularLocalizableString() {
            var s = new LocalizableString() {
                Text = "Computer"
            };

            using (var writer = new PoWriter(_stream)) {
                writer.WriteRecord(s);
            }

            var result = ReadPoStream();
            Assert.Equal($"msgid \"Computer\"", result[0]);
            Assert.Equal($"msgstr \"\"", result[1]);
        }

        [Fact]
        public void WriteRecord_WritesPluralLocalizableString() {
            var s = new LocalizableString() {
                Text = "Computer",
                TextPlural = "Computers"
            };

            using (var writer = new PoWriter(_stream)) {
                writer.WriteRecord(s);
            }

            var result = ReadPoStream();
            Assert.Equal($"msgid \"Computer\"", result[0]);
            Assert.Equal($"msgid_plural \"Computers\"", result[1]);
            Assert.Equal($"msgstr[0] \"\"", result[2]);
        }

        [Fact]
        public void WriteRecord_WritesContext() {
            var s = new LocalizableString() {
                Text = "Computer",
                Context = "CONTEXT"
            };

            using (var writer = new PoWriter(_stream)) {
                writer.WriteRecord(s);
            }

            var result = ReadPoStream();
            Assert.Equal($"msgctxt \"CONTEXT\"", result[0]);
            Assert.Equal($"msgid \"Computer\"", result[1]);
            Assert.Equal($"msgstr \"\"", result[2]);
        }

        [Fact]
        public void WriteRecord_WritesLocations() {
            var s = new LocalizableString() {
                Text = "Computer",
            };
            s.Locations.Add(new LocalizableStringLocation() { SourceFile = "File.cs", SourceFileLine = 1, Comment = "Comment 1" });
            s.Locations.Add(new LocalizableStringLocation() { SourceFile = "File.cs", SourceFileLine = 2, Comment = "Comment 2" });

            using (var writer = new PoWriter(_stream)) {
                writer.WriteRecord(s);
            }

            var result = ReadPoStream();
            Assert.Equal($"#: File.cs:1", result[0]);
            Assert.Equal($"#. Comment 1", result[1]);
            Assert.Equal($"#: File.cs:2", result[2]);
            Assert.Equal($"#. Comment 2", result[3]);
            Assert.Equal($"msgid \"Computer\"", result[4]);
            Assert.Equal($"msgstr \"\"", result[5]);
        }

        private string[] ReadPoStream() {
            using (var reader = new StreamReader(new MemoryStream(_stream.ToArray()))) {
                return reader.ReadToEnd().Split(Environment.NewLine);
            }
        }
    }
}
