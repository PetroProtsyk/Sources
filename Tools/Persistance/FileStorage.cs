using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protsyk.Common.Persistance
{
    public class FileStorage : StreamStorage<FileStream>
    {
        public FileStorage(string name)
            : base(new FileStream(name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)) { }


        public override void Flush()
        {
            stream.Flush(true);
        }
    }
}
