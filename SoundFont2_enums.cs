using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using uint8_t = System.Byte;
using uint16_t = System.UInt16;

namespace Soundfont2
{
    /// <summary> 
    /// Values that represents both the type of sample (mono, stereo left, etc.)
    /// and the whether the sample is located in RAM or ROM memory.
    /// 
    /// @remarks This enumeration represents the official SFSampleLink type.
    /// @see "4.5 SoundFont 2 RIFF File Format Type Definitions".
    /// In SoundFont Technical Specification 2.04.
    /// </summary>
    enum SFSampleLink : uint16_t
    {
        /// <summary>Mono sample</summary>
        monoSample = 1,
        /// <summary>Right sample</summary>
        rightSample = 2,
        /// <summary>Left sample</summary>
        leftSample = 4,
        /// <summary>Linked sample</summary>
        linkedSample = 8,
        /// <summary>Mono sample, located in ROM</summary>
        RomMonoSample = 0x8001,
        /// <summary>Right sample, located in ROM</summary>
        RomRightSample = 0x8002,
        /// <summary>Left sample, located in ROM</summary>
        RomLeftSample = 0x8004,
        /// <summary>Linked sample, located in ROM</summary>
        RomLinkedSample = 0x8008
    }

    /// <summary>
    /// Values that represents the type of generator.
    ///
    /// @remarks This enumeration represents the official SFGenerator type.
    /// @see "8.1.2 Generator Enumerators Defined".
    /// In SoundFont Technical Specification 2.04.
    /// </summary>
    enum SFGenerator : uint16_t
    {
        startAddrsOffset = 0,
        endAddrsOffset = 1,
        startloopAddrsOffset = 2,
        endloopAddrsOffset = 3,
        startAddrsCoarseOffset = 4,
        modLfoToPitch = 5,
        vibLfoToPitch = 6,
        modEnvToPitch = 7,
        initialFilterFc = 8,
        initialFilterQ = 9,
        modLfoToFilterFc = 10,
        modEnvToFilterFc = 11,
        endAddrsCoarseOffset = 12,
        modLfoToVolume = 13,
        chorusEffectsSend = 15,
        reverbEffectsSend = 16,
        pan = 17,
        delayModLFO = 21,
        freqModLFO = 22,
        delayVibLFO = 23,
        freqVibLFO = 24,
        delayModEnv = 25,
        attackModEnv = 26,
        holdModEnv = 27,
        decayModEnv = 28,
        sustainModEnv = 29,
        releaseModEnv = 30,
        keynumToModEnvHold = 31,
        keynumToModEnvDecay = 32,
        delayVolEnv = 33,
        attackVolEnv = 34,
        holdVolEnv = 35,
        decayVolEnv = 36,
        sustainVolEnv = 37,
        releaseVolEnv = 38,
        keynumToVolEnvHold = 39,
        keynumToVolEnvDecay = 40,
        instrument = 41,
        keyRange = 43,
        velRange = 44,
        startloopAddrsCoarseOffset = 45,
        keynum = 46,
        velocity = 47,
        initialAttenuation = 48,
        endloopAddrsCoarseOffset = 50,
        coarseTune = 51,
        fineTune = 52,
        sampleID = 53,
        sampleModes = 54,
        scaleTuning = 56,
        exclusiveClass = 57,
        overridingRootKey = 58,
        endOper = 60
    }

    /// <summary>
    /// Values that represents the type of general controller.
    ///
    /// @remarks This enumeration represents the Index field of SFModulator type,
    /// while the CC field is set to 0.
    /// @see "8.2.1. Source Enumerator Controller Palettes".
    /// In SoundFont Technical Specification 2.04.
    /// </summary>
    enum SFGeneralController : uint8_t
    {
        noController = 0,
        noteOnVelocity = 2,
        noteOnKeyNumber = 3,
        polyPressure = 10,
        channelPressure = 13,
        pitchWheel = 14,
        pitchWheelSensitivity = 16,
        link = 127
    }

