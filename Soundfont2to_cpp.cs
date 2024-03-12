using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Soundfont2
{
    public class Soundfont2to_cpp
    {
        public static string getcpp(sfbk_rec sfbk)
        {
            string SAMPLE_ARRAY_NAME = "";
            bool LOOP = true;
            int LENGTH_BITS = 16;
            int CENTS_OFFSET = 0;
            string SAMPLE_RATE = 44100.ToString("F1").Replace(',','.');
            int SAMPLE_NOTE = 72;
            int LENGTH = 0;
            int LOOP_START = 0;
            int LOOP_END = 0;
            string INIT_ATTENUATION = (-8.2f).ToString("F2").Replace(',', '.');
            string DELAY_ENV = 0.0f.ToString("F2").Replace(',','.');
            string ATTACK_ENV = 6.2f.ToString("F2").Replace(',', '.');
            string HOLD_ENV = 0.98f.ToString("F2").Replace(',', '.');
            string DECAY_ENV = 1.0f.ToString("F2").Replace(',', '.');
            string RELEASE_ENV = 1.0f.ToString("F2").Replace(',', '.');
            string SUSTAIN_FRAC = (-0.0f).ToString("F1").Replace(',', '.');
            string VIB_DELAY_ENV = 0.0f.ToString("F2").Replace(',', '.');
            string VIB_INC_ENV = 0.1f.ToString("F1").Replace(',', '.');
            int VIB_PITCH_INIT = 0;
            int VIB_PITCH_SCND = 0;
            string MOD_DELAY_ENV = 0.0f.ToString("F2").Replace(',', '.');
            string MOD_INC_ENV = 5.4f.ToString("F1").Replace(',', '.');
            int MOD_PITCH_INIT = 0;
            int MOD_PITCH_SCND = 0;
            int MOD_AMP_INIT_GAIN = 0;
            int MOD_AMP_SCND_GAIN = 0;

            string  out_fmt_str =
                "\t{{\n" +
                $"\t\t(int16_t*){SAMPLE_ARRAY_NAME}, // sample\n" +
                $"\t\t{LOOP}, // LOOP\n" +
                $"\t\t{LENGTH_BITS}, // LENGTH_BITS\n" +
                $"\t\t(1 << (32 - {LENGTH_BITS})) * WAVETABLE_CENTS_SHIFT({CENTS_OFFSET}) * {SAMPLE_RATE} / WAVETABLE_NOTE_TO_FREQUENCY({SAMPLE_NOTE}) / AUDIO_SAMPLE_RATE_EXACT + 0.5, // PER_HERTZ_PHASE_INCREMENT\n" +
                $"\t\t((uint32_t){LENGTH} - 1) << (32 - {LENGTH_BITS}), // MAX_PHASE\n" +
                $"\t\t((uint32_t){LOOP_END} - 1) << (32 - {LENGTH_BITS}), // LOOP_PHASE_END\n" +
                $"\t\t(((uint32_t){LOOP_END} - 1) << (32 - {LENGTH_BITS})) - (((uint32_t){LOOP_START} - 1) << (32 - {LENGTH_BITS})), // LOOP_PHASE_LENGTH\n" +
                $"\t\tuint16_t(UINT16_MAX * WAVETABLE_DECIBEL_SHIFT({INIT_ATTENUATION})), // INITIAL_ATTENUATION_SCALAR\n" +
                $"\t\tuint32_t({DELAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5), // DELAY_COUNT\n" +
                $"\t\tuint32_t({ATTACK_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5), // ATTACK_COUNT\n" +
                $"\t\tuint32_t({HOLD_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5), // HOLD_COUNT\n" +
                $"\t\tuint32_t({DECAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5), // DECAY_COUNT\n" +
                $"\t\tuint32_t({RELEASE_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / AudioSynthWavetable::ENVELOPE_PERIOD + 0.5), // RELEASE_COUNT\n" +
                $"\t\tint32_t((1.0 - WAVETABLE_DECIBEL_SHIFT({SUSTAIN_FRAC})) * AudioSynthWavetable::UNITY_GAIN), // SUSTAIN_MULT\n" +
                $"\t\tuint32_t({VIB_DELAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / (2 * AudioSynthWavetable::LFO_PERIOD)), // VIBRATO_DELAY\n" +
                $"\t\tuint32_t({VIB_INC_ENV} * AudioSynthWavetable::LFO_PERIOD * (UINT32_MAX / AUDIO_SAMPLE_RATE_EXACT)), // VIBRATO_INCREMENT\n" +
                $"\t\t(WAVETABLE_CENTS_SHIFT({VIB_PITCH_INIT}) - 1.0) * 4, // VIBRATO_PITCH_COEFFICIENT_INITIAL\n" +
                $"\t\t(1.0 - WAVETABLE_CENTS_SHIFT({VIB_PITCH_SCND})) * 4, // VIBRATO_COEFFICIENT_SECONDARY\n" +
                $"\t\tuint32_t({MOD_DELAY_ENV} * AudioSynthWavetable::SAMPLES_PER_MSEC / (2 * AudioSynthWavetable::LFO_PERIOD)), // MODULATION_DELAY\n" +
                $"\t\tuint32_t({MOD_INC_ENV} * AudioSynthWavetable::LFO_PERIOD * (UINT32_MAX / AUDIO_SAMPLE_RATE_EXACT)), // MODULATION_INCREMENT\n" +
                $"\t\t(WAVETABLE_CENTS_SHIFT({MOD_PITCH_INIT}) - 1.0) * 4, // MODULATION_PITCH_COEFFICIENT_INITIAL\n" +
                $"\t\t(1.0 - WAVETABLE_CENTS_SHIFT({MOD_PITCH_SCND})) * 4, // MODULATION_PITCH_COEFFICIENT_SECOND\n" +
                $"\t\tint32_t(UINT16_MAX * (WAVETABLE_DECIBEL_SHIFT({MOD_AMP_INIT_GAIN}) - 1.0)) * 4, // MODULATION_AMPLITUDE_INITIAL_GAIN\n" +
                $"\t\tint32_t(UINT16_MAX * (1.0 - WAVETABLE_DECIBEL_SHIFT({MOD_AMP_SCND_GAIN}))) * 4, // MODULATION_AMPLITUDE_FINAL_GAIN\n" +
                "\t}},\n";

            return out_fmt_str;
        }
    }
}
