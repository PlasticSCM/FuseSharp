using System;
using System.IO;

namespace FuseSharp
{
    public class DirectoryEntry
    {
        private string name;

        public string Name
        {
            get { return name; }
        }

        public DirectoryEntry(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                throw new ArgumentException(
                    "Name contains invalid filename character", nameof(name));

            this.name = name;
        }
    }
}
