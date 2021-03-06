/* This file is part of the db4o object database http://www.db4o.com

Copyright (C) 2004 - 2011  Versant Corporation http://www.versant.com

db4o is free software; you can redistribute it and/or modify it under
the terms of version 3 of the GNU General Public License as published
by the Free Software Foundation.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program.  If not, see http://www.gnu.org/licenses/. */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Slots;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Fileheader
{
    /// <exclude></exclude>
    public class FileHeaderVariablePart2 : FileHeaderVariablePart
    {
        private const int ChecksumLength = Const4.LongLength;

        private const int SingleLength = ChecksumLength + (Const4.IntLength*8) + Const4
            .LongLength + 1 + Const4.AddedLength;

        private int _address;
        private int _length;

        public FileHeaderVariablePart2(LocalObjectContainer container, int address, int length
            ) : base(container)
        {
            // The variable part format is:
            // (long) checksum
            // (int) address of InMemoryIdSystem slot
            // (int) length of InMemoryIdSystem slot
            // (int) address of InMemoryFreespace
            // (int) length of InMemoryFreespace
            // (int) BTreeFreespace id
            // (int) converter version
            // (int) uuid index ID
            // (int) identity ID
            // (long) versionGenerator
            // (byte) freespace system used
            _address = address;
            _length = length;
        }

        public FileHeaderVariablePart2(LocalObjectContainer container) : this(container,
            0, 0)
        {
        }

        public override IRunnable Commit(bool shuttingDown)
        {
            var length = OwnLength();
            if (_address == 0 || _length < length)
            {
                var slot = AllocateSlot(MarshalledLength(length));
                _address = slot.Address();
                _length = length;
            }
            var buffer = new ByteArrayBuffer(length);
            Marshall(buffer, shuttingDown);
            WriteToFile(0, buffer);
            return new _IRunnable_65(this, length, buffer);
        }

        private int MarshalledLength(int length)
        {
            return length*4;
        }

        private void WriteToFile(int startAdress, ByteArrayBuffer buffer)
        {
            _container.WriteEncrypt(buffer, _address, startAdress);
            _container.WriteEncrypt(buffer, _address, startAdress + _length);
        }

        public virtual int OwnLength()
        {
            return SingleLength;
        }

        public virtual int Address()
        {
            return _address;
        }

        public virtual int Length()
        {
            return _length;
        }

        public override void Read(int address, int length)
        {
            _address = address;
            _length = length;
            var buffer = _container.ReadBufferBySlot(new Slot(address, MarshalledLength
                (length)));
            var versionsAreConsistent = VersionsAreConsistentAndSeek(buffer);
            // TODO: Diagnostic message if versions aren't consistent.
            ReadBuffer(buffer, versionsAreConsistent);
        }

        protected virtual void ReadBuffer(ByteArrayBuffer buffer, bool versionsAreConsistent
            )
        {
            buffer.IncrementOffset(ChecksumLength);
            var systemData = SystemData();
            systemData.IdSystemSlot(ReadSlot(buffer, false));
            systemData.InMemoryFreespaceSlot(ReadSlot(buffer, !versionsAreConsistent));
            systemData.BTreeFreespaceId(buffer.ReadInt());
            systemData.ConverterVersion(buffer.ReadInt());
            systemData.UuidIndexId(buffer.ReadInt());
            systemData.IdentityId(buffer.ReadInt());
            systemData.LastTimeStampID(buffer.ReadLong());
            systemData.FreespaceSystem(buffer.ReadByte());
        }

        private Slot ReadSlot(ByteArrayBuffer buffer, bool readZero)
        {
            var slot = new Slot(buffer.ReadInt(), buffer.ReadInt());
            if (readZero)
            {
                return Slot.Zero;
            }
            return slot;
        }

        private void Marshall(ByteArrayBuffer buffer, bool shuttingDown)
        {
            var checkSumOffset = buffer.Offset();
            buffer.IncrementOffset(ChecksumLength);
            var checkSumBeginOffset = buffer.Offset();
            WriteBuffer(buffer, shuttingDown);
            var checkSumEndOffSet = buffer.Offset();
            var bytes = buffer._buffer;
            var length = checkSumEndOffSet - checkSumBeginOffset;
            var checkSum = CRC32.CheckSum(bytes, checkSumBeginOffset, length);
            buffer.Seek(checkSumOffset);
            buffer.WriteLong(checkSum);
            buffer.Seek(checkSumEndOffSet);
        }

        protected virtual void WriteBuffer(ByteArrayBuffer buffer, bool shuttingDown)
        {
            var systemData = SystemData();
            WriteSlot(buffer, systemData.IdSystemSlot(), false);
            WriteSlot(buffer, systemData.InMemoryFreespaceSlot(), !shuttingDown);
            buffer.WriteInt(systemData.BTreeFreespaceId());
            buffer.WriteInt(systemData.ConverterVersion());
            buffer.WriteInt(systemData.UuidIndexId());
            var identity = systemData.Identity();
            buffer.WriteInt(identity == null
                ? 0
                : identity.GetID(_container.SystemTransaction
                    ()));
            buffer.WriteLong(systemData.LastTimeStampID());
            buffer.WriteByte(systemData.FreespaceSystem());
        }

        private void WriteSlot(ByteArrayBuffer buffer, Slot slot, bool writeZero)
        {
            if (writeZero || slot == null)
            {
                buffer.WriteInt(0);
                buffer.WriteInt(0);
                return;
            }
            buffer.WriteInt(slot.Address());
            buffer.WriteInt(slot.Length());
        }

        private bool CheckSumOK(ByteArrayBuffer buffer, int offset)
        {
            var initialOffSet = buffer.Offset();
            var length = OwnLength();
            length -= ChecksumLength;
            buffer.Seek(offset);
            var readCheckSum = buffer.ReadLong();
            var checkSumBeginOffset = buffer.Offset();
            var bytes = buffer._buffer;
            var calculatedCheckSum = CRC32.CheckSum(bytes, checkSumBeginOffset, length);
            buffer.Seek(initialOffSet);
            return calculatedCheckSum == readCheckSum;
        }

        private bool VersionsAreConsistentAndSeek(ByteArrayBuffer buffer)
        {
            var bytes = buffer._buffer;
            var length = OwnLength();
            var offsets = Offsets();
            var different = false;
            for (var i = 0; i < length; i++)
            {
                var b = bytes[offsets[0] + i];
                for (var j = 1; j < 4; j++)
                {
                    if (b != bytes[offsets[j] + i])
                    {
                        different = true;
                        break;
                    }
                }
            }
            if (!different)
            {
                // The following line cements our checksum algorithm in stone.
                // Things should be safe enough if we remove the throw.
                // If all four versions of the header are the same,
                // it's bound to be OK. (unless all bytes are zero or
                // greyed out by some kind of overwriting algorithm.)
                var firstOffset = 0;
                if (!CheckSumOK(buffer, firstOffset))
                {
                    throw new Db4oFileHeaderCorruptionException();
                }
                return true;
            }
            var firstPairDiffers = false;
            var secondPairDiffers = false;
            for (var i = 0; i < length; i++)
            {
                if (bytes[offsets[0] + i] != bytes[offsets[1] + i])
                {
                    firstPairDiffers = true;
                }
                if (bytes[offsets[2] + i] != bytes[offsets[3] + i])
                {
                    secondPairDiffers = true;
                }
            }
            if (!secondPairDiffers)
            {
                if (CheckSumOK(buffer, offsets[2]))
                {
                    buffer.Seek(offsets[2]);
                    return false;
                }
            }
            if (firstPairDiffers)
            {
                // Should never ever happen, we are toast.
                // We could still try to use any random version of
                // the header but which one?
                // Maybe the first of the second pair could be an 
                // option for a recovery tool, or it could try all
                // versions.
                throw new Db4oFileHeaderCorruptionException();
            }
            if (!CheckSumOK(buffer, 0))
            {
                throw new Db4oFileHeaderCorruptionException();
            }
            return false;
        }

        private int[] Offsets()
        {
            return new[] {0, OwnLength(), OwnLength()*2, OwnLength()*3};
        }

        public override int MarshalledLength()
        {
            return MarshalledLength(OwnLength());
        }

        private sealed class _IRunnable_65 : IRunnable
        {
            private readonly FileHeaderVariablePart2 _enclosing;
            private readonly ByteArrayBuffer buffer;
            private readonly int length;

            public _IRunnable_65(FileHeaderVariablePart2 _enclosing, int length, ByteArrayBuffer
                buffer)
            {
                this._enclosing = _enclosing;
                this.length = length;
                this.buffer = buffer;
            }

            public void Run()
            {
                _enclosing.WriteToFile(length*2, buffer);
            }
        }
    }
}