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
    
    struct Root
    {

    }

    /// <summary> </summary>
    struct sfVersionTag
    {
        int major;
        int minor;
    }
    struct INFO
    {
        // mandotary data fields

        /// <summary>The SoundFont specification version</summary>
        sfVersionTag ifil;
        /// <summary>The sound engine for which the SoundFont was optimized</summary>
        string isng;
        /// <summary>The name of the SoundFont</summary>
        string INAM;

        // optional data fields

        /// <summary>A sound data ROM to which any ROM samples refer</summary>
        string irom;
        /// <summary>A sound data ROM revision to which any ROM samples refer</summary>
        sfVersionTag iver;
        /// <summary>The creation date of the SoundFont, conventionally in the 'Month Day, Year' format</summary>
        string ICRD;
        /// <summary>The author or authors of the SoundFont</summary>
        string IENG;
        /// <summary>The product for which the SoundFont is intended</summary>
        string IPDR;
        /// <summary>Copyright assertion string associated with the SoundFont</summary>
        string ICOP;
        /// <summary>Any comments associated with the SoundFont</summary>
        string ICMT;
        /// <summary>The tool used to create or edit the SoundFont</summary>
        string ISFT;
    }

    /// <summary>sample data offset pointers (data is loaded from file on demand)</summary>
    struct sdta
    {
        /// <summary>smpl data offset as from the beginning of the file</summary>
        UInt32 smpl;
        /// <summary>sm24 data offset as from the beginning of the file</summary>
        UInt32 sm24;
        
    }

    /// <summary>preset data which contains the instrument and preset definitions </summary>
    struct pdta
    {
        /// <summary>The Preset Headers</summary>
        phdr_rec phdr;
        /// <summary>The Preset Index list</summary>
        pbag_rec pbag;
        /// <summary>The Preset Modulator list</summary>
        pmod_rec pmod;
        /// <summary>The Preset Generator list</summary>
        pgen_rec pgen;
        /// <summary>The Instrument Names and Indices</summary>
        inst_rec inst;
        /// <summary>The Instrument Index list</summary>
        ibag_rec ibag;
        /// <summary>The Instrument Modulator list</summary>
        imod_rec imod;
        /// <summary>The Instrument Generator list</summary>
        igen_rec igen;
        /// <summary>The Sample Headers</summary>
        shdr_rec shdr;
    }

    /// <summary>preset data headers</summary>
    struct phdr_rec
    {
        string achPresetName; // max lenght of 20
        WORD wPreset;
        WORD wBank;
        WORD wPresetBagNdx;
        DWORD dwLibrary;
        DWORD dwGenre;
        DWORD dwMorphology;
    }
    struct pbag_rec
    {
        WORD wGenNdx;
        WORD wModNdx;
    }
    /// <summary> sfModList</summary>
    struct pmod_rec
    {
        SFModulator sfModSrcOper;
        SFGenerator sfModDestOper;
        SHORT modAmount;
        SFModulator sdModAmtSrcOper;
        SFTransform sfModTransOper;
    }
    struct pgen_rec
    {
        SFGenerator sfGenOper;
        genAmountType genAmount;
    }
    struct inst_rec
    {
        string achInstName;
        WORD wInstBagNdx;
    }
    struct ibag_rec
    {
        WORD wInstGenNdx;
        WORD wInstModNdx;
    }
    struct imod_rec
    {
        SFModulator sfModSrcOper;
        SFGenerator sfModDestOper;
        SHORT modAmount;
        SFModulator sdModAmtSrcOper;
        SFTransform sfModTransOper;
    }
    struct igen_rec
    {
        SFGenerator sfGenOper;
        genAmountType genAmount;
    }
    struct shdr_rec
    {
        string achSampleName;
        DWORD dwStart;
        DWORD dwEnd;
        DWORD dwStartloop;
        DWORD dwEndloop;
        DWORD dwSampleRate;
        BYTE byOriginalKey;
        CHAR chCorrection;
        WORD wSamepleLink;
        SFSampleLink sfSampleType;
    }

    struct SFModulator // this is a little overcomplicated as C# cannot handle bitfields
    {
        public UInt16 rawdata;
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

    struct rangesType
    {
        BYTE byLo;
        BYTE byHi;
    }

    struct genAmountType
    {
        rangesType ranges;
        SHORT shAmount;
        WORD wAmount;
    }

    class t
    {
        
        void tf()
        {
            SFModulator sfMod = new SFModulator();
        }
    }
}
