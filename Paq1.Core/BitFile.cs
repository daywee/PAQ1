﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace Paq1.Core
{
    public class BitFile : IDisposable
    {
        public BitFileMode FileMode { get; }
        public bool CanRead => FileMode == BitFileMode.Read;
        public bool CanWrite => FileMode == BitFileMode.Write;
        public bool IsEof { get; private set; }

        /// <summary>
        /// Gets the length of stream in bits.
        /// </summary>
        public long Length
        {
            get
            {
                if (FileMode == BitFileMode.Read)
                    return _fileStream.Length * 8;

                return _fileStream.Length * 8 + _bitsInBuffer;
            }
        }

        private readonly FileStream _fileStream;
        private byte _buffer;
        private int _bitsInBuffer;

        public BitFile(string path, BitFileMode fileMode)
        {
            FileMode = fileMode;
            switch (fileMode)
            {
                case BitFileMode.Read:
                    _fileStream = File.OpenRead(path);
                    break;
                case BitFileMode.Write:
                    _fileStream = new FileStream(path, System.IO.FileMode.Create);
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(fileMode), (int)fileMode, typeof(BitFileMode));
            }
        }

        /// <summary>
        /// Writes bit to file (MSB first).
        /// </summary>
        /// <param name="bit">LSB of 'bit' is written.</param>
        public void Write(uint bit)
        {
            if (!CanWrite)
                throw new NotSupportedException("Write not enabled.");

            bit &= 1; // ensure only LSB is used

            _buffer = (byte)((_buffer << 1) | (byte)bit);
            _bitsInBuffer++;

            if (_bitsInBuffer == 8)
            {
                _fileStream.WriteByte(_buffer);
                _bitsInBuffer = 0;
            }
        }

        /// <summary>
        /// Reads next bit from file (MSB first).
        /// </summary>
        /// <returns>Next bit or 0 bit when EOF.</returns>
        public uint Read()
        {
            if (!CanRead)
                throw new NotSupportedException("Read not enabled.");

            if (IsEof)
                return 0;

            if (_bitsInBuffer == 0)
            {
                var read = _fileStream.ReadByte();
                if (read == -1)
                {
                    IsEof = true;
                    return 0;
                }

                _buffer = (byte)read;
                _bitsInBuffer = 8;
            }

            _bitsInBuffer--;
            return (uint)((_buffer >> _bitsInBuffer) & 1);
        }

        public void Dispose()
        {
            if (FileMode == BitFileMode.Write && _bitsInBuffer != 0)
            {
                _buffer <<= 8 - _bitsInBuffer;
                _fileStream.WriteByte(_buffer);
            }

            _fileStream?.Dispose();
        }
    }

    public enum BitFileMode
    {
        Read,
        Write,
    }
}