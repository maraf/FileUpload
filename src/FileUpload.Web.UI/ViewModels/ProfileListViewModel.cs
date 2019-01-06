using FileUpload.Models;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.ViewModels
{
    public class ProfileListViewModel
    {
        public IReadOnlyList<ProfileModel> Profiles { get; }

        public ProfileListViewModel(IReadOnlyList<ProfileModel> profiles)
        {
            Ensure.NotNull(profiles, "profiles");
            Profiles = profiles;
        }
    }
}
