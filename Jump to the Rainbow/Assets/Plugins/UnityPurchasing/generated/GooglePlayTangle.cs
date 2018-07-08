#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("IDHZkdzse2lOPo4y3blaOK+Jmn2r4/btpYg+jNVOllYp4mzjhYj5hgKvjQCxqOiXvRQVFxYXJpQXNCYbR1x4GJZILmdzMVGM83y6x6ByoVozfH4F75WcDdPjnAhaTrs9a8ggT3tpTj6OMt25WjiviZp98O0Htwl29u/dnu3ZsMsd0Q0aq01jsDwuhjQcfQYWnrRllSdsUpnDB/upysOlgqDw21oelR8p0Qx7FUMNDQ93PlGhYM7S7XE3EDx7hfPeZk4gMdmR3Oxs44WI+YZ6EQH49wHUqHMIxRWlXJR36L6Z7INLYgAONtwVVhme0uBGVhme0uBG9u/dnu3ZsMsd0Q0aq007RRpa3RUVWVqG1tIjkz9zk7xnVRccFJQXFxaP0j6WlicfgXfEMatduz1ryCBPlHfovpnsg0tiAA423BUuZ3MxUYzzfLrHoHKhWgKvjQCxqBAfPJBekOEbFxcXExYVlBcZFiaUORdO6Zq6M3x+Be+VnA3T45wIWk56EQH49wHUqHMIxRWlXDtFGlrdFRVZWobW0iOTP3OTvGdVORdO6Zq6gXfEMatdq+P27aWIPozVTpZWKeJllSdsUpnDB/upysOlgkdceBiWSCaUFzQmGxAfPJBekOEbFxcXExYV8O0Htwl2oPDbWh6VHynRDHsVQw1jsDwuhjRgztLtcTcQPHuF895mTpQXGRYmlBccFJQXFxaP0j6WlicfDQ93PlGhRcAhStr+1j/GUsd0ML1FwCFK2v7WP8ZSx3QwvRx9BhaetOiXvRQVFxYX");
        private static int[] order = new int[] { 9,17,14,13,20,24,22,12,25,23,13,21,12,20,16,22,28,22,24,24,24,28,27,23,24,25,27,28,28,29 };
        private static int key = 22;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
