using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soundfont2Tool
{ 
    public struct MyStruct
    {
        internal uint raw;

        const int sz0 = 4, loc0 = 0, mask0 = ((1 << sz0) - 1) << loc0;
        const int sz1 = 4, loc1 = loc0 + sz0, mask1 = ((1 << sz1) - 1) << loc1;
        const int sz2 = 4, loc2 = loc1 + sz1, mask2 = ((1 << sz2) - 1) << loc2;
        const int sz3 = 4, loc3 = loc2 + sz2, mask3 = ((1 << sz3) - 1) << loc3;

        public uint Item0
        {
            get { return (uint)(raw & mask0) >> loc0; }
            set { raw = (uint)(raw & ~mask0 | (value << loc0) & mask0); }
        }

        public uint Item1
        {
            get { return (uint)(raw & mask1) >> loc1; }
            set { raw = (uint)(raw & ~mask1 | (value << loc1) & mask1); }
        }

        public uint Item2
        {
            get { return (uint)(raw & mask2) >> loc2; }
            set { raw = (uint)(raw & ~mask2 | (value << loc2) & mask2); }
        }

        public uint Item3
        {
            get { return (uint)((raw & mask3) >> loc3); }
            set { raw = (uint)(raw & ~mask3 | (value << loc3) & mask3); }
        }
    }
}
