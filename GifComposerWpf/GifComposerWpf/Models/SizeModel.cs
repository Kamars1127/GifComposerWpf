using System;
using System.Collections.Generic;
using System.Text;

namespace GifComposerWpf.Models
{
    public record class SizeModel
    {
        public int Width {  get; set; }
        public int Height { get; set; }

        public string DisplayName => $"{Width} x {Height}";
    }
}
