using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Models
{
    public class FileModel
    {
        public string Name { get; }
        public long Size { get; }

        public FileModel(string name, long size)
        {
            Ensure.NotNull(name, "name");
            Ensure.PositiveOrZero(size, "size");
            Name = name;
            Size = size;
        }
    }
}
