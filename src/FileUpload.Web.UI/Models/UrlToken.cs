using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Models
{
    public class UrlToken
    {
        public string Value { get; }

        public UrlToken(string value)
        {
            Ensure.NotNullOrEmpty(value, "value");
            Value = value;
        }

        public override string ToString() => Value;
    }
}
