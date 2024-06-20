/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
    Authors: Apryse Software.

    This program is offered under a commercial and under the AGPL license.
    For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

    AGPL licensing:
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using System.IO;

namespace iText.Kernel.Utils {
	//\cond DO_NOT_DOCUMENT		
    internal class IdleOutputStream : Stream
	{
	    public override void Flush()
	    {
	        
	    }

	    public override long Seek(long offset, SeekOrigin origin)
	    {
	        throw new NotSupportedException();
	    }

	    public override void SetLength(long value)
	    {
            throw new NotSupportedException();
	    }

	    public override int Read(byte[] buffer, int offset, int count)
	    {
            throw new NotSupportedException();
	    }

	    public override void Write(byte[] buffer, int offset, int count)
	    {
	        
	    }

	    public override bool CanRead
	    {
	        get { return false; }
	    }

	    public override bool CanSeek
	    {
	        get { return false; }
	    }

	    public override bool CanWrite
	    {
	        get { return true; }
	    }

	    public override long Length
	    {
            get { throw new NotSupportedException(); }
	    }

	    public override long Position { get; set; }
	}
   //\endcond 
}
