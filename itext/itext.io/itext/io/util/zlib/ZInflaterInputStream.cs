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
using System.IO;

namespace System.util.zlib {
    /// <summary>
    /// Summary description for DeflaterOutputStream.
    /// </summary>
    public class ZInflaterInputStream : Stream {
        protected ZStream z=new ZStream();
        protected int flushLevel=JZlib.Z_NO_FLUSH;
        private const int BUFSIZE = 4192;
        protected byte[] buf=new byte[BUFSIZE];
        private byte[] buf1=new byte[1];

        protected Stream inp=null;
        private bool nomoreinput=false;

        public ZInflaterInputStream(Stream inp) : this(inp, false) {
        }
    
        public ZInflaterInputStream(Stream inp, bool nowrap) {
            this.inp=inp;
            z.inflateInit(nowrap);
            z.next_in=buf;
            z.next_in_index=0;
            z.avail_in=0;
        }
    
        public override bool CanRead {
            get {
                // TODO:  Add DeflaterOutputStream.CanRead getter implementation
                return true;
            }
        }
    
        public override bool CanSeek {
            get {
                // TODO:  Add DeflaterOutputStream.CanSeek getter implementation
                return false;
            }
        }
    
        public override bool CanWrite {
            get {
                // TODO:  Add DeflaterOutputStream.CanWrite getter implementation
                return false;
            }
        }
    
        public override long Length {
            get {
                // TODO:  Add DeflaterOutputStream.Length getter implementation
                return 0;
            }
        }
    
        public override long Position {
            get {
                // TODO:  Add DeflaterOutputStream.Position getter implementation
                return 0;
            }
            set {
                // TODO:  Add DeflaterOutputStream.Position setter implementation
            }
        }
    
        public override void Write(byte[] b, int off, int len) {
        }
    
        public override long Seek(long offset, SeekOrigin origin) {
            // TODO:  Add DeflaterOutputStream.Seek implementation
            return 0;
        }
    
        public override void SetLength(long value) {
            // TODO:  Add DeflaterOutputStream.SetLength implementation

        }
    
        public override int Read(byte[] b, int off, int len) {
            if(len==0)
                return(0);
            int err;
            z.next_out=b;
            z.next_out_index=off;
            z.avail_out=len;
            do {
                if((z.avail_in==0)&&(!nomoreinput)) { // if buffer is empty and more input is avaiable, refill it
                    z.next_in_index=0;
                    z.avail_in=inp.Read(buf, 0, BUFSIZE);//(BUFSIZE<z.avail_out ? BUFSIZE : z.avail_out));
                    if(z.avail_in<=0) {
                        z.avail_in=0;
                        nomoreinput=true;
                    }
                }
                err = z.inflate(flushLevel);
                if (err!=JZlib.Z_OK && err!=JZlib.Z_STREAM_END)
                    throw new IOException("inflating: "+z.msg);
                if((nomoreinput||err==JZlib.Z_STREAM_END)&&(z.avail_out==len))
                    return(0);
            } 
            while(z.avail_out==len&&err==JZlib.Z_OK);
            //System.err.print("("+(len-z.avail_out)+")");
            return(len-z.avail_out);
        }
    
        public override void Flush() {
            inp.Flush();
        }
    
        public override void WriteByte(byte b) {
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                inp.Dispose();
            }
            base.Dispose(disposing);
        }
    
        public override int ReadByte() {
            if(Read(buf1, 0, 1)<=0)
                return -1;
            return(buf1[0]&0xFF);
        }
    }
}
