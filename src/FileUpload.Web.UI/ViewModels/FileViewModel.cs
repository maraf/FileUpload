using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Web.UI.ViewModels
{
    public class FileViewModel
    {
        public string Name { get; }
        public long Size { get; }

        public FileViewModel(string name, long size)
        {
            Ensure.NotNull(name, "name");
            Ensure.PositiveOrZero(size, "size");
            Name = name;
            Size = size;
        }
    }
}
