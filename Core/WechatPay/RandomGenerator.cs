using System.Security.Cryptography;

namespace shunda.Core.WechatPay;

public class RandomGenerator {
    private readonly RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

    public int Next(int minValue, int maxExclusiveValue) {
        if (minValue >= maxExclusiveValue) {
            throw new ArgumentOutOfRangeException(nameof(maxExclusiveValue), "minValue must be lower than maxExclusiveValue");
        }

        long diff = (long)maxExclusiveValue - minValue;
        long upperBound = uint.MaxValue / diff * diff;

        uint ui;
        do {
            ui = GetRandomUInt();
        } while (ui >= upperBound);

        return (int)(minValue + (ui % diff));
    }

    public uint GetRandomUInt() {
        var randomBytes = GenerateRandomBytes(sizeof(uint));
        return BitConverter.ToUInt32(randomBytes, 0);
    }

    private byte[] GenerateRandomBytes(int bytesNumber) {
        byte[] buffer = new byte[bytesNumber];
        randomNumberGenerator.GetBytes(buffer);
        return buffer;
    }
}