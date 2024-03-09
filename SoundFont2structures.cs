using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BYTE = System.Byte;
using CHAR = System.SByte;
using WORD = System.UInt16;
using DWORD = System.UInt32;
using SHORT = System.Int16;

namespace Soundfont2
{
    public class RIFF
    {
        public long size; // is the whole file size
        public sfbk_rec sfbk;
        public RIFF()
        {
            size = 0;
            sfbk = new sfbk_rec();
        }
    }
    public class sfbk_rec
    {
        public UInt32 size; // comes from parent RIFF
        public INFO info;
        public sdta_rec sdta;
        public pdta_rec pdta;
        public sfbk_rec()
        {
            size = 0;
            info = new INFO();
            sdta = new sdta_rec();
            pdta = new pdta_rec();
        }
    }

    /// <summary> </summary>
    public class sfVersionTag
    {
        public UInt16 major;
        public UInt16 minor;
        public sfVersionTag()
        {
            major = 0;
            minor = 0;
        }
        public sfVersionTag(UInt32 val)
        {
            major = (UInt16)(val & 0xFFFF);
            minor = (UInt16)((val & 0xFFFF0000) >> 16);
        }
        /// <summary>
        /// Returns the version as major.minor with zero padding at minor
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return major.ToString() + "." + minor.ToString().PadLeft(2, '0');
        }
        public static implicit operator sfVersionTag(UInt32 val)
        {
          return new sfVersionTag(val);
        }
    }
    public class INFO
    {
        public UInt32 size; // comes from the parent LIST
        // mandatory data fields

        /// <summary>The SoundFont specification version</summary>
        public sfVersionTag ifil;
        /// <summary>The sound engine for which the SoundFont was optimized</summary>
        public string isng;
        /// <summary>The name of the SoundFont</summary>
        public string INAM;

        // optional data fields

        /// <summary>A sound data ROM to which any ROM samples refer</summary>
        public string irom;
        /// <summary>A sound data ROM revision to which any ROM samples refer</summary>
        public sfVersionTag iver;
        /// <summary>The creation date of the SoundFont, conventionally in the 'Month Day, Year' format</summary>
        public string ICRD;
        /// <summary>The author or authors of the SoundFont</summary>
        public string IENG;
        /// <summary>The product for which the SoundFont is intended</summary>
        public string IPRD;
        /// <summary>Copyright assertion string associated with the SoundFont</summary>
        public string ICOP;
        /// <summary>Any comments associated with the SoundFont</summary>
        public string ICMT;
        /// <summary>The tool used to create or edit the SoundFont</summary>
        public string ISFT;

        public INFO()
        {
            size = 0;
            ifil = new sfVersionTag();
            isng = "";
            INAM = "";
            irom = "";
            iver = new sfVersionTag();
            ICRD = "";
            IENG = "";
            IPRD = "";
            ICOP = "";
            ICMT = "";
            ISFT = "";
        }
        public override string ToString()
        {
            string ret = "*** Info *** \r\n";
            ret += "size: "+ size.ToString() + "\r\n";
            ret += "Soundfont version: " + ifil.ToString() +"\r\n";
            ret += "Name: " + INAM + "\r\n";
            ret += "SoundEngine: " + isng + "\r\n";
            ret += "ROM: " + irom + "\r\n";
            ret += "ROM ver: " + iver.ToString() + "\r\n";
            ret += "Date: " + ICRD + "\r\n";
            ret += "Credits: " + IENG + "\r\n";
            ret += "Product: " + IPRD + "\r\n";
            ret += "Copyright: " + ICOP + "\r\n";
            ret += "Comment: " + ICMT + "\r\n";
            ret += "Tools: " + ISFT + "\r\n";
            return ret;
        }
    }

    /// <summary>sample data offset pointers (data is loaded from file on demand)</summary>
    public class sdta_rec
    {
        public UInt32 size; // comes from parent LIST
        /// <summary>smpl data offset as from the beginning of the file</summary>
        public UInt32 smpl;
        public UInt32 smplSize;
        /// <summary>sm24 data offset as from the beginning of the file</summary>
        public UInt32 sm24;
        public UInt32 sm24Size;
        
        public sdta_rec()
        {
            size = 0;
            smpl = 0;
            sm24 = 0;
        }
    }

    /// <summary>preset data which contains the instrument and preset definitions </summary>
    public class pdta_rec
    {
        public UInt32 size; // comes from parent LIST
        /// <summary>The Preset Headers</summary>
        public List<phdr_rec> phdr;
        /// <summary>The Preset Index list</summary>
        public List<pbag_rec> pbag;
        /// <summary>The Preset Modulator list</summary>
        public List<pmod_rec> pmod;
        /// <summary>The Preset Generator list</summary>
        public List<pgen_rec> pgen;
        /// <summary>The Instrument Names and Indices</summary>
        public List<inst_rec> inst;
        /// <summary>The Instrument Index list</summary>
        public List<ibag_rec> ibag;
        /// <summary>The Instrument Modulator list</summary>
        public List<imod_rec> imod;
        /// <summary>The Instrument Generator list</summary>
        public List<igen_rec> igen;
        /// <summary>The Sample Headers</summary>
        public List<shdr_rec> shdr;

        public pdta_rec()
        {
            phdr = new List<phdr_rec>();
            pbag = new List<pbag_rec>();
            pmod = new List<pmod_rec>();
            pgen = new List<pgen_rec>();
            inst = new List<inst_rec>();
            ibag = new List<ibag_rec>();
            imod = new List<imod_rec>();
            igen = new List<igen_rec>();
            shdr = new List<shdr_rec>();
        }
    }

    /// <summary>preset data headers</summary>
    public class phdr_rec // 38 bytes each item
    {
        public string achPresetName; // max lenght of 20
        public WORD wPreset;
        public WORD wBank;
        public WORD wPresetBagNdx;
        public DWORD dwLibrary;
        public DWORD dwGenre;
        public DWORD dwMorphology;

        public phdr_rec()
        {
            achPresetName = "";
            wPreset = 0;
            wBank = 0;
            wPresetBagNdx = 0;
            dwLibrary = 0;
            dwGenre = 0;
            dwMorphology = 0;
        }
    }
    public class pbag_rec
    {
        public WORD wGenNdx;
        public WORD wModNdx;

        public pbag_rec()
        {
            wGenNdx = 0;
            wModNdx = 0;
        }
    }
    /// <summary> sfModList</summary>
    public class pmod_rec
    {
        public SFModulator sfModSrcOper;
        public SFGenerator sfModDestOper;
        public SHORT modAmount;
        public SFModulator sfModAmtSrcOper;
        public SFTransform sfModTransOper;

        public pmod_rec()
        {
            sfModSrcOper = new SFModulator();
            sfModDestOper = SFGenerator.startAddrsOffset;
            modAmount = 0;
            sfModAmtSrcOper = new SFModulator();
            sfModTransOper = SFTransform.absoluteValue;
        }
    }
    public class pgen_rec
    {
        public SFGenerator sfGenOper;
        public genAmountType genAmount;
        
        public pgen_rec()
        {
            sfGenOper = SFGenerator.startAddrsOffset;
            genAmount = new genAmountType();
        }
    }
    public class inst_rec
    {
        public string achInstName;
        public WORD wInstBagNdx;

        public inst_rec()
        {
            achInstName = "";
            wInstBagNdx = 0;
        }
    }
    public class ibag_rec
    {
        public WORD wInstGenNdx;
        public WORD wInstModNdx;

        public ibag_rec()
        {
            wInstGenNdx = 0;
            wInstModNdx = 0;
        }
    }
    public class imod_rec
    {
        public SFModulator sfModSrcOper;
        public SFGenerator sfModDestOper;
        public SHORT modAmount;
        public SFModulator sfModAmtSrcOper;
        public SFTransform sfModTransOper;

        public imod_rec()
        {
            sfModSrcOper = new SFModulator();
            sfModDestOper = SFGenerator.startAddrsOffset;
            modAmount = 0;
            sfModAmtSrcOper = new SFModulator();
            sfModTransOper = SFTransform.absoluteValue;
        }
    }
    public class igen_rec
    {
        public SFGenerator sfGenOper;
        public genAmountType genAmount;

        public igen_rec()
        {
            sfGenOper = new SFGenerator();
            genAmount = new genAmountType();
        }
    }
    public class shdr_rec
    {
        public string achSampleName;
        public DWORD dwStart;
        public DWORD dwEnd;
        public DWORD dwStartloop;
        public DWORD dwEndloop;
        public DWORD dwSampleRate;
        public BYTE byOriginalKey;
        public CHAR chCorrection;
        public WORD wSamepleLink;
        public SFSampleLink sfSampleType;

        public shdr_rec()
        {
            achSampleName = "";
            dwStart = 0;
            dwEnd = 0;
            dwStartloop = 0;
            dwEndloop = 0;
            dwSampleRate = 0;
            byOriginalKey = 0;
            chCorrection = 0;
            wSamepleLink = 0;
            sfSampleType = SFSampleLink.monoSample;
        }
    }

    public class SFModulator // this is a little overcomplicated as C# cannot handle bitfields
    {
        public UInt16 rawdata = 0;
        /// <summary>A 6 bit value specifying the continuity of the controller</summary>
        public byte Type
        {
            get { return (byte)((rawdata >> 10) & 0x1F); }
            set { rawdata = (UInt16)((rawdata & 0x03FF) | ((value & 0x1F) << 10)); }
        }
        /// <summary>Polarity</summary>
        public byte P
        {
            get { return (byte)((rawdata >> 9) & 0x01); }
            set { rawdata = (UInt16)((rawdata & 0xFDFF) | ((value & 0x01) << 9)); }
        }
        /// <summary>Direction</summary>
        public byte D
        {
            get { return (byte)((rawdata >> 8) & 0x01); }
            set { rawdata = (ushort)((rawdata & 0xFEFF) | ((value & 0x01) << 8)); }
        }
        /// <summary>MIDI Continuous Controller Flag</summary>
        public byte CC
        {
            get { return (byte)((rawdata >> 7) & 0x01); }
            set { rawdata = (ushort)((rawdata & 0xFF7F) | ((value & 0x01) << 7)); }
        }
        /// <summary>A 7 bit value specifying the controller source</summary>
        public byte Index
        {
            get { return (byte)(rawdata & 0x7F); }
            set { rawdata = (ushort)((rawdata & 0xFF80) | (value & 0x7F)); }
        }
    }

    public class rangesType
    {
        public BYTE byLo;
        public BYTE byHi;
        
        public rangesType()
        {
            byLo = 0;
            byHi = 0;
        }
    }

    public class genAmountType
    {
        public rangesType ranges;
        public SHORT shAmount;
        public WORD wAmount;

        public genAmountType()
        {
            ranges = new rangesType();
            shAmount = 0;
            wAmount = 0;
        }
    }

    class t
    {
        
        void tf()
        {
            SFModulator sfMod = new SFModulator();
        }
    }
}
