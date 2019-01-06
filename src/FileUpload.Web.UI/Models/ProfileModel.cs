using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Models
{
    public class ProfileModel
    {
        public string Name { get; }
        public string UrlToken { get; }

        public ProfileModel(string name, string urlToken)
        {
            Ensure.NotNull(name, "name");
            Name = name;
            UrlToken = urlToken;
        }
    }
}
