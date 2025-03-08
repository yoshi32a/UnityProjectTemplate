using System;
using System.Collections.Generic;
using System.IO;

namespace Master.Editor
{
    public class TinyCsvReader : IDisposable
    {
        static char[] trim =
        {
            ' ',
            '\t'
        };

        readonly StreamReader reader;
        public IReadOnlyList<string> Header { get; private set; }

        public TinyCsvReader(StreamReader reader)
        {
            this.reader = reader;

            var line = reader.ReadLine();
            if (line == null)
            {
                throw new InvalidOperationException("Header is null.");
            }

            var index = 0;
            var header = new List<string>();
            while (index < line.Length)
            {
                var s = GetValue(line, ref index);
                if (s.Length == 0) break;
                header.Add(s);
            }

            Header = header;
        }

        string GetValue(string line, ref int i)
        {
            var temp = new char[line.Length - i];
            var j = 0;
            for (; i < line.Length; i++)
            {
                if (line[i] == ',')
                {
                    i += 1;
                    break;
                }

                temp[j++] = line[i];
            }

            return new string(temp, 0, j).Trim(trim);
        }

        public string[] ReadValues()
        {
            var line = reader.ReadLine();
            if (line == null) return null;
            if (string.IsNullOrWhiteSpace(line)) return null;

            var values = new string[Header.Count];
            var lineIndex = 0;
            for (int i = 0; i < values.Length; i++)
            {
                var s = GetValue(line, ref lineIndex);
                values[i] = s;
            }

            return values;
        }

        public Dictionary<string, string> ReadValuesWithHeader()
        {
            var values = ReadValues();
            if (values == null)
            {
                return null;
            }

            var dict = new Dictionary<string, string>();
            for (var i = 0; i < values.Length; i++)
            {
                dict.Add(Header[i], values[i]);
            }

            return dict;
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}