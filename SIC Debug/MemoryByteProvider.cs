using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Windows.Forms;

namespace SIC_Debug
{
    class MemoryByteProvider : IByteProvider
    {

        private byte[] backingStore;

        public MemoryByteProvider(byte[] backing)
        {
            this.backingStore = backing;
        }

        public byte ReadByte(long index)
        {
            return backingStore[index];
        }

        public void WriteByte(long index, byte value)
        {
            backingStore[index] = value;
            if (Changed != null)
                Changed(this, new EventArgs());
        }

        public void InsertBytes(long index, byte[] bs)
        {
            LengthChanged(null, null);
            throw new NotSupportedException();
        }

        public void DeleteBytes(long index, long length)
        {
            throw new NotSupportedException();
        }

        public long Length
        {
            get { return backingStore.Length; }
        }

        public event EventHandler LengthChanged;

        public bool HasChanges()
        {
            return false;
        }

        public void ApplyChanges()
        {
        }

        public event EventHandler Changed;

        public bool SupportsWriteByte()
        {
            return true;
        }

        public bool SupportsInsertBytes()
        {
            return false;
        }

        public bool SupportsDeleteBytes()
        {
            return false;
        }
    }
}
