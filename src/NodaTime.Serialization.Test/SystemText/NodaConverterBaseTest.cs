// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.IO;
using System.Text.Json;
using NodaTime.Serialization.SystemText;
using NodaTime.Utility;
using NUnit.Framework;

namespace NodaTime.Serialization.Test.SystemText
{
    public class NodaConverterBaseTest
    {
        [Test]
        public void Serialize_NonNullValue()
        {
            var converter = new TestConverter();
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                Converters = { converter }
            };

            JsonSerializer.Serialize(5, options);
        }

        [Test]
        public void Serialize_NullValue()
        {
            var converter = new TestConverter();
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                Converters = { converter }
            };

            JsonSerializer.Serialize(null, options);
        }

        [Test]
        public void Deserialize_NullableType_NullValue()
        {
            var converter = new TestConverter();
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                Converters = { converter }
            };

            Assert.IsNull(JsonSerializer.Deserialize<int?>("null", options));
            Assert.IsNull(JsonSerializer.Deserialize<int?>("\"\"", options));
        }

        [Test]
        public void Deserialize_ReferenceType_NullValue()
        {
            var converter = new TestStringConverter();
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                Converters = { converter }
            };

            Assert.IsNull(JsonSerializer.Deserialize<string>("null", options));
            Assert.IsNull(JsonSerializer.Deserialize<string>("\"\"", options));
        }

        [Test]
        public void Deserialize_NullableType_NonNullValue()
        {
            var converter = new TestConverter();
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                Converters = { converter }
            };

            Assert.AreEqual(5, JsonSerializer.Deserialize<int?>("\"5\"", options));
        }

        [Test]
        public void Deserialize_NonNullableType_NullValue()
        {
            var converter = new TestConverter();
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                Converters = { converter }
            };

            Assert.Throws<InvalidNodaDataException>(() => JsonSerializer.Deserialize<int>("null", options));
            Assert.Throws<InvalidNodaDataException>(() => JsonSerializer.Deserialize<int>("\"\"", options));
        }

        [Test]
        public void Deserialize_NonNullableType_NonNullValue()
        {
            var converter = new TestConverter();
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                Converters = { converter }
            };

            Assert.AreEqual(5, JsonSerializer.Deserialize<int>("\"5\"", options));
        }

        [Test]
        public void CanConvert_ValidValues()
        {
            var converter = new TestConverter();

            Assert.IsTrue(converter.CanConvert(typeof(int)));
            Assert.IsTrue(converter.CanConvert(typeof(int?)));
        }

        [Test]
        public void CanConvert_InvalidValues()
        {
            var converter = new TestConverter();

            Assert.IsFalse(converter.CanConvert(typeof(uint)));
        }

        [Test]
        public void CanConvert_Inheritance()
        {
            var converter = new TestInheritanceConverter();

            Assert.IsTrue(converter.CanConvert(typeof(MemoryStream)));
        }

        private class TestConverter : NodaConverterBase<int>
        {
            protected override int ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options)
            {
                return int.Parse(reader.GetString());
            }

            protected override void WriteJsonImpl(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }

        private class TestStringConverter : NodaConverterBase<string>
        {
            protected override string ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options)
            {
                return reader.GetString();
            }

            protected override void WriteJsonImpl(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value);
            }
        }

        /// <summary>
        /// Just use for CanConvert testing...
        /// </summary>
        private class TestInheritanceConverter : NodaConverterBase<Stream>
        {
            protected override Stream ReadJsonImpl(ref Utf8JsonReader reader, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            protected override void WriteJsonImpl(Utf8JsonWriter writer, Stream value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