    /// <summary>
    /// Values that represents the type of MIDI controller.
    ///
    /// If you want to construct SFMidiController from a control number,
    /// it can be done in `SFMidiController(number)`.
    ///
    /// @remarks This enumeration represents the Index field of SFModulator type,
    /// while the CC field is set to 1.
    /// which is a kind of Modulator Source Enumerators.
    /// @see General MIDI Level 2 Recommended Practice (RP024).
    /// </summary>
    enum SFMidiController : uint8_t
    {
        kBankSelect = 0,
        kModulationDepth,
        kController2,
        kController3,
        kController4,
        kPortamentoTime,
        kDataEntry,
        kChannelVolume,
        kController8,
        kController9,
        kPan,
        kExpression,
        kController12,
        kController13,
        kController14,
        kController15,
        kController16,
        kController17,
        kController18,
        kController19,
        kController20,
        kController21,
        kController22,
        kController23,
        kController24,
        kController25,
        kController26,
        kController27,
        kController28,
        kController29,
        kController30,
        kController31,
        kBankSelectLSB,
        kController33,
        kController34,
        kController35,
        kController36,
        kController37,
        kDataEntryLSB,
        kController39,
        kController40,
        kController41,
        kController42,
        kController43,
        kController44,
        kController45,
        kController46,
        kController47,
        kController48,
        kController49,
        kController50,
        kController51,
        kController52,
        kController53,
        kController54,
        kController55,
        kController56,
        kController57,
        kController58,
        kController59,
        kController60,
        kController61,
        kController62,
        kController63,
        kHold,
        kPortamento,
        kSostenuto,
        kSoft,
        kController68,
        kController69,
        kController70,
        kFilterResonance,
        kReleaseTime,
        kAttackTime,
        kBrightness,
        kDecayTime,
        kVibratoRate,
        kVibratoDepth,
        kVibratoDelay,
        kController79,
        kController80,
        kController81,
        kController82,
        kController83,
        kController84,
        kController85,
        kController86,
        kController87,
        kController88,
        kController89,
        kController90,
        kReverbSendLevel,
        kController92,
        kChorusSendLevel,
        kController94,
        kController95,
        kController96,
        kController97,
        kNRPNLSB,
        kNRPNMSB,
        kRPNLSB,
        kRPNMSB,
        kController102,
        kController103,
        kController104,
        kController105,
        kController106,
        kController107,
        kController108,
        kController109,
        kController110,
        kController111,
        kController112,
        kController113,
        kController114,
        kController115,
        kController116,
        kController117,
        kController118,
        kController119,
        kAllSoundOff,
        kResetAllController,
        kController122,
        kAllNotesOff,
        kOmniModeOff,
        kOmniModeOn,
        kMonoModeOn,
        kPolyModeOn
    }

    /// <summary>
    /// Values that represents the type of controller palette.
    ///
    /// @remarks This enumeration represents the CC field of SFModulator type.
    /// @see "8.2 Modulator Source Enumerators".
    /// In SoundFont Technical Specification 2.04.
    /// </summary>
    enum SFControllerPalette : uint8_t
    {
        generalController = 0,
        midiController = 1
    }

    /// <summary>
    /// Values that represents the direction of controller.
    ///
    /// @remarks This enumeration represents the D field of SFModulator type.
    /// @see "8.2 Modulator Source Enumerators".
    /// In SoundFont Technical Specification 2.04.
    /// </summary>
    enum SFControllerDirection : uint8_t
    {
        increase = 0,
        decrease = 1
    }

    /// <summary>
    /// Values that represents the polarity of controller.
    ///
    /// @remarks This enumeration represents the P field of SFModulator type.
    /// @see "8.2 Modulator Source Enumerators".
    /// In SoundFont Technical Specification 2.04.
    /// </summary>
    enum SFControllerPolarity : uint8_t
    {
        unipolar = 0,
        bipolar = 1
    }

    /// <summary>
    /// Values that represents the source type of controller.
    ///
    /// @remarks This enumeration represents the Type field of SFModulator type.
    /// @see "8.2.4 Source Types".
    /// In SoundFont Technical Specification 2.04.
    /// </summary>
    enum SFControllerType : uint8_t
    {
        linearType = 0,
        concaveType = 1,
        convexType = 2,
        switchType = 3
    }

    /// <summary>
    /// Values that represents the transform type of controller.
    ///
    /// @remarks This enumeration represents the official SFTransform type.
    /// @see "8.3 Modulator Transform Enumerators".
    /// In SoundFont Technical Specification 2.04.
    /// </summary>
    enum SFTransform : uint8_t
    {
        linear = 0,
        absoluteValue = 2
    }

    /// <summary>
    /// Values that represents the bit flags for the sampleModes generator.
    /// </summary>
    enum SampleMode : uint16_t
    {
        /// <summary>Indicates a sound reproduced with no loop.</summary> 
        kNoLoop = 0,
        /// <summary>Indicates a sound which loops continuously.</summary> 
        kLoopContinuously,
        /// <summary>Unused but should be interpreted as indicating no loop.</summary> 
        kUnusedNoLoop,
        /// <summary>Indicates a sound which loops for the duration of key depression then proceeds to play the remainder of the sample.</summary> 
        kLoopEndsByKeyDepression
    }
}
