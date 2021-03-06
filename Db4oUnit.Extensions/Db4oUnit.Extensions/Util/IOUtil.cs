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

using System.IO;
using Db4objects.Db4o.Foundation.IO;
using File = Sharpen.IO.File;

namespace Db4oUnit.Extensions.Util
{
    /// <exclude></exclude>
    public class IOUtil
    {
        /// <summary>Deletes the directory</summary>
        /// <exception cref="System.IO.IOException"></exception>
        public static void DeleteDir(string dir)
        {
            var absolutePath = new File(dir).GetCanonicalPath();
            var fDir = new File(dir);
            if (fDir.IsDirectory())
            {
                var files = fDir.List();
                for (var i = 0; i < files.Length; i++)
                {
                    DeleteDir(Path.Combine(absolutePath, files[i]));
                }
            }
            File4.Delete(dir);
        }
    }
}