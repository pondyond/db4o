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

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
    public sealed class CharHandler : PrimitiveHandler
    {
        internal const int Length = Const4.CharBytes + Const4.AddedLength;
        private static readonly char Defaultvalue = (char) 0;

        public override object DefaultValue()
        {
            return Defaultvalue;
        }

        public override int LinkLength()
        {
            return Length;
        }

        public override Type PrimitiveJavaClass()
        {
            return typeof (char);
        }

        internal override object Read1(ByteArrayBuffer a_bytes)
        {
            var b1 = a_bytes.ReadByte();
            var b2 = a_bytes.ReadByte();
            var ret = (char) ((b1 & unchecked(0xff)) | ((b2 & unchecked(0xff))
                                                        << 8));
            return ret;
        }

        public override void Write(object a_object, ByteArrayBuffer a_bytes)
        {
            var char_ = ((char) a_object);
            a_bytes.WriteByte((byte) (char_ & unchecked(0xff)));
            a_bytes.WriteByte((byte) (char_ >> 8));
        }

        public override object Read(IReadContext context)
        {
            var b1 = context.ReadByte();
            var b2 = context.ReadByte();
            var charValue = (char) ((b1 & unchecked(0xff)) | ((b2 & unchecked(0xff)) << 8));
            return charValue;
        }

        public override void Write(IWriteContext context, object obj)
        {
            var charValue = ((char) obj);
            context.WriteBytes(new[]
            {
                (byte) (charValue & unchecked(0xff)), (byte
                    ) (charValue >> 8)
            });
        }

        public override IPreparedComparison InternalPrepareComparison(object source)
        {
            var sourceChar = ((char) source);
            return new _IPreparedComparison_90(sourceChar);
        }

        private sealed class _IPreparedComparison_90 : IPreparedComparison
        {
            private readonly char sourceChar;

            public _IPreparedComparison_90(char sourceChar)
            {
                this.sourceChar = sourceChar;
            }

            public int CompareTo(object target)
            {
                if (target == null)
                {
                    return 1;
                }
                var targetChar = ((char) target);
                return sourceChar == targetChar ? 0 : (sourceChar < targetChar ? -1 : 1);
            }
        }
    }
}