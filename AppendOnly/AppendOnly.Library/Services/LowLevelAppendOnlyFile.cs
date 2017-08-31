﻿using System;
using System.Collections.Generic;
using Renfield.AppendOnly.Library.Contracts;

namespace Renfield.AppendOnly.Library.Services
{
    public class LowLevelAppendOnlyFile : LowLevelAppendOnly
    {
        public long[] GetIndex() => index.ToArray();

        public LowLevelAppendOnlyFile(RandomAccessor data, IEnumerable<long> index = null)
        {
            this.data = data;
            this.index = new List<long>(index ?? RebuildIndex());
        }

        public void Append(byte[] record)
        {
            lock (lockObject)
            {
                var position = data.get_length();
                data.write_int(position, record.Length);
                index.Add(position);

                position += sizeof(int);
                data.write_bytes(position, record);
            }
        }

        public byte[] Read(int i)
        {
            var length = data.get_length();
            var position = index[i];

            var size = data.read_int(position);
            position += sizeof(int);

            if (position + size > length)
                throw new Exception($"Internal error: cannot read {size} bytes starting at {position}; i = {i}, Index[i] = {index[i]}, length = {length}");

            return data.read_bytes(position, size);
        }

        public IEnumerable<byte[]> ReadFrom(int i)
        {
            var length = data.get_length();
            var position = index[i];

            while (position < length)
            {
                var size = data.read_int(position);
                position += sizeof(int);

                yield return data.read_bytes(position, size);
                position += size;
            }
        }

        //

        private readonly RandomAccessor data;
        private readonly List<long> index;
        private readonly object lockObject = new object();

        private IEnumerable<long> RebuildIndex()
        {
            var length = data.get_length();
            long position = 0;
            while (position < length)
            {
                yield return position;

                var size = data.read_int(position);
                position += size + sizeof(int);
            }
        }
    }
}