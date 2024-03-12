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

    public class Test
    {
        public class CustomListItem
        {
            public string Text { get; set; }
            public System.Drawing.Color Color { get; set; }

            public CustomListItem(string text, System.Drawing.Color color)
            {
                Text = text;
                Color = color;
            }
        }
        System.Windows.Forms.ListBox listBox1;
        void test()
        {
            listBox1 = new System.Windows.Forms.ListBox();
            // Set the DrawMode of the ListBox to OwnerDrawFixed
            // so that we can control the rendering of list items.
            listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            listBox1.DrawItem += listBox1_DrawItem;

            // Add some items to the ListBox
            listBox1.Items.Add(new CustomListItem("Item 1", System.Drawing.Color.DarkMagenta));
            listBox1.Items.Add(new CustomListItem("Item 2", System.Drawing.Color.Blue));
            listBox1.Items.Add(new CustomListItem("Item 3", System.Drawing.Color.Green));
        }
        // Handle the DrawItem event of the ListBox
        private void listBox1_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                // Get the item from the ListBox
                CustomListItem item = listBox1.Items[e.Index] as CustomListItem;

                // Draw the background
                e.DrawBackground();


                // Create a brush with the color specified for the item
                System.Drawing.Brush brush = new System.Drawing.SolidBrush(item.Color);

                // Draw the item's text
                e.Graphics.DrawString(item.Text, e.Font, brush, e.Bounds);
                e.Graphics.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red), 0, 0, 10, 10);

                // Dispose of the brush
                brush.Dispose();

                // If the ListBox has focus, draw the focus rectangle
                e.DrawFocusRectangle();
            }
        }
    }
    /*
    public class SFGeneratorItemBase
    {

    }
    public class SFGeneratorItem
    {
        public string typeName;
        public SFGeneratorItemType type;

        //public Func<Tin, Tout> exec;
        public Func<ushort, double> ushort2double;
        public Func<short, double> short2double;
        public Func<short, short> short2short;
        public Func<short, ushort> short2ushort;
        public Func<ushort, ushort> ushort2ushort;
        public Func<ushort, short> ushort2short;
    }
    private void button1_Click_1(object sender, EventArgs e)
    {
        StringBuilder sb = new StringBuilder();

        Dictionary<int, SFGeneratorItem> SFGeneratorDefs = new Dictionary<int, SFGeneratorItem>()
            {
                { 0, new SFGeneratorItem{typeName="test1", type=SFGeneratorItemType.short_t, ushort2double=(x) => (double)(x*x)}}
            };


    }*/
}
