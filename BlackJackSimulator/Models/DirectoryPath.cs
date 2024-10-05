using LanguageExt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSimulator.Models
{
    public record DirectoryPath
    {
        //use fromString
        private DirectoryPath(string pathString)
        {
            PathString = pathString;
        }

        public string PathString { get; init; }
        public static Either<DirectoryNotFoundException, DirectoryPath> FromString(string path) 
            => Path.Exists(path) ? new DirectoryPath(path) 
                                 : new DirectoryNotFoundException(path + " does not exist!");

    }
}
